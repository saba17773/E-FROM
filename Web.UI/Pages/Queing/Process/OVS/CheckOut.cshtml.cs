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

namespace Web.UI.Pages.Queing.Process.OVS
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
        public string TruckID { get; set; }
        [BindProperty]
        public decimal WeightOut { get; set; }
        [BindProperty]
        public string WeightOutDate { get; set; }
        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        public int WeighId { get; set; }
        [BindProperty]
        public string ContainerNo { get; set; }
        [BindProperty]
        public string WorkOrderNumber1 { get; set; }
        [BindProperty]
        public string WorkOrderNumber2 { get; set; }
        [BindProperty]
        public string WorkOrderNumber3 { get; set; }
        [BindProperty]
        public string SubWorkOrderNo { get; set; }
        [BindProperty]
        public int IdChange { get; set; }
        [BindProperty]
        public int isSubWorkOrderNo { get; set; }

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
	                            ,Q.WEIGHID
                                ,Q.ISSUBWORKORDERNO
                            FROM TB_QingOVS Q JOIN
                            TB_QingMaster_Status S ON Q.STATUS = S.ID JOIN
                            TB_QingMaster_Route R ON Q.ROUTEID = R.ID JOIN
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
                    var QueingOVSRepo = new GenericRepository<QingOVS_TB>(unitOfWork1.Transaction);
                    var QueingOVSByID = await QueingOVSRepo.GetAsync(id);

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
                        truckid = QueingOVSByID.TRUCKID,
                        weightout = WeightOut,
                        weightoutdate = DateOut,
                        containerno = QueingOVSByID.CONTAINERNO
                    });

                }
            }
            catch (Exception)
            {
                return new JsonResult(false);
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

                        var QueingOVSRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                        var QueingOVSById = await QueingOVSRepo.GetAsync(Id);

                        var CompanyMasterRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                        var CompanyALL = await CompanyMasterRepo.GetAllAsync();
                        var CompanyByPlant = CompanyALL.Where(x => x.company == QueingOVSById.PLANT).FirstOrDefault();

                        var VOUCHER_TYPE = QueingOVSById.WORKORDERNO.Substring(0, 4);
                        var VOUCHER_SERIES = QueingOVSById.WORKORDERNO.Substring(4, 4);
                        var VOUCHER_NO = QueingOVSById.WORKORDERNO.Substring(8);
                        var DSG_LOADINGPLANT = CompanyByPlant.DataAreaId;

                        using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
                        {
                            if (QueingOVSById.ISSUBWORKORDERNO == 1)
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
                                    return Redirect($"/Queing/Process/OVS/{plant}/CheckOut");
                                }

                                var upax = unitOfWork2.Transaction.Connection.Execute(@"UPDATE DSG_SUBWORKORDERTABLE SET 
                                        DSG_GrossWeight_1 = @DSG_GrossWeight_1,
                                        DSG_GrossWeight_2 = @DSG_GrossWeight_2
                                    WHERE VOUCHER_TYPE = @VOUCHER_TYPE AND 
                                        VOUCHER_SERIES = @VOUCHER_SERIES AND 
                                        DSG_ID = @VOUCHER_NO  ",
                                    new
                                    {
                                        @DSG_GrossWeight_1 = WeightOut,
                                        @DSG_GrossWeight_2 = WeightOut,
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
                                    return Redirect($"/Queing/Process/OVS/{plant}/CheckOut");
                                }

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
                                        @pLoadid = QueingOVSById.LOADID
                                    },
                                    unitOfWork3.Transaction
                                );
                                unitOfWork3.Complete();
                            }
                        }

                        QueingOVSById.WEIGHTOUT = WeightOut;
                        QueingOVSById.WEIGHTOUTDATE = Convert.ToDateTime(WeightOutDate);
                        QueingOVSById.STATUS = QueingStatusModel.CheckOut;
                        QueingOVSById.CHECKOUTBY = CheckOutBy;
                        QueingOVSById.CHECKOUTDATE = CheckOutDate;

                        await QueingOVSRepo.UpdateAsync(QueingOVSById);

                        unitOfWork.Complete();

                        AlertSuccess = "Check Out Successfully";
                        return Redirect($"/Queing/Process/OVS/{plant}/CheckOut");
                    }
                    else
                    {
                        var ChangeDate = DateTime.Now;
                        var ChangeBy = _authService.GetClaim().UserId;

                        var QingOVSRepo = new GenericRepository<QingOVS_TB>(unitOfWork.Transaction);
                        var QueingOVSById = await QingOVSRepo.GetAsync(IdChange);

                        var CompanyMasterRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                        var CompanyALL = await CompanyMasterRepo.GetAllAsync();
                        var CompanyByPlant = CompanyALL.Where(x => x.company == QueingOVSById.PLANT).FirstOrDefault();

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

                        var dsg_id = WorkOrderNumber3 + "-" + SubWorkOrderNo;

                        using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection("AXCust")))
                        {
                            if (isSub == 1)
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
                                    @VOUCHER_TYPE = WorkOrderNumber1,
                                    @VOUCHER_SERIES = WorkOrderNumber2,
                                    @VOUCHER_NO = dsg_id
                                }, unitOfWork2.Transaction);


                                if (checkax.FirstOrDefault() == null)
                                {
                                    AlertError = "Work Order No not have in ax";
                                    return Redirect($"/Queing/Process/OVS/{plant}/CheckOut");
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
                                        @DSG_TIME = Convert.ToDateTime(QueingOVSById.WEIGHTINDATE),
                                        @DSG_GROSSWEIGHT_IN = QueingOVSById.WEIGHTIN,
                                        @DSG_ContainerNo = QueingOVSById.CONTAINERNO,
                                        @DSG_SealNo = QueingOVSById.SEALNO,
                                        @VOUCHER_TYPE = WorkOrderNumber1,
                                        @VOUCHER_SERIES = WorkOrderNumber2,
                                        @VOUCHER_NO = dsg_id
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
                                    @VOUCHER_TYPE = WorkOrderNumber1,
                                    @VOUCHER_SERIES = WorkOrderNumber2,
                                    @VOUCHER_NO = WorkOrderNumber3,
                                    @DSG_LOADINGPLANT = DSG_LOADINGPLANT
                                }, unitOfWork2.Transaction);


                                if (checkax.FirstOrDefault() == null)
                                {
                                    AlertError = "Work Order No not have in ax";
                                    return Redirect($"/Queing/Process/OVS/{plant}/CheckOut");
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
                                        @DSG_TIME = Convert.ToDateTime(QueingOVSById.WEIGHTINDATE),
                                        @DSG_GROSSWEIGHT_IN = QueingOVSById.WEIGHTIN,
                                        @DSG_ContainerNo = QueingOVSById.CONTAINERNO,
                                        @DSG_SealNo = QueingOVSById.SEALNO,
                                        @VOUCHER_TYPE = WorkOrderNumber1,
                                        @VOUCHER_SERIES = WorkOrderNumber2,
                                        @VOUCHER_NO = WorkOrderNumber3,
                                        @DSG_LOADINGPLANT = DSG_LOADINGPLANT
                                    },
                                    unitOfWork2.Transaction
                                );
                                unitOfWork2.Complete();
                            }
                                
                        }

                        QueingOVSById.WORKORDERNO = WorkOrderNo;
                        QueingOVSById.ISSUBWORKORDERNO = isSub;
                        QueingOVSById.ASSIGNBAYBY = ChangeBy;
                        QueingOVSById.ASSIGNBAYDATE = ChangeDate;

                        await QingOVSRepo.UpdateAsync(QueingOVSById);

                        unitOfWork.Complete();

                        AlertSuccess = "Change Work Order Number Successfully";
                        return Redirect($"/Queing/Process/OVS/{plant}/CheckOut");
                    }
                       
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/OVS/{plant}/CheckOut");
            }
        }
    }
}
