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
    public class TranspotCategoryModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Description_Add { get; set; }
        [BindProperty]
        public int IsAgent_Add { get; set; }
        [BindProperty]
        public int IsActive_Add { get; set; }
        [BindProperty]
        public string Description_Edit { get; set; }
        [BindProperty]
        public int IsAgent_Edit { get; set; }
        [BindProperty]
        public int IsActive_Edit { get; set; }
        [BindProperty]
        public int Id_Edit { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public TranspotCategoryModel(
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
                        DESCRIPTION = "DESCRIPTION"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<QingMaster_TranspotCategory_TB>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT * FROM TB_QingMaster_TranspotCategory
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
                    var TranspotRepo = new GenericRepository<QingMaster_TranspotCategory_TB>(unitOfWork.Transaction);

                    if (!string.IsNullOrEmpty(add))
                    {
                        await TranspotRepo.InsertAsync(new QingMaster_TranspotCategory_TB
                        {
                            DESCRIPTION = Description_Add,
                            ISAGENT = IsAgent_Add == 1 ? 1 : 0,
                            ISACTIVE = IsActive_Add == 1 ? 1 : 0
                        });

                        AlertSuccess = "Add Transpot Category Success";
                    }

                    if (!string.IsNullOrEmpty(edit))
                    {
                        var TranspotById = await TranspotRepo.GetAsync(Id_Edit);

                        TranspotById.DESCRIPTION = Description_Edit;
                        TranspotById.ISAGENT = IsAgent_Edit == 1 ? 1 : 0;
                        TranspotById.ISACTIVE = IsActive_Edit == 1 ? 1 : 0;

                        await TranspotRepo.UpdateAsync(TranspotById);

                        AlertSuccess = "Edit Transpot Category Success";
                    }

                    unitOfWork.Complete();
                    return Redirect($"/Queing/Master/All/TranspotCategory");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Master/All/TranspotCategory");
            }
        }
    }
}
