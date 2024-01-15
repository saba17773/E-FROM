using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Master.All
{
    public class RouteModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Route_Add { get; set; }
        [BindProperty]
        public int IsActive_Add { get; set; }
        [BindProperty]
        public int RouteId_Edit { get; set; }
        [BindProperty]
        public string Route_Edit { get; set; }
        [BindProperty]
        public int IsActive_Edit { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public RouteModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            await _authService.CanAccess(nameof(QueingPermissionModel.ADMIN_QUEING));
            return Page();
        }
        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        ROUTE = "ROUTE"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<QingMaster_Route_TB>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT * FROM TB_QingMaster_Route
                        )T
                        WHERE " + filter + @"
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnPostAsync(string add, string edit)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RouteRepo = new GenericRepository<QingMaster_Route_TB>(unitOfWork.Transaction);

                    if (!string.IsNullOrEmpty(add))
                    {
                        await RouteRepo.InsertAsync(new QingMaster_Route_TB
                        {
                            ROUTE = Route_Add,
                            ISACTIVE = IsActive_Add == 1 ? 1 : 0
                        });

                        AlertSuccess = "Add Route Success";
                    }

                    if (!string.IsNullOrEmpty(edit))
                    {
                        var RouteById = await RouteRepo.GetAsync(RouteId_Edit);

                        RouteById.ROUTE = Route_Edit;
                        RouteById.ISACTIVE = IsActive_Edit == 1 ? 1 : 0;

                        await RouteRepo.UpdateAsync(RouteById);

                        AlertSuccess = "Edit Route Success";
                    }

                    unitOfWork.Complete();
                    return Redirect($"/Queing/Master/All/Route");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Master/All/Route");
            }
        }
    }
}
