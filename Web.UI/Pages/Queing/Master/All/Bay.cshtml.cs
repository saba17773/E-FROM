using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Master.All
{
    public class BayModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Bay_Add { get; set; }
        [BindProperty]
        public string Plant_Add { get; set; }
        public List<SelectListItem> PlantAddMaster { get; set; }
        [BindProperty]
        public int IsActive_Add { get; set; }
        [BindProperty]
        public string IsOVS_Add { get; set; }
        [BindProperty]
        public string IsDOM_Add { get; set; }
        [BindProperty]
        public int BayId_Edit { get; set; }
        [BindProperty]
        public string Bay_Edit { get; set; }
        [BindProperty]
        public string Plant_Edit { get; set; }
        [BindProperty]
        public int IsActive_Edit { get; set; }
        [BindProperty]
        public string IsOVS_Edit { get; set; }
        [BindProperty]
        public string IsDOM_Edit { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public BayModel(
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

            PlantAddMaster = await GetPlantAdd();

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
                        BAY = "BAY"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<QingMaster_Bay_TB>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT * FROM TB_QingMaster_Bay
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
                    var BayRepo = new GenericRepository<QingMaster_Bay_TB>(unitOfWork.Transaction);

                    if (!string.IsNullOrEmpty(add))
                    {
                        await BayRepo.InsertAsync(new QingMaster_Bay_TB
                        {
                            PLANT = Plant_Add,
                            BAY = Bay_Add,
                            ISDOM = IsDOM_Add == "1" ? 1 : 0,
                            ISOVS = IsOVS_Add == "1" ? 1 : 0,
                            ISACTIVE = IsActive_Add == 1 ? 1 : 0
                        });

                        AlertSuccess = "Add Bay Success";
                    }

                    if (!string.IsNullOrEmpty(edit))
                    {
                        var BayById = await BayRepo.GetAsync(BayId_Edit);

                        BayById.BAY = Bay_Edit;
                        BayById.ISDOM = IsDOM_Edit == "1" ? 1 : 0;
                        BayById.ISOVS = IsOVS_Edit == "1" ? 1 : 0;
                        BayById.ISACTIVE = IsActive_Edit == 1 ? 1 : 0;

                        await BayRepo.UpdateAsync(BayById);

                        AlertSuccess = "Edit Bay Success";
                    }

                    unitOfWork.Complete();
                    return Redirect($"/Queing/Master/All/Bay");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Master/All/Bay");
            }
        }
        private async Task<List<SelectListItem>> GetPlantAdd()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var PlantRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                var PlantALL = await PlantRepo.GetAllAsync();

                unitOfWork.Complete();

                return PlantALL
                    .Where(x => x.IsQingCompany == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.company,
                        Text = x.company,
                    }).ToList();
            }
        }
    }
}
