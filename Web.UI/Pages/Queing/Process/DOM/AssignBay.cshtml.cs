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
                return Redirect($"/Queing/Process/DOM/{plant}/Index");
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
                        ROUTE = "ROUTE",
                        STATUSDETAIL = "STATUSDETAIL"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<QingDOMGridModel>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT Q.ID
	                            ,Q.NO
	                            ,Q.WORKORDERNO
	                            ,Q.PLANT
	                            ,Q.TRUCKID
	                            ,Q.DRIVERID
	                            ,Q.DRIVERNAME
	                            ,Q.TRUCKCATEID
	                            ,Q.STDTIME
	                            ,Q.DRIVERTEL
	                            ,Q.PROVINCESID
	                            ,Q.STATUS
	                            ,Q.CHECKINBY
	                            ,CONVERT(NVARCHAR,Q.CHECKINDATE,103) CHECKINDATE
	                            ,S.DESCRIPTION STATUSDETAIL
	                            ,P.NameInThai PROVINCESNAME
	                            ,T.DESCRIPTION TRUCKCATEDESC
                            FROM TB_QingDOM Q JOIN
                            TB_QingMaster_Status S ON Q.STATUS = S.ID JOIN
                            TB_Provinces P ON Q.PROVINCESID = P.Id JOIN
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
                        WHERE ISACTIVE = 1 AND ISDOM = 1 AND PLANT = @plant AND 
                            ID NOT IN (SELECT BAYID 
                            FROM TB_QingDOM 
                            WHERE CHECKOUTDATE IS NULL 
                            AND BAYID <> 0)
                        ", new { @plant = plant },unitOfWork.Transaction);

                unitOfWork.Complete();

                return BayALL
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.BAY.ToString(),
                    }).ToList();
            }
        }
        public async Task<JsonResult> OnPostTruckGridAsync(string truckid, string plant,int isFristLoad, string CheckINDate)
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
                    if (isFristLoad == 1)
                    {
                        var data = await unitOfWork.Transaction.Connection.QueryAsync<TruckWeightGridModel>($@"
                        EXEC Queing_GetWeightAssignBay  @pProcess,@pBranchCode,@pTruckID",
                           new
                           {
                               @pProcess = "DOM",
                               @pBranchCode = BranchCode,
                               @pTruckID = truckid
                           }, unitOfWork.Transaction);
                        unitOfWork.Complete();

                        return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                    }
                    else
                    {
                        var data = await unitOfWork.Transaction.Connection.QueryAsync<TruckWeightGridModel>($@"
                        EXEC Queing_GetWeightAssignBay  @pProcess,@pBranchCode,@pTruckID,NULL,@pCheckinDate",
                           new
                           {
                               @pProcess = "DOM",
                               @pBranchCode = BranchCode,
                               @pTruckID = truckid,
                               @pCheckinDate = CheckINDate
                           }, unitOfWork.Transaction);
                        unitOfWork.Complete();

                        return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                    }
                  
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

                    var QueingDOMRepo = new GenericRepository<QingDOM_TB>(unitOfWork.Transaction);
                    var QueingDOMById = await QueingDOMRepo.GetAsync(Id);

                    var ProvinceRepo = new GenericRepository<ProvinceTable>(unitOfWork.Transaction);
                    var ProvinceByID = await ProvinceRepo.GetAsync(QueingDOMById.PROVINCESID);

                    var BayRepo = new GenericRepository<QingMaster_Bay_TB>(unitOfWork.Transaction);
                    var BayByID = await BayRepo.GetAsync(Bay);

                    var CompanyMasterRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                    var CompanyALL = await CompanyMasterRepo.GetAllAsync();
                    var CompanyByPlant = CompanyALL.Where(x => x.company == QueingDOMById.PLANT).FirstOrDefault();

                    var AgentRepo = new GenericRepository<QingMaster_Agent_TB>(unitOfWork.Transaction);
                    var AgentByID = await AgentRepo.GetAsync(QueingDOMById.AGENTID);

                    var VOUCHER_TYPE = QueingDOMById.WORKORDERNO.Substring(0, 4);
                    var VOUCHER_SERIES = QueingDOMById.WORKORDERNO.Substring(4, 4);
                    var VOUCHER_NO = QueingDOMById.WORKORDERNO.Substring(8);
                    var DSG_LOADINGPLANT = CompanyByPlant.DataAreaId;
                    var REMARKS = "";

                    if (QueingDOMById.AGENTID == 0)
                    {
                        REMARKS = QueingDOMById.REMARK;
                    }
                    else
                    {
                        if (QueingDOMById.REMARK != "" || QueingDOMById.REMARK != null)
                        {
                            REMARKS = QueingDOMById.DRIVERNAME + " " + QueingDOMById.DRIVERTEL + " " + AgentByID.AGENTNAME + "   " + QueingDOMById.REMARK; 
                        }
                        else
                        {
                            REMARKS = QueingDOMById.DRIVERNAME + " " + QueingDOMById.DRIVERTEL + " " + AgentByID.AGENTNAME;
                        }
                        
                    }

                    var DriverID = "";
                    if (QueingDOMById.DRIVERID == "" || QueingDOMById.DRIVERID == null)
                    {
                        DriverID = "";
                    }
                    else
                    {
                        DriverID = QueingDOMById.DRIVERID;
                    }

                    using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
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
                            return Redirect($"/Queing/Process/DOM/{plant}/AssignBay");
                        }

                        var upax = unitOfWork2.Transaction.Connection.Execute(@"UPDATE DSG_WORKORDERTABLE SET 
                                TRUCK_ID = @TRUCK_ID,
                                DRIVER_ID = @DRIVER_ID,
                                DSG_TIME = CONVERT(INT,86400 * CONVERT(FLOAT,@DSG_TIME-CONVERT(DATE,@DSG_TIME))),
                                DSG_GROSSWEIGHT_IN = @DSG_GROSSWEIGHT_IN
                            WHERE VOUCHER_TYPE = @VOUCHER_TYPE AND 
                            VOUCHER_SERIES = @VOUCHER_SERIES AND 
                            VOUCHER_NO = @VOUCHER_NO AND
                            DSG_LOADINGPLANT = @DSG_LOADINGPLANT",
                            new
                            {
                                @TRUCK_ID = QueingDOMById.TRUCKID,
                                @DRIVER_ID = DriverID,
                                @DSG_TIME = Convert.ToDateTime(WeightINDate),
                                @DSG_GROSSWEIGHT_IN = WeightIN,
                                @VOUCHER_TYPE = VOUCHER_TYPE,
                                @VOUCHER_SERIES = VOUCHER_SERIES,
                                @VOUCHER_NO = VOUCHER_NO,
                                @DSG_LOADINGPLANT = DSG_LOADINGPLANT
                            },
                            unitOfWork2.Transaction
                        );
                        unitOfWork2.Complete();
                    }

                    if (CompanyByPlant.IsWMS == 1)
                    {
                        using (var unitOfWork3 = new UnitOfWork(_databaseContext.GetConnection()))
                        {
                            var upwms = unitOfWork3.Transaction.Connection.Execute(@"
                               EXEC Queing_UpdateAssignWMS @pROUTE,@pDoor,NULL,
                                                     @pAppointmenttime,@pTrailerid,
                                                     @pDrivername,NULL,@pLoadid

                            ",
                                new
                                {
                                    @pROUTE = ProvinceByID.NameInThai,
                                    @pDoor = BayByID.BAY,
                                    @pAppointmenttime = Convert.ToDateTime(WeightINDate),
                                    @pTrailerid = QueingDOMById.TRUCKID,
                                    @pDrivername = QueingDOMById.DRIVERNAME,
                                    @pLoadid = QueingDOMById.LOADID
                                },
                                unitOfWork3.Transaction
                            );
                            unitOfWork3.Complete();
                        }
                    }

                    QueingDOMById.BAYID = Bay;
                    QueingDOMById.WEIGHID = WeighId;
                    QueingDOMById.WEIGHTIN = WeightIN;
                    QueingDOMById.WEIGHTINDATE = Convert.ToDateTime(WeightINDate);
                    QueingDOMById.STATUS = QueingStatusModel.LoadComplete;
                    QueingDOMById.ASSIGNBAYBY = AssignBayBy;
                    QueingDOMById.ASSIGNBAYDATE = AssignBayDate;

                    await QueingDOMRepo.UpdateAsync(QueingDOMById);
                   


                    unitOfWork.Complete();

                    AlertSuccess = "ASSIGN BAY SUCCESS";
                    return Redirect($"/Queing/Process/DOM/{plant}/AssignBay");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/DOM/{plant}/AssignBay");
            }
        }
    }
}
