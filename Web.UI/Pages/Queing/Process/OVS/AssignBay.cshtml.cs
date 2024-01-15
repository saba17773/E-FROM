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
    public class AssignBayModel : PageModel
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
        public int Bay { get; set; }
        public List<SelectListItem> BayMaster { get; set; }
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
        public string ContainerNo { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public AssignBayModel(
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

                BayMaster = await GetBayMaster(plant);
            }
        }
        public async Task<JsonResult> OnPostGridAsync(string plant)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        WORKORDERNO = "WORKORDERNO",
                        TRUCKID = "TRUCKID",
                        DRIVERNAME = "DRIVERNAME",
                        ROUTEDESC = "ROUTEDESC",
                        STATUSDETAIL = "STATUSDETAIL"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<QingOVSGridModel>(@"
                      SELECT *
                        FROM
                        (
	                       SELECT Q.ID
	                            ,Q.NO
	                            ,Q.WORKORDERNO
	                            ,Q.PLANT
	                            ,Q.AGENTCODE
	                            ,Q.BOOKINGNUMBER
	                            ,Q.TRUCKID
	                            ,Q.DRIVERID
	                            ,Q.DRIVERNAME
	                            ,Q.TRUCKCATEID
	                            ,Q.DRIVERTEL
	                            ,Q.INVOICENO
	                            ,Q.SEALNO
	                            ,Q.CONTAINERSIZEID
	                            ,Q.CONTAINERNO
	                            ,Q.ROUTEID
	                            ,Q.BAYID
	                            ,Q.STATUS
	                            ,Q.CHECKINBY
	                            ,CONVERT(NVARCHAR,Q.CHECKINDATE,103) CHECKINDATE
	                            ,S.DESCRIPTION STATUSDETAIL
	                            ,R.ROUTE ROUTEDESC
	                            ,T.DESCRIPTION TRUCKCATEDESC
                            FROM TB_QingOVS Q JOIN
                            TB_QingMaster_Status S ON Q.STATUS = S.ID JOIN
                            TB_QingMaster_Route R ON Q.ROUTEID = R.ID JOIN
                            TB_QingMaster_TruckCategory T ON Q.TRUCKCATEID = T.ID
                            WHERE Q.STATUS = 1 AND Q.PLANT = @plant
                        )T
                        WHERE " + filter + @"
                    ",
                    new
                    {
                        @plant = plant
                    }
                    , unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task<List<SelectListItem>> GetBayMaster(string plant)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var BayALL = await unitOfWork.Transaction.Connection.QueryAsync<QingMaster_Bay_TB>(@"
                        SELECT * 
                        FROM TB_QingMaster_Bay 
                        WHERE ISACTIVE = 1 AND ISOVS = 1 AND PLANT = @plant AND 
                              ID NOT IN (
                                SELECT BAYID 
                                FROM TB_QingOVS 
                                WHERE CHECKOUTDATE IS NULL AND BAYID <> 0
                              )
                        ", new { @plant = plant }, unitOfWork.Transaction);

                unitOfWork.Complete();

                return BayALL
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.BAY.ToString(),
                    }).ToList();
            }
        }
        public async Task<IActionResult> OnGetQingOVS(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var QueingOVSRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var QueingOVSByID = await QueingOVSRepo.GetAsync(id);

                    unitOfWork.Complete();
                    return new JsonResult(new { truckid = QueingOVSByID.TRUCKID, containerno = QueingOVSByID.CONTAINERNO } );

                }
            }
            catch (Exception)
            {
                return new JsonResult(false);
            }

        }
        public async Task<JsonResult> OnPostTruckGridAsync(string truckid, string containerno, string plant)
        {
            try
            {
                var BranchCode = "";
                using (var unitOfWork1 = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var CompanyRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork1.Transaction);
                    var CompanyALL = await CompanyRepo.GetAllAsync();
                    var CompanyByplant = CompanyALL.Where(x => x.company == plant).FirstOrDefault();
                    BranchCode = CompanyByplant.Branchcode;

                    unitOfWork1.Complete();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("Apple")))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<TruckWeightGridModel>($@"
                        EXEC Queing_GetWeightAssignBay  @pProcess,@pBranchCode,@pTruckID,@pContainerNo
                    ",
                     new
                     {
                         @pProcess = "OVS",
                         @pBranchCode = BranchCode,
                         @pTruckID = truckid,
                         @pContainerNo = containerno
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
        public async Task<IActionResult> OnPostAsync(string plant)
        {
            if (!ModelState.IsValid)
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.VIEW_QUEING));
                await GetData(plant);

                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AssignBayDate = DateTime.Now;
                    var AssignBayBy = _authService.GetClaim().UserId;

                    var QueingOVSRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                    var QueingOVSById = await QueingOVSRepo.GetAsync(Id);

                    var RouteRepo = new GenericRepository<QingMaster_Route_TB>(unitOfWork.Transaction);
                    var RouteByID = await RouteRepo.GetAsync(QueingOVSById.ROUTEID);

                    var BayRepo = new GenericRepository<QingMaster_Bay_TB>(unitOfWork.Transaction);
                    var BayByID = await BayRepo.GetAsync(Bay);

                    var CompanyMasterRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                    var CompanyALL = await CompanyMasterRepo.GetAllAsync();
                    var CompanyByPlant = CompanyALL.Where(x => x.company == QueingOVSById.PLANT).FirstOrDefault();

                    var VOUCHER_TYPE = QueingOVSById.WORKORDERNO.Substring(0, 4);
                    var VOUCHER_SERIES = QueingOVSById.WORKORDERNO.Substring(4, 4);
                    var VOUCHER_NO = QueingOVSById.WORKORDERNO.Substring(8);

                    var DSG_LOADINGPLANT = CompanyByPlant.DataAreaId;

                    var DriverID = "";
                    var Remark = "";
                    if (QueingOVSById.DRIVERID == "" || QueingOVSById.DRIVERID == null)
                    {
                        DriverID = "";
                        Remark = QueingOVSById.DRIVERNAME;
                    }
                    else
                    {
                        DriverID = QueingOVSById.DRIVERID;
                        Remark = "";
                    }

                    var isSub = QueingOVSById.ISSUBWORKORDERNO;

                    using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
                    {
                        if(isSub == 1)
                        {
                            var checkax = await unitOfWork2.Transaction.Connection.QueryAsync<AX_WORKORDERTABLE>(@"
                                SELECT * 
                                FROM DSG_SUBWORKORDERTABLE 
                                WHERE VOUCHER_TYPE = @VOUCHER_TYPE AND 
                                    VOUCHER_SERIES = @VOUCHER_SERIES AND 
                                    DSG_ID = @VOUCHER_NO 
                            ",
                            new
                            {
                                @VOUCHER_TYPE = VOUCHER_TYPE,
                                @VOUCHER_SERIES = VOUCHER_SERIES,
                                @VOUCHER_NO = VOUCHER_NO
                            }, unitOfWork2.Transaction);


                            if (checkax.FirstOrDefault() == null)
                            {
                                AlertError = "Work Order No not have in ax";
                                return Redirect($"/Queing/Process/OVS/{plant}/AssignBay");
                            }

                            var upax = unitOfWork2.Transaction.Connection.Execute(@"UPDATE DSG_SUBWORKORDERTABLE SET 
                                    TRUCK_ID = @TRUCK_ID,
                                    DRIVER_ID = @DRIVER_ID,
                                    DSG_TIME = CONVERT(INT,86400 * CONVERT(FLOAT,@DSG_TIME-CONVERT(DATE,@DSG_TIME))),
                                    DSG_GROSSWEIGHT_IN = @DSG_GROSSWEIGHT_IN,
                                    DSG_ContainerNo = @DSG_ContainerNo,
                                    DSG_SealNo = @DSG_SealNo
                                WHERE VOUCHER_TYPE = @VOUCHER_TYPE AND 
                                VOUCHER_SERIES = @VOUCHER_SERIES AND 
                                DSG_ID = @VOUCHER_NO  ",
                                new
                                {
                                    @TRUCK_ID = QueingOVSById.TRUCKID,
                                    @DRIVER_ID = DriverID,
                                    @DSG_TIME = Convert.ToDateTime(WeightINDate),
                                    @DSG_GROSSWEIGHT_IN = WeightIN,
                                    @DSG_ContainerNo = QueingOVSById.CONTAINERNO,
                                    @DSG_SealNo = QueingOVSById.SEALNO,
                                    @VOUCHER_TYPE = VOUCHER_TYPE,
                                    @VOUCHER_SERIES = VOUCHER_SERIES,
                                    @VOUCHER_NO = VOUCHER_NO
                                },
                                unitOfWork2.Transaction
                            );
                            unitOfWork2.Complete();

                        }
                        else
                        {
                            var checkax = await unitOfWork2.Transaction.Connection.QueryAsync<AX_WORKORDERTABLE>(@"
                                SELECT * 
                                FROM DSG_WORKORDERTABLE 
                                WHERE VOUCHER_TYPE = @VOUCHER_TYPE AND 
                                    VOUCHER_SERIES = @VOUCHER_SERIES AND 
                                    VOUCHER_NO = @VOUCHER_NO AND
                                    DSG_LOADINGPLANT = @DSG_LOADINGPLANT
                            ",
                            new
                            {
                                @VOUCHER_TYPE = VOUCHER_TYPE,
                                @VOUCHER_SERIES = VOUCHER_SERIES,
                                @VOUCHER_NO = VOUCHER_NO,
                                @DSG_LOADINGPLANT = DSG_LOADINGPLANT
                            }, unitOfWork2.Transaction);


                            if (checkax.FirstOrDefault() == null)
                            {
                                AlertError = "Work Order No not have in ax";
                                return Redirect($"/Queing/Process/OVS/{plant}/AssignBay");
                            }

                            var upax = unitOfWork2.Transaction.Connection.Execute(@"UPDATE DSG_WORKORDERTABLE SET 
                                TRUCK_ID = @TRUCK_ID,
                                DRIVER_ID = @DRIVER_ID,
                                DSG_TIME = CONVERT(INT,86400 * CONVERT(FLOAT,@DSG_TIME-CONVERT(DATE,@DSG_TIME))),
                                DSG_GROSSWEIGHT_IN = @DSG_GROSSWEIGHT_IN,
                                DSG_ContainerNo = @DSG_ContainerNo,
                                DSG_SealNo = @DSG_SealNo
                            WHERE VOUCHER_TYPE = @VOUCHER_TYPE AND 
                            VOUCHER_SERIES = @VOUCHER_SERIES AND 
                            VOUCHER_NO = @VOUCHER_NO AND
                            DSG_LOADINGPLANT = @DSG_LOADINGPLANT ",
                                new
                                {
                                    @TRUCK_ID = QueingOVSById.TRUCKID,
                                    @DRIVER_ID = DriverID,
                                    @DSG_TIME = Convert.ToDateTime(WeightINDate),
                                    @DSG_GROSSWEIGHT_IN = WeightIN,
                                    @DSG_ContainerNo = QueingOVSById.CONTAINERNO,
                                    @DSG_SealNo = QueingOVSById.SEALNO,
                                    @VOUCHER_TYPE = VOUCHER_TYPE,
                                    @VOUCHER_SERIES = VOUCHER_SERIES,
                                    @VOUCHER_NO = VOUCHER_NO,
                                    @DSG_LOADINGPLANT = DSG_LOADINGPLANT
                                },
                                unitOfWork2.Transaction
                            );
                            unitOfWork2.Complete();
                        }
                        
                    }

                    if (CompanyByPlant.IsWMS == 1)
                    {
                        using (var unitOfWork3 = new UnitOfWork(_databaseContext.GetConnection()))
                        {
                            var upwms = unitOfWork3.Transaction.Connection.Execute(@"
                               EXEC Queing_UpdateAssignWMS @pROUTE,@pDoor,@pSealnumber,
                                                     @pAppointmenttime,@pTrailerid,
                                                     @pDrivername,@pLOADUSR1,@pLoadid

                            ",
                                new
                                {
                                    @pROUTE = RouteByID.ROUTE,
                                    @pDoor = BayByID.BAY,
                                    @pSealnumber = QueingOVSById.SEALNO,
                                    @pAppointmenttime = Convert.ToDateTime(WeightINDate),
                                    @pTrailerid = QueingOVSById.TRUCKID,
                                    @pDrivername = QueingOVSById.DRIVERNAME,
                                    @pLOADUSR1 = QueingOVSById.CONTAINERNO,
                                    @pLoadid = QueingOVSById.LOADID
                                },
                                unitOfWork3.Transaction
                            );
                            unitOfWork3.Complete();
                        }
                    }

                    QueingOVSById.BAYID = Bay;
                    QueingOVSById.WEIGHID = WeighId;
                    QueingOVSById.WEIGHTIN = WeightIN;
                    QueingOVSById.WEIGHTINDATE = Convert.ToDateTime(WeightINDate);
                    QueingOVSById.STATUS = QueingStatusModel.LoadComplete;
                    QueingOVSById.ASSIGNBAYBY = AssignBayBy;
                    QueingOVSById.ASSIGNBAYDATE = AssignBayDate;

                    await QueingOVSRepo.UpdateAsync(QueingOVSById);

                    unitOfWork.Complete();

                    AlertSuccess = "ASSIGN BAY SUCCESS";
                    return Redirect($"/Queing/Process/OVS/{plant}/AssignBay");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/OVS/{plant}/AssignBay");
            }
        }
    }
}
