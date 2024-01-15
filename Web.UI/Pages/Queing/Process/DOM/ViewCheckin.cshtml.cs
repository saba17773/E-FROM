using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Web.UI.Pages.Queing.Process.DOM
{
    public class ViewCheckinModel : PageModel
    {
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
        public string TruckDesc { get; set; }
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
        public int StdTime { get; set; }
        [BindProperty]
        public string ProvincesName { get; set; }
        [BindProperty]
        public string PageBack { get; set; }
        [BindProperty]
        public string PageBackTitle { get; set; }
        [BindProperty]
        public string Loadid { get; set; }
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
                    return Redirect($"/Queing/Process/DOM/{plant}/Transaction");
                }
                else
                {
                    AlertError = ex.Message;
                    return Redirect($"/Queing/Process/DOM/{plant}/CheckinList");
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

                var QueingDOMRepo = new GenericRepository<QingDOM_TB>(unitOfWork.Transaction);
                var QueingByID = await QueingDOMRepo.GetAsync(id);

                var ProvinceRepo = new GenericRepository<ProvinceTable>(unitOfWork.Transaction);
                var ProvinceByID = await ProvinceRepo.GetAsync(QueingByID.PROVINCESID);

                WorkOrderNumber1 = QueingByID.WORKORDERNO.Substring(0, 4);
                WorkOrderNumber2 = QueingByID.WORKORDERNO.Substring(4, 4);
                WorkOrderNumber3 = QueingByID.WORKORDERNO.Substring(8);

                TruckId = QueingByID.TRUCKID;
                TruckDesc = QueingByID.TRUCKDESC;
                DriverId = QueingByID.DRIVERID;
                DriverName = QueingByID.DRIVERNAME;
                DriverTel = QueingByID.DRIVERTEL;
                CateTruck = QueingByID.TRUCKCATEID;
                StdTime = QueingByID.STDTIME;
                ProvincesName = ProvinceByID.NameInThai;
                Loadid = QueingByID.LOADID;
                Status = QueingByID.STATUS;

                if (isoverall == 1 && Status >= 2)
                {
                    var BayRepo = new GenericRepository<QingMaster_Bay_TB>(unitOfWork.Transaction);
                    var BayByID = await BayRepo.GetAsync(QueingByID.BAYID);
                    Bay = BayByID.BAY;
                    WeightIn = QueingByID.WEIGHTIN.ToString();
                    WeightInDate = Convert.ToDateTime(QueingByID.WEIGHTINDATE).ToString("dd/MM/yyyy HH:mm:ss");
                }

                if (isoverall == 1 && Status == 3)
                {
                    WeightOut = QueingByID.WEIGHTOUT.ToString();
                    WeightOutDate = Convert.ToDateTime(QueingByID.WEIGHTOUTDATE).ToString("dd/MM/yyyy HH:mm:ss");
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
                    .Where(x => x.ISDOM == 1 && x.ISACTIVE == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DESCRIPTION.ToString(),
                    }).ToList();
            }
        }
        
    }
}
