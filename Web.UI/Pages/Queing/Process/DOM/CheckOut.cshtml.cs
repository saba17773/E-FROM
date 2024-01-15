using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    public class CheckOutModel : PageModel
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
        public int Id { get; set; }
        [BindProperty]
        public string TruckID { get; set; }
        [BindProperty]
        public decimal WeightOut { get; set; }
        [BindProperty]
        public string WeightOutDate { get; set; }
        [BindProperty]
        public string WorkOrderNumber1 { get; set; }
        [BindProperty]
        public string WorkOrderNumber2 { get; set; }
        [BindProperty]
        public string WorkOrderNumber3 { get; set; }
        [BindProperty]
        public int IdChange { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public CheckOutModel(
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
	                            ,Q.WEIGHID
                            FROM TB_QingDOM Q JOIN
                            TB_QingMaster_Status S ON Q.STATUS = S.ID JOIN
                            TB_Provinces P ON Q.PROVINCESID = P.Id JOIN
                            TB_QingMaster_TruckCategory T ON Q.TRUCKCATEID = T.ID
                            WHERE Q.STATUS = 2 AND Q.PLANT = @plant
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
        public async Task<IActionResult> OnGetWeightOutDetail(int id, int weighid, string plant)
        {
            try
            {
                using (var unitOfWork1 = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var QueingDOMRepo = new GenericRepository<QingDOM_TB>(unitOfWork1.Transaction);
                    var QueingDOMByID = await QueingDOMRepo.GetAsync(id);

                    var BranchCode = "";

                    var CompanyRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork1.Transaction);
                    var CompanyALL = await CompanyRepo.GetAllAsync();
                    var CompanyByplant = CompanyALL.Where(x => x.company == plant).FirstOrDefault();
                    BranchCode = CompanyByplant.Branchcode;

                    decimal WeightOut = 0;
                    var DateOut = "";
                    using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("Apple")))
                    {
                        var WeightData = await unitOfWork.Transaction.Connection.QueryAsync<TruckWeightGridModel>(@"
                               EXEC Queing_GetWeightCheckOut  @pBranchCode,@pWeighid
                            ",
                             new
                             {
                                 @pBranchCode = BranchCode,
                                 @pWeighid = weighid
                             },
                             unitOfWork.Transaction);

                        var WeightDataList = WeightData.ToList().FirstOrDefault();

                        WeightOut = WeightDataList.WEIGHTOUT;
                        DateOut = WeightDataList.DATEOUT;

                        unitOfWork.Complete();
                    }

                    unitOfWork1.Complete();
                    return new JsonResult(new
                    {
                        truckid = QueingDOMByID.TRUCKID,
                        weightout = WeightOut,
                        weightoutdate = DateOut
                    });

                }
            }
            catch (Exception)
            {
                return new JsonResult(false);
            }

        }
        public async Task<IActionResult> OnPostAsync(string plant, string checkout)
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
                    if (!string.IsNullOrEmpty(checkout))
                    {
                        var CheckOutDate = DateTime.Now;
                        var CheckOutBy = _authService.GetClaim().UserId;

                        var QueingDOMRepo = new GenericRepository<QingDOM_TB>(unitOfWork.Transaction);
                        var QueingDOMById = await QueingDOMRepo.GetAsync(Id);

                        var CompanyMasterRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                        var CompanyALL = await CompanyMasterRepo.GetAllAsync();
                        var CompanyByPlant = CompanyALL.Where(x => x.company == QueingDOMById.PLANT).FirstOrDefault();

                        var VOUCHER_TYPE = QueingDOMById.WORKORDERNO.Substring(0, 4);
                        var VOUCHER_SERIES = QueingDOMById.WORKORDERNO.Substring(4, 4);
                        var VOUCHER_NO = QueingDOMById.WORKORDERNO.Substring(8);
                        var DSG_LOADINGPLANT = CompanyByPlant.DataAreaId;

                        using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
                        {
                            var upax = unitOfWork2.Transaction.Connection.Execute(@"UPDATE DSG_WORKORDERTABLE SET 
                                DSG_GrossWeight_1 = @DSG_GrossWeight_1,
                                DSG_GrossWeight_2 = @DSG_GrossWeight_2
                            WHERE VOUCHER_TYPE = @VOUCHER_TYPE AND 
                            VOUCHER_SERIES = @VOUCHER_SERIES AND 
                            VOUCHER_NO = @VOUCHER_NO AND
                            DSG_LOADINGPLANT = @DSG_LOADINGPLANT ",
                                new
                                {
                                    @DSG_GrossWeight_1 = WeightOut,
                                    @DSG_GrossWeight_2 = WeightOut,
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
                               EXEC Queing_UpdateCheckOUTWMS @pENDLOADTIME,@pLoadid
                            ",
                                    new
                                    {
                                        @pENDLOADTIME = Convert.ToDateTime(WeightOutDate),
                                        @pLoadid = QueingDOMById.LOADID
                                    },
                                    unitOfWork3.Transaction
                                );
                                unitOfWork3.Complete();
                            }
                        }

                        QueingDOMById.WEIGHTOUT = WeightOut;
                        QueingDOMById.WEIGHTOUTDATE = Convert.ToDateTime(WeightOutDate);
                        QueingDOMById.STATUS = QueingStatusModel.CheckOut;
                        QueingDOMById.CHECKOUTBY = CheckOutBy;
                        QueingDOMById.CHECKOUTDATE = CheckOutDate;

                        await QueingDOMRepo.UpdateAsync(QueingDOMById);

                        unitOfWork.Complete();

                        AlertSuccess = "Check Out Successfully";
                        return Redirect($"/Queing/Process/DOM/{plant}/CheckOut");
                    }
                    else
                    {
                        var ChangeDate = DateTime.Now;
                        var ChangeBy = _authService.GetClaim().UserId;

                        var QueingDOMRepo = new GenericRepository<QingDOM_TB>(unitOfWork.Transaction);
                        var QueingDOMById = await QueingDOMRepo.GetAsync(IdChange);

                        var AgentRepo = new GenericRepository<QingMaster_Agent_TB>(unitOfWork.Transaction);
                        var AgentByID = await AgentRepo.GetAsync(QueingDOMById.AGENTID);

                        var CompanyMasterRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                        var CompanyALL = await CompanyMasterRepo.GetAllAsync();
                        var CompanyByPlant = CompanyALL.Where(x => x.company == QueingDOMById.PLANT).FirstOrDefault();

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
                               @VOUCHER_TYPE = WorkOrderNumber1,
                               @VOUCHER_SERIES = WorkOrderNumber2,
                               @VOUCHER_NO = WorkOrderNumber3,
                               @DSG_LOADINGPLANT = DSG_LOADINGPLANT
                           }, unitOfWork2.Transaction);


                            if (checkax.FirstOrDefault() == null)
                            {
                                AlertError = "Work Order No not have in ax";
                                return Redirect($"/Queing/Process/DOM/{plant}/CheckOut");
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
                                    @DSG_TIME = Convert.ToDateTime(QueingDOMById.WEIGHTINDATE),
                                    @DSG_GROSSWEIGHT_IN = QueingDOMById.WEIGHTIN,
                                    @VOUCHER_TYPE = WorkOrderNumber1,
                                    @VOUCHER_SERIES = WorkOrderNumber2,
                                    @VOUCHER_NO = WorkOrderNumber3,
                                    @DSG_LOADINGPLANT = DSG_LOADINGPLANT
                                },
                                unitOfWork2.Transaction
                            );
                            unitOfWork2.Complete();
                        }

                        var WorkOrderNo = WorkOrderNumber1 + WorkOrderNumber2 + WorkOrderNumber3;

                        QueingDOMById.WORKORDERNO = WorkOrderNo;
                        QueingDOMById.ASSIGNBAYBY = ChangeBy;
                        QueingDOMById.ASSIGNBAYDATE = ChangeDate;

                        await QueingDOMRepo.UpdateAsync(QueingDOMById);

                        unitOfWork.Complete();

                        AlertSuccess = "Change Work Order Number Successfully";
                        return Redirect($"/Queing/Process/DOM/{plant}/CheckOut");
                    }
                    
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/DOM/{plant}/CheckOut");
            }
        }
    }
}
