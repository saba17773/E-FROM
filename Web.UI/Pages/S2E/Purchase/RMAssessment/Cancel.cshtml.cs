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
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Purchase.RMAssessment
{
    public class CancelModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int RequestId { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public string MaterialGroup { get; set; }
        [BindProperty]
        public string MaterialName { get; set; }
        [BindProperty]
        public string ItemGroup { get; set; }
        [BindProperty]
        public bool DataAreaID1 { get; set; }
        [BindProperty]
        public bool DataAreaID2 { get; set; }
        [BindProperty]
        public bool DataAreaID3 { get; set; }
        [BindProperty]
        public bool DataAreaID4 { get; set; }
        [BindProperty]
        public bool DataAreaID5 { get; set; }
        [BindProperty]
        public int CreateBy { get; set; }
        [BindProperty]
        public bool DocRef1 { get; set; }
        [BindProperty]
        public bool DocRef2 { get; set; }
        [BindProperty]
        public bool DocRef3 { get; set; }
        [BindProperty]
        public bool DocRef4 { get; set; }
        [BindProperty]
        public string ReasonOfEvaluation { get; set; }
        [BindProperty]
        public string DocRef4_Desc { get; set; }
        [BindProperty]
        public bool DocRef5 { get; set; }
        [BindProperty]
        public bool DocRef6 { get; set; }
        [BindProperty]
        public bool DocRef7 { get; set; }
        [BindProperty]
        public string Urgent { get; set; }
        [BindProperty]
        public int isStartTest { get; set; }
        [BindProperty]
        public string isStartTestRemark { get; set; }
        [BindProperty]
        public string DocRef9_Desc { get; set; }
        [BindProperty]
        public bool DocRef10 { get; set; }
        [BindProperty]
        public bool DocRef11 { get; set; }
        [BindProperty]
        public bool DocRef12 { get; set; }
        [BindProperty]
        public bool DocRef13 { get; set; }
        [BindProperty]
        public bool DocRef14 { get; set; }
        [BindProperty]
        public bool DocRef15 { get; set; }
        [BindProperty]
        public bool DocRef16 { get; set; }
        [BindProperty]
        public string DocRef16_Desc { get; set; }
        [BindProperty]
        public bool DocRef17 { get; set; }
        [BindProperty]
        public bool DocRef18 { get; set; }
        [BindProperty]
        public string Ecotoxic { get; set; }
        [BindProperty]
        public int isEcotoxic { get; set; }
        [BindProperty]
        public string isEcotoxicRemark { get; set; }
        [BindProperty]
        public string DocRef18_Desc { get; set; }
        [BindProperty]
        public bool DocRef19 { get; set; }
        [BindProperty]
        public bool DocRef20 { get; set; }
        [BindProperty]
        public string DocRef20_Desc { get; set; }
        [BindProperty]
        public string Humantoxic { get; set; }
        [BindProperty]
        public int isHumantoxic { get; set; }
        [BindProperty]
        public string isHumantoxicRemark { get; set; }
        [BindProperty]
        public bool DocRef21 { get; set; }
        [BindProperty]
        public bool DocRef22 { get; set; }
        [BindProperty]
        public bool DocRef23 { get; set; }
        [BindProperty]
        public bool DocRef24 { get; set; }
        [BindProperty]
        public bool DocRef25 { get; set; }
        [BindProperty]
        public bool DocRef26 { get; set; }
        [BindProperty]
        public bool DocRef27 { get; set; }
        [BindProperty]
        public string DocRef27_Desc { get; set; }
        [BindProperty]
        public bool DocRef28 { get; set; }
        [BindProperty]
        public bool DocRef29 { get; set; }
        [BindProperty]
        public string RFQ { get; set; }
        [BindProperty]
        public string SupplierName { get; set; }
        [BindProperty]
        public string ProductionSite { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public decimal Price { get; set; }
        [BindProperty]
        public string SupplierNameRef { get; set; }
        [BindProperty]
        public string ProductionSiteRef { get; set; }
        [BindProperty]
        public string ItemCodeRef { get; set; }
        [BindProperty]
        public string ItemNameRef { get; set; }
        [BindProperty]
        public decimal PriceRef { get; set; }
        [BindProperty]
        public decimal SupplierD1 { get; set; }
        [BindProperty]
        public decimal SupplierD2 { get; set; }
        [BindProperty]
        public decimal SupplierD3 { get; set; }
        [BindProperty]
        public decimal SupplierD4 { get; set; }
        [BindProperty]
        public decimal SupplierD5 { get; set; }
        [BindProperty]
        public decimal SupplierRefD1 { get; set; }
        [BindProperty]
        public decimal SupplierRefD2 { get; set; }
        [BindProperty]
        public decimal SupplierRefD3 { get; set; }
        [BindProperty]
        public decimal SupplierRefD4 { get; set; }
        [BindProperty]
        public decimal SupplierRefD5 { get; set; }
        [BindProperty]
        public decimal MonthSaveCost { get; set; }
        [BindProperty]
        public decimal YearSaveCost { get; set; }
        [BindProperty]
        public string PlantCodeD1D2 { get; set; }
        [BindProperty]
        public string PlantCodeD3 { get; set; }
        [BindProperty]
        public string PlantCodeD4 { get; set; }
        [BindProperty]
        public string PlantCodeD5 { get; set; }
        [BindProperty]
        public decimal Qty { get; set; }
        [BindProperty]
        public string Dataarea { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public int isCompaire { get; set; }
        [BindProperty]
        public int AssessmentId { get; set; }
        [BindProperty]
        public string CurrencyCode { get; set; }
        [BindProperty]
        public string CurrencyCodeREF { get; set; }
        [BindProperty]
        public string PerUnit { get; set; }
        [BindProperty]
        public string PerUnitRef { get; set; }
        [BindProperty]
        public int DepartmentID { get; set; }
        [BindProperty]
        public string DepartmentRemark { get; set; }
        public List<SelectListItem> DepartmentMaster { get; set; }
        [BindProperty]
        public string CancelRemark { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public CancelModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IEmailService emailService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync(int AssessmentID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_RMASSESSMENT));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var CancelBy = _authService.GetClaim().UserId;
                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessmentALL = await RMAssessmentRepo.GetAllAsync();
                    var RMAssessmentByCancelBy = RMAssessmentALL.Where(x => x.ID == AssessmentID &&
                                                                    x.CREATEBY == CancelBy).FirstOrDefault();
                    if (RMAssessmentByCancelBy == null)
                    {
                        AlertError = "ไม่สามารถ Cancel Request นี้ได้";
                        return Redirect("/S2E/Purchase/RMAssessment/Main");
                    }

                    AssessmentId = AssessmentID;
                    DepartmentMaster = await GetDepartmentMaster();
                    await GetData(AssessmentID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/RMAssessment/Main");
            }
        }
        public async Task<List<SelectListItem>> GetDepartmentMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var DeptRepo = new GenericRepository<S2EMaster_LABTestDepartment_TB>(unitOfWork.Transaction);

                var DeptALL = await DeptRepo.GetAllAsync();

                return DeptALL
                    .Where(x => x.ISACTIVE == 1)
                    .OrderBy(x1 => x1.ISORDERBY).ThenBy(x2 => x2.ID)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DEPARTMENTDESC
                    })
                    .ToList();
            }
        }
        public async Task GetData(int AssessmentID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentBYID = await RMAssessmentRepo.GetAsync(AssessmentID);

                RequestId = RMAssessmentBYID.REQUESTID;
                Plant = RMAssessmentBYID.PLANT;

                if (RMAssessmentBYID.PLANT != null)
                {
                    string[] DataAreaList = RMAssessmentBYID.PLANT.Split(",");
                    foreach (var d in DataAreaList)
                    {
                        if (d == "DSL") { DataAreaID1 = true; }
                        if (d == "DRB") { DataAreaID2 = true; }
                        if (d == "DSI") { DataAreaID3 = true; }
                        if (d == "DSR") { DataAreaID4 = true; }
                        if (d == "STR") { DataAreaID5 = true; }
                    }
                }
                else
                {
                    DataAreaID1 = false;
                    DataAreaID2 = false;
                    DataAreaID3 = false;
                    DataAreaID4 = false;
                    DataAreaID5 = false;
                }

                DepartmentID = RMAssessmentBYID.DEPARTMENTID;
                DepartmentRemark = RMAssessmentBYID.DEPARTMENTREMARK;
                MaterialGroup = RMAssessmentBYID.MATERIALGROUP;
                MaterialName = RMAssessmentBYID.MATERIALNAME;
                ItemGroup = RMAssessmentBYID.ITEMGROUP;
                SupplierD1 = RMAssessmentBYID.SUPPLIERD1;
                SupplierD2 = RMAssessmentBYID.SUPPLIERD2;
                SupplierD3 = RMAssessmentBYID.SUPPLIERD3;
                SupplierD4 = RMAssessmentBYID.SUPPLIERD4;
                SupplierD5 = RMAssessmentBYID.SUPPLIERD5;
                SupplierRefD1 = RMAssessmentBYID.SUPPLIERREFD1;
                SupplierRefD2 = RMAssessmentBYID.SUPPLIERREFD2;
                SupplierRefD3 = RMAssessmentBYID.SUPPLIERREFD3;
                SupplierRefD4 = RMAssessmentBYID.SUPPLIERREFD4;
                SupplierRefD5 = RMAssessmentBYID.SUPPLIERREFD5;
                MonthSaveCost = RMAssessmentBYID.MONTHSAVECOST;
                YearSaveCost = RMAssessmentBYID.YEARSAVECOST;
                PlantCodeD1D2 = RMAssessmentBYID.PLANTCODED1D2;
                PlantCodeD3 = RMAssessmentBYID.PLANTCODED3;
                PlantCodeD4 = RMAssessmentBYID.PLANTCODED4;
                PlantCodeD5 = RMAssessmentBYID.PLANTCODED5;
                isStartTest = RMAssessmentBYID.ISSTARTTEST == 1 ? 1 : 2;
                isStartTestRemark = RMAssessmentBYID.ISSTARTTESTREMARK;

                if (RMAssessmentBYID.ISECOTOXIC == 0)
                {
                    isEcotoxic = 0;
                }
                else
                {
                    isEcotoxic = RMAssessmentBYID.ISECOTOXIC == 1 ? 1 : 2;
                }
                isEcotoxicRemark = RMAssessmentBYID.ISECOTOXICREMARK;

                if (RMAssessmentBYID.ISHUMANTOXIC == 0)
                {
                    isHumantoxic = 0;
                }
                else
                {
                    isHumantoxic = RMAssessmentBYID.ISHUMANTOXIC == 1 ? 1 : 2;
                }

                isHumantoxicRemark = RMAssessmentBYID.ISHUMANTOXICREMARK;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestId);

                RequestCode = NewRequestByID.REQUESTCODE;
                SupplierName = NewRequestByID.DEALER + " || " + NewRequestByID.SUPPLIERNAME;
                ProductionSite = NewRequestByID.PRODUCTIONSITE;
                ItemCode = NewRequestByID.ITEMCODE;
                ItemName = NewRequestByID.ITEMNAME;
                Price = NewRequestByID.PRICE;
                CurrencyCode = NewRequestByID.CURRENCYCODE;
                Unit = NewRequestByID.UNIT;
                Dataarea = NewRequestByID.PLANT;
                Qty = NewRequestByID.QTY;
                isCompaire = NewRequestByID.ISCOMPAIRE;
                PerUnit = NewRequestByID.PERUNIT;

                if (NewRequestByID.ISCOMPAIRE == 1)
                {
                    var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                    var NewRequestCompaireALL = await NewRequestCompaireRepo.GetAllAsync();
                    var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == RequestId).FirstOrDefault();

                    SupplierNameRef = NewRequestCompaireByRequestID.DEALERREF + " || " + NewRequestCompaireByRequestID.SUPPLIERNAMEREF;
                    ProductionSiteRef = NewRequestCompaireByRequestID.PRODUCTIONSITEREF;
                    ItemCodeRef = NewRequestCompaireByRequestID.ITEMCODEREF;
                    ItemNameRef = NewRequestCompaireByRequestID.ITEMNAMEREF;
                    PriceRef = NewRequestCompaireByRequestID.PRICEREF;
                    CurrencyCodeREF = NewRequestCompaireByRequestID.CURRENCYCODEREF;
                    PerUnitRef = NewRequestCompaireByRequestID.PERUNITREF;
                }

                //Doc
                var AssessmentLogDocRepo = new GenericRepository<S2ERMAssessmentLogsDoc_TB>(unitOfWork.Transaction);
                var AssessmentLogDocALL = await AssessmentLogDocRepo.GetAllAsync();

                //checkbox
                foreach (var docLog in AssessmentLogDocALL.Where(x => x.ASSESSMENTID == AssessmentID))
                {
                    //4,16,18,20,27
                    if (docLog.DOCID == 1) { DocRef1 = true; }
                    if (docLog.DOCID == 2) { DocRef2 = true; }
                    if (docLog.DOCID == 3) { DocRef3 = true; }
                    if (docLog.DOCID == 4) { DocRef4 = true; DocRef4_Desc = docLog.REMARK; }
                    if (docLog.DOCID == 5) { DocRef5 = true; }
                    if (docLog.DOCID == 6) { DocRef6 = true; }
                    if (docLog.DOCID == 7) { DocRef7 = true; }
                    //if (docLog.DOCID == 8) { DocRef8 = true; }
                    //if (docLog.DOCID == 9) { DocRef9 = true; DocRef9_Desc = docLog.REMARK; }
                    if (docLog.DOCID == 10) { DocRef10 = true; }
                    if (docLog.DOCID == 11) { DocRef11 = true; }
                    if (docLog.DOCID == 12) { DocRef12 = true; }
                    if (docLog.DOCID == 13) { DocRef13 = true; }
                    if (docLog.DOCID == 14) { DocRef14 = true; }
                    if (docLog.DOCID == 15) { DocRef15 = true; }
                    if (docLog.DOCID == 16) { DocRef16 = true; DocRef16_Desc = docLog.REMARK; }
                    if (docLog.DOCID == 17) { DocRef17 = true; }
                    if (docLog.DOCID == 18) { DocRef18 = true; DocRef18_Desc = docLog.REMARK; }
                    if (docLog.DOCID == 19) { DocRef19 = true; }
                    if (docLog.DOCID == 20) { DocRef20 = true; DocRef20_Desc = docLog.REMARK; }
                    if (docLog.DOCID == 21) { DocRef21 = true; }
                    if (docLog.DOCID == 22) { DocRef22 = true; }
                    if (docLog.DOCID == 23) { DocRef23 = true; }
                    if (docLog.DOCID == 24) { DocRef24 = true; }
                    if (docLog.DOCID == 25) { DocRef25 = true; }
                    if (docLog.DOCID == 26) { DocRef26 = true; }
                    if (docLog.DOCID == 27) { DocRef27 = true; DocRef27_Desc = docLog.REMARK; }
                    if (docLog.DOCID == 28) { DocRef28 = true; }
                    if (docLog.DOCID == 29) { DocRef29 = true; }
                }

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostGridViewFileUploadAsync(int RequestID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ENewRequestLogsFileGridViewModel>($@"
                        SELECT ID,
		                    REQUESTID,
		                    FILENAME,
                            CREATEBY,
		                    CONVERT(NVARCHAR,CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,CREATEDATE,108) AS CREATEDATE
                    FROM TB_S2ENewRequestLogsFile 
                    WHERE REQUESTID = {RequestID} 
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
        public async Task<IActionResult> OnGetDownloadFileUploadAsync(int RequestID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var NewRequestLogFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                    var NewRequestLogFileByID = await NewRequestLogFileRepo.GetAsync(Fileid);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/NewRequest";

                    var fileName = NewRequestLogFileByID.FILENAME;

                    var basePath = $"{filePath}/{fileName}";
                    if (!System.IO.File.Exists(basePath))
                    {
                        throw new Exception("File not found.");
                    }

                    byte[] fileBytes = System.IO.File.ReadAllBytes(basePath);

                    return File(fileBytes, "application/x-msdownload", fileName);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnPostGridViewRMAssessmentFileUploadAsync(int AssessmentID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ERMAssessmentLogsFileGridViewModel>($@"
                        SELECT *
                        FROM
                        (
	                        SELECT ID,
		                        ASSESSMENTID REQUESTID,
		                        FILENAME,
		                        CREATEBY,
		                        CONVERT(NVARCHAR,CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,CREATEDATE,108) AS CREATEDATE

	                        FROM TB_S2ERMAssessmentLogsFile 
	                        WHERE ASSESSMENTID = {AssessmentID} 
                        )T
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
        public async Task<IActionResult> OnPostGridViewApproveAsync(int AssessmentID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<GetApproveLogsGridViewModel>($@"
                        SELECT * FROM
                        (
	                        SELECT EMAIL,
		                        CONVERT(NVARCHAR,APPROVEDATE,103) + ' ' + CONVERT(NVARCHAR,APPROVEDATE,108) AS APPROVEDATE,
		                        REMARK
	                        FROM TB_S2ERMAssessmentApproveTrans
	                        WHERE ASSESSMENTID = {AssessmentID} AND ISCURRENTAPPROVE = 1
	                        AND APPROVEDATE IS NOT NULL
                        )T
                        ORDER BY APPROVEDATE ASC
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
        public async Task<IActionResult> OnGetDownloadFileRMAssessmentUploadAsync(int AssessmentID, int Fileid, int RequestID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RMAssessmentLogFileRepo = new GenericRepository<S2ERMAssessmentLogsFile_TB>(unitOfWork.Transaction);
                    var RMAssessmentLogFile = await RMAssessmentLogFileRepo.GetAsync(Fileid);

                    var RMAssessmentRequestRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessment = await RMAssessmentRequestRepo.GetAsync(AssessmentID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/RMAssessment";

                    var fileName = RMAssessmentLogFile.FILENAME;

                    var basePath = $"{filePath}/{fileName}";
                    if (!System.IO.File.Exists(basePath))
                    {
                        throw new Exception("File not found.");
                    }

                    byte[] fileBytes = System.IO.File.ReadAllBytes(basePath);

                    return File(fileBytes, "application/x-msdownload", fileName);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnPostAsync(int AssessmentID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var CancelBy = _authService.GetClaim().UserId;
                    var DatetimeNow = DateTime.Now;

                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                    var RequestID = RMAssessmentByID.REQUESTID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    if (RMAssessmentByID.APPROVESTATUS == RequestStatusModel.WaitingForApprove)
                    {
                        var TransRepo = new GenericRepository<S2ERMAssessmentApproveTrans_TB>(unitOfWork.Transaction);
                        var TransRepoALL = await TransRepo.GetAllAsync();

                        var TransCurrent = TransRepoALL.Where(x => x.ASSESSMENTID == RMAssessmentByID.ID
                                                               && x.SENDEMAILDATE != null
                                                               && x.APPROVEDATE == null
                                                               && x.REJECTDATE == null
                                                               && x.ISDONE == 0
                                                               && x.ISCURRENTAPPROVE == 1).FirstOrDefault();

                        if (TransCurrent == null)
                        {
                            AlertError = "ไม่สามารถยกเลิกรายการนี้ได้กรุณาดำเนินการใหม่อีกครั้ง";
                            return Redirect("/S2E/Purchase/RMAssessment/Main");
                        }

                        var NonceRepo = new GenericRepository<S2ERMAssessmentNonce_TB>(unitOfWork.Transaction);
                        var NonceALL = await NonceRepo.GetAllAsync();

                        var NonceCurrent = NonceALL.Where(x => x.CREATEDATE == TransCurrent.SENDEMAILDATE && x.ISUSED == 0).FirstOrDefault();

                        if (NonceCurrent == null)
                        {
                            AlertError = "ไม่สามารถยกเลิกรายการนี้ได้กรุณาดำเนินการใหม่อีกครั้ง";
                            return Redirect("/S2E/Purchase/RMAssessment/Main");
                        }

                        var NonceByID = await NonceRepo.GetAsync(NonceCurrent.ID);
                        NonceByID.ISUSED = 1;
                        await NonceRepo.UpdateAsync(NonceByID);
                    }

                    NewRequestByID.CANCELREMARK = CancelRemark;
                    NewRequestByID.APPROVESTATUS = RequestStatusModel.Cancel;
                    NewRequestByID.COMPLETEDATE = DatetimeNow;
                    NewRequestByID.COMPLETEBY = CancelBy;

                    RMAssessmentByID.CANCELREMARK = CancelRemark;
                    RMAssessmentByID.APPROVESTATUS = RequestStatusModel.Cancel;
                    RMAssessmentByID.COMPLETEDATE = DatetimeNow;
                    RMAssessmentByID.COMPLETEBY = CancelBy;

                    await NewRequestRepo.UpdateAsync(NewRequestByID);
                    await RMAssessmentRepo.UpdateAsync(RMAssessmentByID);

                    unitOfWork.Complete();

                    AlertSuccess = "ยกเลิกใบร้องขอทดสอบวัตถุดิบสำเร็จ";
                    return Redirect("/S2E/Purchase/RMAssessment/Main");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Purchase/RMAssessment/{AssessmentID}/Cancel");
            }
        }
    }
}
