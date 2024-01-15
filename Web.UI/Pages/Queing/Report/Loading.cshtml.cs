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
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.Queing;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Report
{
    public class LoadingModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public string PlantView { get; set; }
        [BindProperty]
        public int Checker { get; set; }
        public List<SelectListItem> CheckerMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public LoadingModel(
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
        public async Task<IActionResult> OnGetAsync(string plant)
        {
            try
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.VIEW_QUEING_REPORT));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    Plant = plant;

                    var PlantRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                    var PlantALL = await PlantRepo.GetAllAsync();
                    var PlantbyPlantCode = PlantALL.Where(x => x.company == plant).FirstOrDefault();

                    PlantView = PlantbyPlantCode.FullName_EN;
                    CheckerMaster = await GetCheckerMaster(plant);

                }

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Report/{plant}/Index");
            }
        }
        public async Task<List<SelectListItem>> GetCheckerMaster(string plant)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var data = await unitOfWork.Transaction.Connection.QueryAsync<CheckerGridModel>(@"
                            SELECT *
                            FROM 
                            (
                                SELECT Q.USERID,U.EmployeeId + ' : ' + U.Email USERDETAIL
                                FROM TB_QingMaster_UserMapping Q JOIN
                                TB_User U ON Q.USERID = U.Id
                                WHERE Q.PLANT = @PLANT
                                GROUP BY Q.USERID,U.EmployeeId,U.Email
                            )T
                        ",
                         new
                         {
                             @PLANT = plant
                         },
                         unitOfWork.Transaction);

                unitOfWork.Complete();

                return data.Select(x => new SelectListItem
                {
                    Value = x.USERID.ToString(),
                    Text = x.USERDETAIL
                }).ToList();
            }
        }
        public async Task<JsonResult> OnPostGridAsync(string plant, string CheckOutDate, string CheckOutBy, string Status, string Process)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<QingReportLoadingModel>(@"
                            EXEC QUEING_REPORTLOADING @pPlant,@pCheckOutDate,@pCheckOutBy,@pStatus,@pProcess
                        ",
                         new
                         {
                             @pPlant = plant,
                             @pCheckOutDate = CheckOutDate,
                             @pCheckOutBy = CheckOutBy,
                             @pStatus = Status,
                             @pProcess = Process
                         },
                         unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
