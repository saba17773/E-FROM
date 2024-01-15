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
    public class TruckCategoryModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Description_Add { get; set; }
        [BindProperty]
        public int IsActive_Add { get; set; }
        [BindProperty]
        public string IsProcess_Add { get; set; }
        [BindProperty]
        public int STDTime_Add { get; set; }
        [BindProperty]
        public int CateId_Edit { get; set; }
        [BindProperty]
        public string Description_Edit { get; set; }
        [BindProperty]
        public int IsActive_Edit { get; set; }
        [BindProperty]
        public string IsProcess_Edit { get; set; }
        [BindProperty]
        public int STDTime_Edit { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public TruckCategoryModel(
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

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<QingMaster_TruckCategory_TB>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT * FROM TB_QingMaster_TruckCategory
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
                    var TruckCateRepo = new GenericRepository<QingMaster_TruckCategory_TB>(unitOfWork.Transaction);
                    

                    if (!string.IsNullOrEmpty(add))
                    {
                        int isdom; int isovs;
                        if (IsProcess_Add == "DOM")
                        {
                            isdom = 1;
                            isovs = 0;
                        }
                        else
                        {
                            isdom = 0;
                            isovs = 1;
                        }

                        await TruckCateRepo.InsertAsync(new QingMaster_TruckCategory_TB
                        {
                            DESCRIPTION = Description_Add,
                            STDTIME = STDTime_Add,
                            ISDOM = isdom,
                            ISOVS = isovs,
                            ISACTIVE = IsActive_Add == 1 ? 1 : 0
                        });

                        AlertSuccess = "Add Truck Category Success";
                    }

                    if (!string.IsNullOrEmpty(edit))
                    {
                        int isdom; int isovs;
                        if (IsProcess_Edit == "DOM")
                        {
                            isdom = 1;
                            isovs = 0;
                        }
                        else
                        {
                            isdom = 0;
                            isovs = 1;
                        }

                        var TruckCateById = await TruckCateRepo.GetAsync(CateId_Edit);

                        TruckCateById.DESCRIPTION = Description_Edit;
                        TruckCateById.STDTIME = STDTime_Edit;
                        TruckCateById.ISDOM = isdom;
                        TruckCateById.ISOVS = isovs;
                        TruckCateById.ISACTIVE = IsActive_Edit == 1 ? 1 : 0;

                        await TruckCateRepo.UpdateAsync(TruckCateById);

                        AlertSuccess = "Edit Truck Category Success";
                    }

                    unitOfWork.Complete();
                    return Redirect($"/Queing/Master/All/TruckCategory");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Master/All/TruckCategory");
            }
        }
    }
}
