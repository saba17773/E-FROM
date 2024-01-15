using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.Queing;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Process.OVS
{
    public class CancelCheckinModel : PageModel
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
        public string WorkOrderNumber1 { get; set; }
        [BindProperty]
        public string WorkOrderNumber2 { get; set; }
        [BindProperty]
        public string WorkOrderNumber3 { get; set; }
        [BindProperty]
        public string TruckId { get; set; }
        [BindProperty]
        public string DriverId { get; set; }
        [BindProperty]
        public string DriverName { get; set; }
        [BindProperty]
        public string DriverTel { get; set; }
        [BindProperty]
        public int CateTruck { get; set; }
        public List<SelectListItem> CateTruckMaster { get; set; }
        [BindProperty]
        public int RouteId { get; set; }
        public List<SelectListItem> RouteMaster { get; set; }
        [BindProperty]
        public string Agent { get; set; }
        public List<SelectListItem> AgentMaster { get; set; }
        [BindProperty]
        public string BookingNumber { get; set; }
        [BindProperty]
        public string InvoiceNo { get; set; }
        [BindProperty]
        public string SealNo { get; set; }
        [BindProperty]
        public int ContainerSizeId { get; set; }
        public List<SelectListItem> ContainerSizeMaster { get; set; }
        [BindProperty]
        public string ContainerNo { get; set; }
        [BindProperty]
        public string PageBack { get; set; }
        [BindProperty]
        public string PageBackTitle { get; set; }
        [BindProperty]
        public string Loadid { get; set; }
        [BindProperty]
        public string Remark { get; set; }
        [BindProperty]
        public int IsOverAll { get; set; }
        [BindProperty]
        public string Bay { get; set; }
        [BindProperty]
        public string WeightIn { get; set; }
        [BindProperty]
        public string WeightInDate { get; set; }
        [BindProperty]
        public string WeightOut { get; set; }
        [BindProperty]
        public string WeightOutDate { get; set; }
        [BindProperty]
        public int Status { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public CancelCheckinModel(
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
        public async Task<IActionResult> OnGetAsync(string plant, int id)
        {
            try
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.CANCEL_QUEING));

                await GetData(plant, id);

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/OVS/{plant}/CheckinList");
            }
        }
        public async Task GetData(string plant, int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                Plant = plant;
                PageBack = "CheckinList";
                PageBackTitle = "List";

                var PlantRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                var PlantALL = await PlantRepo.GetAllAsync();
                var PlantbyPlantCode = PlantALL.Where(x => x.company == plant).FirstOrDefault();

                PlantView = PlantbyPlantCode.FullName_EN;

                CateTruckMaster = await GetTruckCategory();
                RouteMaster = await GetRoute();
                AgentMaster = await GetAgent();
                ContainerSizeMaster = await GetContainerSize();

                var QueingOVSRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                var QueingOVSByID = await QueingOVSRepo.GetAsync(id);

                WorkOrderNumber1 = QueingOVSByID.WORKORDERNO.Substring(0, 4);
                WorkOrderNumber2 = QueingOVSByID.WORKORDERNO.Substring(4, 4);
                WorkOrderNumber3 = QueingOVSByID.WORKORDERNO.Substring(8);

                Agent = QueingOVSByID.AGENTCODE;
                BookingNumber = QueingOVSByID.BOOKINGNUMBER;
                InvoiceNo = QueingOVSByID.INVOICENO;
                SealNo = QueingOVSByID.SEALNO;
                CateTruck = QueingOVSByID.TRUCKCATEID;
                TruckId = QueingOVSByID.TRUCKID;
                DriverId = QueingOVSByID.DRIVERID;
                DriverName = QueingOVSByID.DRIVERNAME;
                DriverTel = QueingOVSByID.DRIVERTEL;
                ContainerSizeId = QueingOVSByID.CONTAINERSIZEID;
                ContainerNo = QueingOVSByID.CONTAINERNO;
                RouteId = QueingOVSByID.ROUTEID;
                Loadid = QueingOVSByID.LOADID;
                Remark = QueingOVSByID.REMARK;

                Status = QueingOVSByID.STATUS;

            }
        }
        private async Task<List<SelectListItem>> GetTruckCategory()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var TruckCateRepo = new GenericRepository<QingMaster_TruckCategory_TB>(unitOfWork.Transaction);
                var TruckCateALL = await TruckCateRepo.GetAllAsync();

                unitOfWork.Complete();

                return TruckCateALL
                    .Where(x => x.ISOVS == 1 && x.ISACTIVE == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DESCRIPTION.ToString(),
                    }).ToList();
            }
        }
        private async Task<List<SelectListItem>> GetRoute()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var RouteRepo = new GenericRepository<QingMaster_Route_TB>(unitOfWork.Transaction);
                var RouteALL = await RouteRepo.GetAllAsync();

                unitOfWork.Complete();

                return RouteALL
                    .Where(x => x.ISACTIVE == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.ROUTE.ToString(),
                    }).ToList();
            }
        }
        private async Task<List<SelectListItem>> GetAgent()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var AgentRepo = new GenericRepository<QingMaster_AgentTractor_TB>(unitOfWork.Transaction);
                var AgentALL = await AgentRepo.GetAllAsync();

                unitOfWork.Complete();

                return AgentALL
                    .Where(x => x.ISACTIVE == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.AGENTCODE.ToString(),
                        Text = x.AGENTCODE.ToString(),
                    }).ToList();
            }
        }
        private async Task<List<SelectListItem>> GetContainerSize()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var ContainerSizeRepo = new GenericRepository<QingMaster_ContainerSize_TB>(unitOfWork.Transaction);
                var ContainerSizeALL = await ContainerSizeRepo.GetAllAsync();

                unitOfWork.Complete();

                return ContainerSizeALL
                    .Where(x => x.ISACTIVE == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.SIZEDESCRIPTION.ToString(),
                    }).ToList();
            }
        }
        public async Task<IActionResult> OnPostAsync(string plant, int id)
        {
            if (!ModelState.IsValid)
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.CANCEL_QUEING));

                await GetData(plant, id);

                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var CancelDate = DateTime.Now;
                    var CancelBy = _authService.GetClaim().UserId;

                    var QueingOVSRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var QueingOVSById = await QueingOVSRepo.GetAsync(id);

                    QueingOVSById.STATUS = QueingStatusModel.Cancel;
                    QueingOVSById.CANCELBY = CancelBy;
                    QueingOVSById.CANCELDATE = CancelDate;

                    await QueingOVSRepo.UpdateAsync(QueingOVSById);

                    unitOfWork.Complete();

                    AlertSuccess = "CANCEL SUCCESS";
                    return Redirect($"/Queing/Process/OVS/{plant}/CheckinList");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/OVS/{plant}/Checkin");
            }
        }
    }
}
