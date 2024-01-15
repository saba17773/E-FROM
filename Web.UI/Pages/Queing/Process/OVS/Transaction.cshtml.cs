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

namespace Web.UI.Pages.Queing.Process.OVS
{
    public class TransactionModel : PageModel
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
        public string TruckID { get; set; }
        [BindProperty]
        public decimal WeightIN { get; set; }
        [BindProperty]
        public string WeightINDate { get; set; }
        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        public int WeighId { get; set; }

        [BindProperty]
        public int Status { get; set; }
        public List<SelectListItem> StatusMaster { get; set; }
        [BindProperty]
        public int TruckCate { get; set; }
        public List<SelectListItem> TruckCateMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public TransactionModel(
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
                await _authService.CanAccess(nameof(QueingPermissionModel.VIEW_QUEING));
                await GetData(plant);


                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/OVS/{plant}/Index");
            }
        }
        public async Task GetData(string plant)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                Plant = plant;

                var PlantRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                var PlantALL = await PlantRepo.GetAllAsync();
                var PlantbyPlantCode = PlantALL.Where(x => x.company == plant).FirstOrDefault();

                PlantView = PlantbyPlantCode.FullName_EN;

                StatusMaster = await GetStatusMaster();
                TruckCateMaster = await GetSTruckCateMaster();


            }
        }
        public async Task<JsonResult> OnPostGridAsync(string plant,string CheckInDateFrom, string CheckInDateTo, 
                                                      string TruckID, string TruckCateID, string WorkOrderNo, 
                                                      string Status, int isFristLoad)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    if (isFristLoad == 1)
                    {
                        var data = await unitOfWork.Transaction.Connection.QueryAsync<QingOVSGridModel>(@"
                            EXEC Queing_ViewTransactionOVS @pCompany
                        ", 
                        new
                        {
                            @pCompany = plant
                        }, 
                        unitOfWork.Transaction);

                        unitOfWork.Complete();

                        return new JsonResult(_datatableService.Format(Request, data.ToList()));
                    }
                    else
                    {
                        var data = await unitOfWork.Transaction.Connection.QueryAsync<QingOVSGridModel>(@"
                            EXEC Queing_ViewTransactionOVS @pCompany,@pCheckInDateFrom,@pCheckInDateTo,
                                                            @pTruckID,@pTruckCateID,@pWorkOrderNo,@pStatus
                        ", 
                        new
                        {
                            @pCompany = plant,
                            @pCheckInDateFrom = CheckInDateFrom,
                            @pCheckInDateTo = CheckInDateTo,
                            @pTruckID = TruckID,
                            @pTruckCateID = TruckCateID,
                            @pWorkOrderNo = WorkOrderNo,
                            @pStatus = Status
                        },
                        unitOfWork.Transaction);

                        unitOfWork.Complete();

                        return new JsonResult(_datatableService.Format(Request, data.ToList()));
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<SelectListItem>> GetStatusMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var statusRepo = new GenericRepository<QingMaster_Status_TB>(unitOfWork.Transaction);
                var statusALL = await statusRepo.GetAllAsync();

                return statusALL
                    .Where(x => x.ISACTIVE == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DESCRIPTION
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetSTruckCateMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var TruckCateRepo = new GenericRepository<QingMaster_TruckCategory_TB>(unitOfWork.Transaction);
                var TruckCateALL = await TruckCateRepo.GetAllAsync();

                return TruckCateALL
                    .Where(x => x.ISACTIVE == 1 && x.ISOVS == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DESCRIPTION
                    })
                    .ToList();
            }
        }
    }
}
