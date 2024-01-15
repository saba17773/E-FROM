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
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Process.OVS
{
    public class ViewCheckinModel : PageModel
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

        public ViewCheckinModel(
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
        public async Task<IActionResult> OnGetAsync(string plant, int id, int isoverall)
        {
            try
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.VIEW_QUEING));

                await GetData(plant, id, isoverall);

                return Page();
            }
            catch (Exception ex)
            {
                if (isoverall == 1)
                {
                    AlertError = ex.Message;
                    return Redirect($"/Queing/Process/OVS/{plant}/Transaction");
                }
                else
                {
                    AlertError = ex.Message;
                    return Redirect($"/Queing/Process/OVS/{plant}/CheckinList");
                }
            }
        }
        public async Task GetData(string plant, int id, int isoverall)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                Plant = plant;
                IsOverAll = isoverall;
                if (isoverall == 1)
                {
                    PageBack = "Transaction";
                    PageBackTitle = "Transaction";
                }
                else
                {

                    PageBack = "CheckinList";
                    PageBackTitle = "List";
                }

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

                if (isoverall == 1 && (Status == 2 || Status == 3))
                {
                    var BayRepo = new GenericRepository<QingMaster_Bay_TB>(unitOfWork.Transaction);
                    var BayByID = await BayRepo.GetAsync(QueingOVSByID.BAYID);
                    Bay = BayByID.BAY;
                    WeightIn = QueingOVSByID.WEIGHTIN.ToString();
                    WeightInDate = Convert.ToDateTime(QueingOVSByID.WEIGHTINDATE).ToString("dd/MM/yyyy HH:mm:ss");
                }

                if (isoverall == 1 && Status == 3)
                {
                    WeightOut = QueingOVSByID.WEIGHTOUT.ToString();
                    WeightOutDate = Convert.ToDateTime(QueingOVSByID.WEIGHTOUTDATE).ToString("dd/MM/yyyy HH:mm:ss");
                }
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
    }
}
