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

namespace Web.UI.Pages.Queing.Process.DOM
{
    public class EditCheckinModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public int Id { get; set; }
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
        public int ProvincesId { get; set; }
        [BindProperty]
        public string ProvincesName { get; set; }
        [BindProperty]
        public string Loadid { get; set; }
        [BindProperty]
        public string PickingListID { get; set; }
        [BindProperty]
        public string Remark { get; set; }
        [BindProperty]
        public int TranspotCateId { get; set; }
        public List<SelectListItem> TranspotCateMaster { get; set; }
        [BindProperty]
        public int Agent { get; set; }
        public List<SelectListItem> AgentMaster { get; set; }
        public int isAgent { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public EditCheckinModel(
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
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var QingDOMRepo = new GenericRepository<QingDOM_TB>(unitOfWork.Transaction);
                    var QingDOMByID = await QingDOMRepo.GetAsync(id);

                    if (QingDOMByID.STATUS != 1)
                    {
                        unitOfWork.Complete();
                        AlertError = "This Order Assign Bay Already";
                        return Redirect($"/Queing/Process/DOM/{plant}/CheckinList");
                    }

                    await _authService.CanAccess(nameof(QueingPermissionModel.EDIT_QUEING));
                    await GetData(plant,id);
                    return Page();

                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/DOM/{plant}/CheckinList");
            }
        }
        public async Task GetData(string plant, int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                Plant = plant;
                Id = id;

                var PlantRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                var PlantALL = await PlantRepo.GetAllAsync();
                var PlantbyPlantCode = PlantALL.Where(x => x.company == plant).FirstOrDefault();

                PlantView = PlantbyPlantCode.FullName_EN;

                CateTruckMaster = await GetTruckCategory();
                TranspotCateMaster = await GetTranspotCate();
                AgentMaster = await GetAgent(plant);

                var DOMDetailRepo = new GenericRepository<QingDOM_TB>(unitOfWork.Transaction);
                var DOMDetailByID = await DOMDetailRepo.GetAsync(id);

                var ProvinceRepo = new GenericRepository<ProvinceTable>(unitOfWork.Transaction);
                var ProvinceByID = await ProvinceRepo.GetAsync(DOMDetailByID.PROVINCESID);

                var TranspotCateRepo = new GenericRepository<QingMaster_TranspotCategory_TB>(unitOfWork.Transaction);
                var TranspotCateByID = await TranspotCateRepo.GetAsync(DOMDetailByID.TRANSPOTCATEID);

                isAgent = TranspotCateByID.ISAGENT;

                WorkOrderNumber2 = DOMDetailByID.WORKORDERNO.Substring(4, 4);
                WorkOrderNumber3 = DOMDetailByID.WORKORDERNO.Substring(8);
                Loadid = DOMDetailByID.LOADID;
                TranspotCateId = DOMDetailByID.TRANSPOTCATEID;
                TruckId = DOMDetailByID.TRUCKID;
                TruckDesc = DOMDetailByID.TRUCKDESC;
                DriverId = DOMDetailByID.DRIVERID;
                DriverName = DOMDetailByID.DRIVERNAME;
                DriverTel = DOMDetailByID.DRIVERTEL;
                CateTruck = DOMDetailByID.TRUCKCATEID;
                StdTime = DOMDetailByID.STDTIME;
                Agent = DOMDetailByID.AGENTID;
                ProvincesName = ProvinceByID.NameInThai;
                ProvincesId = DOMDetailByID.PROVINCESID;
                Remark = DOMDetailByID.REMARK;

                unitOfWork.Complete();

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
                    .Where(x => x.ISDOM == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DESCRIPTION.ToString(),
                    }).ToList();
            }
        }
        private async Task<List<SelectListItem>> GetTranspotCate()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var TranspotCateRepo = new GenericRepository<QingMaster_TranspotCategory_TB>(unitOfWork.Transaction);
                var TranspotCateALL = await TranspotCateRepo.GetAllAsync();

                unitOfWork.Complete();

                return TranspotCateALL
                    .Where(x => x.ISACTIVE == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DESCRIPTION.ToString(),
                    }).ToList();
            }
        }
        public async Task<JsonResult> OnPostTruckGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
                {
                    var field = new
                    {
                        TRUCKID = "TRUCKID",
                        TRUCKDESC = "TRUCKDESC"
                    };

                    var filter = _datatableService.Filter(Request, field);

                    var Truck = await unitOfWork.Transaction.Connection.QueryAsync<TruckGridModel>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT TRUCK_ID TRUCKID
                                ,TUCK_DESC TRUCKDESC 
                            FROM DSG_TRUCK WHERE DATAAREAID = 'DV'
                        )T
                        WHERE " + filter + @" 
                        ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, Truck.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<JsonResult> OnPostDriverGridAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
                {
                    var field = new
                    {
                        DRIVERID = "DRIVERID",
                        FULLNAME = "FULLNAME",
                        PHONE = "PHONE"
                    };

                    var filter = _datatableService.Filter(Request, field);

                    var Truck = await unitOfWork.Transaction.Connection.QueryAsync<DriverGridModel>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT EMPLID DRIVERID,
                                    TITLE+ ' ' +NAME + ' ' +ALIAS AS FULLNAME,
                                    PHONE
                            FROM EMPLTABLE
                            WHERE  DATAAREAID = 'DV' AND  STATUS = 1
                            AND LEFT(EMPLID, 3) = @plant
                        )T
                        WHERE " + filter + @" 
                        ",
                        new
                        {
                            @plant = plant
                        }, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, Truck.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        private async Task<List<SelectListItem>> GetAgent(string plant)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var AgentRepo = new GenericRepository<QingMaster_Agent_TB>(unitOfWork.Transaction);
                var AgentALL = await AgentRepo.GetAllAsync();

                unitOfWork.Complete();

                return AgentALL
                    .Where(x => x.ISACTIVE == 1 && x.COMPANY == plant)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.AGENTNAME.ToString(),
                    }).ToList();
            }
        }
        public async Task<IActionResult> OnGetSelectTransportCate(int AgentId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var TranspotRepo = new GenericRepository<QingMaster_TranspotCategory_TB>(unitOfWork.Transaction);
                    var TranspotByID = await TranspotRepo.GetAsync(AgentId);

                    var isAgent = TranspotByID.ISAGENT;
                    bool data;
                    if (isAgent == 0)
                    {
                        data = false;
                    }
                    else
                    {
                        data = true;
                    }

                    unitOfWork.Complete();

                    return new JsonResult(data);
                }
            }
            catch (Exception)
            {
                return new JsonResult(0);
            }

        }
        public async Task<JsonResult> OnPostLoadidGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<LoadidGridModel>($@"
                        EXEC Queing_GetLoadIDWMS
                    ", null, unitOfWork.Transaction);
                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<JsonResult> OnPostProvincesGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        PROVINCESID = "PROVINCESID",
                        PROVINCESNAME = "PROVINCESNAME"
                    };

                    var filter = _datatableService.Filter(Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<ProvincesGridModel>(@"
                      SELECT *
                        FROM
                        (
	                       SELECT Id PROVINCESID
                              ,NameInThai PROVINCESNAME
                           FROM TB_Provinces
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
        public async Task<IActionResult> OnGetSelectCate(int Cateid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var TruckCateRepo = new GenericRepository<QingMaster_TruckCategory_TB>(unitOfWork.Transaction);
                    var TruckCateByID = await TruckCateRepo.GetAsync(Cateid);

                    var StdTime = TruckCateByID.STDTIME;

                    unitOfWork.Complete();

                    return new JsonResult(StdTime);
                }
            }
            catch (Exception)
            {
                return new JsonResult(0);
            }

        }
        public async Task<JsonResult> OnPostPickingListGridAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("STR")))
                {
                    var field = new
                    {
                        ORDERID = "ORDERID",
                        PICKINGLISTID = "PICKINGLISTID",
                        PICKINGLISTDATE = "PICKINGLISTDATE"
                    };

                    var filter = _datatableService.Filter(Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<PickingListGridModel>(@"
                      SELECT *
                        FROM
                        (
	                       SELECT ORDERID,
                            PICKINGLISTID,
                            PICKINGLISTDATE
                            FROM LoadingTable
                            WHERE STATUS = 1 AND Company = @plant
                        )T
                        WHERE " + filter + @" 
                        ",
                        new
                        {
                            @plant = plant
                        }, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<IActionResult> OnPostAsync(string plant, int id)
        {
            if (!ModelState.IsValid)
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.EDIT_QUEING));
                await GetData(plant, id);

                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var CreateDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;

                    var QingDOMRepo = new GenericRepository<QingDOM_TB>(unitOfWork.Transaction);
                    var QingDOMByID = await QingDOMRepo.GetAsync(id);

                    if (QingDOMByID.STATUS != 1)
                    {
                        unitOfWork.Complete();
                        AlertError = "This Order Assign Bay Already";
                        return Redirect($"/Queing/Process/DOM/{plant}/CheckinList");
                    }

                    var WorkOrderNo = WorkOrderNumber1 + WorkOrderNumber2 + WorkOrderNumber3;

                    QingDOMByID.WORKORDERNO = WorkOrderNo;
                    QingDOMByID.LOADID = Loadid;
                    QingDOMByID.TRANSPOTCATEID = TranspotCateId;
                    QingDOMByID.TRUCKID = TruckId;
                    QingDOMByID.TRUCKDESC = TruckDesc;
                    QingDOMByID.DRIVERID = DriverId == null ? "" : DriverId;
                    QingDOMByID.DRIVERNAME = DriverName;
                    QingDOMByID.DRIVERTEL = DriverTel;
                    QingDOMByID.TRUCKCATEID = CateTruck;
                    QingDOMByID.STDTIME = StdTime;
                    QingDOMByID.PROVINCESID = ProvincesId;
                    QingDOMByID.AGENTID = Agent;
                    QingDOMByID.REMARK = Remark;
                    QingDOMByID.CHECKINBY = CreateBy;
                    QingDOMByID.CHECKINDATE = CreateDate;
                    
                    await QingDOMRepo.UpdateAsync(QingDOMByID);

                    unitOfWork.Complete();

                    AlertSuccess = "EDIT CHECK IN SUCCESS";
                    return Redirect($"/Queing/Process/DOM/{plant}/CheckinList");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/DOM/{plant}/{id}/EditCheckin");
            }
        }
    }
}
