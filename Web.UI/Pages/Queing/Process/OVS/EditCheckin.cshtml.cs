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
using Web.UI.Infrastructure.Models.Queing;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Process.OVS
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
        public string SubWorkOrderNo { get; set; }
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
                    var QingOVSRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var QingOVSByID = await QingOVSRepo.GetAsync(id);

                    if (QingOVSByID.STATUS != 1)
                    {
                        unitOfWork.Complete();
                        AlertError = "This Order Assign Bay Already";
                        return Redirect($"/Queing/Process/OVS/{plant}/CheckinList");
                    }

                    await _authService.CanAccess(nameof(QueingPermissionModel.EDIT_QUEING));
                    await GetData(plant, id);

                    return Page();

                }

                
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
                Id = id;
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

                var OVSDetailRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                var OVSDetailByID = await OVSDetailRepo.GetAsync(id);

                WorkOrderNumber2 = OVSDetailByID.WORKORDERNO.Substring(4, 4);

                if (OVSDetailByID.ISSUBWORKORDERNO == 1)
                {
                    string[] wonoList = OVSDetailByID.WORKORDERNO.Substring(8).Split("-");
                    WorkOrderNumber3 = wonoList[0];
                    SubWorkOrderNo = wonoList[1];
                }
                else
                {
                    WorkOrderNumber3 = OVSDetailByID.WORKORDERNO.Substring(8);
                }

                Loadid = OVSDetailByID.LOADID;
                Agent = OVSDetailByID.AGENTCODE;
                BookingNumber = OVSDetailByID.BOOKINGNUMBER;
                InvoiceNo = OVSDetailByID.INVOICENO;
                SealNo = OVSDetailByID.SEALNO;
                CateTruck = OVSDetailByID.TRUCKCATEID;
                TruckId = OVSDetailByID.TRUCKID;
                DriverId = OVSDetailByID.DRIVERID;
                DriverName = OVSDetailByID.DRIVERNAME;
                DriverTel = OVSDetailByID.DRIVERTEL;
                ContainerSizeId = OVSDetailByID.CONTAINERSIZEID;
                ContainerNo = OVSDetailByID.CONTAINERNO;
                RouteId = OVSDetailByID.ROUTEID;
                Remark = OVSDetailByID.REMARK;

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
                    .Where(x => x.ISOVS == 1)
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
        public async Task<JsonResult> OnPostSubWONoGridAsync(string txtwono1, string txtwono2, string txtwono3)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<SubWorkOrderNumberGridModel>($@"
                        SELECT VOUCHER_TYPE AS VOUCHERTYPE 
	                          ,VOUCHER_SERIES AS VOUCHERSERIES
	                          ,VOUCHER_NO AS VOUCHERNO
	                          ,DSG_NUM AS DSGNUM
	                          ,DSG_ID AS DSGID
	                          ,REMARKS
                        FROM DSG_SUBWORKORDERTABLE 
                        WHERE VOUCHER_TYPE = @type AND 
                              VOUCHER_SERIES = @series AND 
                              VOUCHER_NO LIKE @no   
                        ORDER BY DSG_NUM ASC
                    ",
                    new
                    {
                        @type = txtwono1,
                        @series = txtwono2,
                        @no = txtwono3
                    }, unitOfWork.Transaction);
                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
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

                    var QingOVSRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var QingOVSByID = await QingOVSRepo.GetAsync(id);

                    if (QingOVSByID.STATUS != 1)
                    {
                        unitOfWork.Complete();
                        AlertError = "This Order Assign Bay Already";
                        return Redirect($"/S2E/Purchase/RMAssessment/Main");
                    }

                    var WorkOrderNo = "";
                    var isSub = 0;
                    if (SubWorkOrderNo == "" || SubWorkOrderNo == null)
                    {
                        WorkOrderNo = WorkOrderNumber1 + WorkOrderNumber2 + WorkOrderNumber3;
                        isSub = 0;
                    }
                    else
                    {
                        WorkOrderNo = WorkOrderNumber1 + WorkOrderNumber2 + WorkOrderNumber3 + "-" + SubWorkOrderNo;
                        isSub = 1;
                    }

                    //var WorkOrderNo = WorkOrderNumber1 + WorkOrderNumber2 + WorkOrderNumber3;

                    QingOVSByID.WORKORDERNO = WorkOrderNo;
                    QingOVSByID.LOADID = Loadid;
                    QingOVSByID.AGENTCODE = Agent;
                    QingOVSByID.BOOKINGNUMBER = BookingNumber;
                    QingOVSByID.TRUCKID = TruckId;
                    QingOVSByID.DRIVERID = DriverId == null ? "" : DriverId;
                    QingOVSByID.DRIVERNAME = DriverName;
                    QingOVSByID.DRIVERTEL = DriverTel;
                    QingOVSByID.TRUCKCATEID = CateTruck;
                    QingOVSByID.INVOICENO = InvoiceNo;
                    QingOVSByID.SEALNO = SealNo;
                    QingOVSByID.CONTAINERSIZEID = ContainerSizeId;
                    QingOVSByID.CONTAINERNO = ContainerNo;
                    QingOVSByID.ROUTEID = RouteId;
                    QingOVSByID.ISSUBWORKORDERNO = isSub;
                    QingOVSByID.REMARK = Remark;
                    QingOVSByID.CHECKINBY = CreateBy;
                    QingOVSByID.CHECKINDATE = CreateDate;

                    await QingOVSRepo.UpdateAsync(QingOVSByID);

                    unitOfWork.Complete();

                    AlertSuccess = "EDIT CHECK IN SUCCESS";
                    return Redirect($"/Queing/Process/OVS/{plant}/CheckinList");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/OVS/{plant}/{id}/EditCheckin");
            }
        }
    }
}
