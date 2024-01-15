using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Purchase
{
    public class ApproveRMAssessmentModel : PageModel
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
        public string ApproveRemark { get; set; }
        [BindProperty]
        public int ApproveResult { get; set; }
        [BindProperty]
        public int isKeyin { get; set; }
        [BindProperty]
        public int isEcotoxic { get; set; }
        [BindProperty]
        public string isEcotoxicRemark { get; set; }
        [BindProperty]
        public int isHumantoxic { get; set; }
        [BindProperty]
        public string isHumantoxicRemark { get; set; }
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

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public ApproveRMAssessmentModel(
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
        public async Task<IActionResult> OnGetAsync(int AssessmentID, int TranID, string nonce, string email, int isKeyinWhenApprove)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var _nonce = await unitOfWork.S2EControl.GetNonceRMAssessmentByKey(nonce);
                    if (_nonce.ISUSED == 1)
                    {
                        AlertError = "Link Is Used.";
                        return Redirect($"/S2E/Purchase/RMAssessmentTodolist?Email={email}");
                    }

                    DepartmentMaster = await GetDepartmentMaster();
                    AssessmentId = AssessmentID;
                    isKeyin = isKeyinWhenApprove;
                   
                    await GetData(AssessmentID);

                    isEcotoxic = 0;
                    isHumantoxic = 0;

                    if (isKeyinWhenApprove == 0)
                    {
                        await GetData2(AssessmentID);
                    }

                    return Page();
                }
            }
            catch (Exception)
            {
                throw;
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
        public async Task GetData2(int AssessmentID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentBYID = await RMAssessmentRepo.GetAsync(AssessmentID);

                isEcotoxic = RMAssessmentBYID.ISECOTOXIC == 1 ? 1 : 2;
                isEcotoxicRemark = RMAssessmentBYID.ISECOTOXICREMARK;
                isHumantoxic = RMAssessmentBYID.ISHUMANTOXIC == 1 ? 1 : 2;
                isHumantoxicRemark = RMAssessmentBYID.ISHUMANTOXICREMARK;

                unitOfWork.Complete();
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
        public async Task<IActionResult> OnPostGridViewApproveAsync(int AssessmentID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<GetApproveLogsGridViewModel>($@"
                       SELECT EMAIL,
                        CONVERT(NVARCHAR,APPROVEDATE,103) + ' ' + CONVERT(NVARCHAR,APPROVEDATE,108) AS APPROVEDATE
                        ,REMARK
                    FROM TB_S2ERMAssessmentApproveTrans
                    WHERE ASSESSMENTID = {AssessmentID} AND ISCURRENTAPPROVE = 1
                    AND APPROVEDATE IS NOT NULL
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
        public async Task<IActionResult> OnPostAsync(int AssessmentID, int TranID, string nonce, string email)
        {
            try
            {
                if (ApproveResult == 0)
                {
                    AlertError = "กรุณาเลือกว่าจะ อนุมัติ หรือ ไม่อนุมัติ";
                }
                if (ApproveResult == 2 && ApproveRemark == null)
                {

                    AlertError = "กรุณาใส่สาเหตุที่ไม่อนุมัติ !!";
                    return Redirect($"/S2E/Purchase/RMAssessmentApprove?AssessmentID={AssessmentID}&TranID={TranID}&nonce={nonce}&email={email}");
                }
                if (!ModelState.IsValid)
                {
                    return Redirect($"/S2E/Purchase/ApproveRMAssessment?AssessmentID={AssessmentID}&TranID={TranID}&nonce={nonce}&email={email}");
                }
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var AssessmentBYID = await AssessmentRepo.GetAsync(AssessmentID);
                    var RequestID = AssessmentBYID.REQUESTID;
                    var DatetimeNow = DateTime.Now;
                    int approvemasterid = AssessmentBYID.APPROVEMASTERID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    //CHECK NONCE
                    var nonceRepo = new GenericRepository<S2ERMAssessmentNonce_TB>(unitOfWork.Transaction);
                    var _nonce = await unitOfWork.S2EControl.GetNonceRMAssessmentByKey(nonce);
                    if (_nonce.ISUSED == 1)
                    {
                        throw new Exception("Link Is Used.");
                    }
                    //UPDATE NONCE IS USED
                    _nonce.ISUSED = 1;

                    //GET APPROVE TRANS
                    var RMAssessmentTransRepo = new GenericRepository<S2ERMAssessmentApproveTrans_TB>(unitOfWork.Transaction);
                    var RMAssessmentTrans = await RMAssessmentTransRepo.GetAsync(TranID);
                    var ApproveLevel = RMAssessmentTrans.APPROVELEVEL;
                    var ApproveGroupID = RMAssessmentTrans.APPROVEGROUPID;

                    var ApproveTransRepo = new GenericRepository<S2ERMAssessmentApproveTrans_TB>(unitOfWork.Transaction);
                    var ApproveTranAllLevel = await ApproveTransRepo.GetAllAsync();
                    var ApproveTranLevel = ApproveTranAllLevel.Where(x => x.ASSESSMENTID == AssessmentID &&
                                                                    x.APPROVEMASTERID == approvemasterid &&
                                                                    x.APPROVELEVEL == ApproveLevel &&
                                                                    x.ISCURRENTAPPROVE == 1 &&
                                                                    x.APPROVEGROUPID == ApproveGroupID);

                    //UPDATE Approve or Reject
                    foreach (var UpdateApproveTrans in ApproveTranLevel)
                    {
                        UpdateApproveTrans.ISDONE = 1;
                        if (UpdateApproveTrans.EMAIL == email)
                        {
                            UpdateApproveTrans.REMARK = ApproveRemark;
                            if (ApproveResult == 1)
                            {
                                UpdateApproveTrans.APPROVEDATE = DatetimeNow;
                            }
                            else if (ApproveResult == 2)
                            {
                                UpdateApproveTrans.REJECTDATE = DatetimeNow;
                            }
                        }
                        await ApproveTransRepo.UpdateAsync(UpdateApproveTrans);
                    }

                    //GET REQUEST BY DETAIL
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(AssessmentBYID.CREATEBY);

                    //GET APPROVE TRANS ALL LEVEL
                    var ApproveTransAll = await unitOfWork.S2EControl.GetApproveTransByAssessmentIDAllLevel(AssessmentID, approvemasterid, ApproveGroupID);
                    var AllLevel = ApproveTransAll.ToList().Count;

                    //isFinal
                    if (AssessmentBYID.CURRENTAPPROVESTEP == AllLevel && ApproveResult == 1)
                    {
                        if (AssessmentBYID.COMPLETEDATE == null)
                        {
                            //UPDATE ASSESSMENT_TB (HEAD TABLE)
                            AssessmentBYID.APPROVESTATUS = RequestStatusModel.Complete;
                            if (AssessmentBYID.ISECOTOXIC == 0)
                            {
                                AssessmentBYID.ISECOTOXIC = isEcotoxic;
                                AssessmentBYID.ISECOTOXICREMARK = isEcotoxicRemark;
                            }
                            if (AssessmentBYID.ISHUMANTOXIC == 0)
                            {
                                AssessmentBYID.ISHUMANTOXIC = isHumantoxic;
                                AssessmentBYID.ISHUMANTOXICREMARK = isHumantoxicRemark;
                            }

                            //GET EMAIL SUCCESSFULLY
                            var EmailSuccess = new List<string>();
                            var ApproveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                            var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();

                            var ApproveFlowReceiveComplete = ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid &&
                                                                                  x.ReceiveWhenComplete == 1 && x.IsActive == 1);
                            //CASE WHEN SET IN FLOW MASTER
                            if (ApproveFlowReceiveComplete != null)
                            {
                                foreach (var emaillog in ApproveFlowReceiveComplete)
                                {
                                    EmailSuccess.Add(emaillog.Email);
                                }
                            }

                            //Email Requester
                            EmailSuccess.Add(UserALL.Email);

                            //SEND EMAIL
                            var sendEmail = _emailService.SendEmail(
                                 $"{NewRequestByID.REQUESTCODE} / ร้องขอเพื่ออนุมัติเอกสารขอประเมินวัตถุดิบ ",
                                  $@"
                                    <b> REQUEST NO : {NewRequestByID.REQUESTCODE}  </b><br/>
                                    <b>สถานะ : ดำเนินการเรียบร้อย </b>
                                  ",
                                 EmailSuccess,
                                 new List<string> { }
                           );

                            if (sendEmail.Result == false)
                            {
                                throw new Exception(sendEmail.Message);
                            }

                        }
                    }
                    //isReject
                    else if (ApproveResult == 2 && ApproveRemark != null)
                    {
                        AssessmentBYID.APPROVESTATUS = RequestStatusModel.Reject;
                        AssessmentBYID.ISECOTOXIC = 0;
                        AssessmentBYID.ISECOTOXICREMARK = null;
                        AssessmentBYID.ISHUMANTOXIC = 0;
                        AssessmentBYID.ISHUMANTOXICREMARK = null; 

                        //GET EMAIL REJECT
                        var EmaiReject = new List<string>();
                        //CASE SET IN FLOW
                        var ApproveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                        var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();
                        //foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenFailed == 1 && x.IsActive == 1 && x.ApproveLevel < PCRequestBYID.CURRENTAPPROVESTEP))
                        //{
                        //    EmaiReject.Add(emaillog.Email);
                        //}
                        EmaiReject.Add(UserALL.Email);

                        //GET Reject BY
                        var approveFlowNameALL = ApproveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                        x.ApproveLevel == AssessmentBYID.CURRENTAPPROVESTEP &&
                                                                        x.IsActive == 1);
                        var RejectByFirstName = approveFlowNameALL.Select(s => s.Name).FirstOrDefault();
                        var RejectByLastName = approveFlowNameALL.Select(s => s.LastName).FirstOrDefault();
                        var RejectBy = RejectByFirstName + " " + RejectByLastName;


                        //SEND EMAIL
                        var sendEmail = _emailService.SendEmail(
                             $"{NewRequestByID.REQUESTCODE} / Reject / ร้องขอเพื่ออนุมัติเอกสารขอประเมินวัตถุดิบ ",
                              $@"
                                    <b> REQUEST NO : {NewRequestByID.REQUESTCODE}  </b><br/>
                                    <b>สถานะ : </b><b style='color:red'> ไม่ผ่านการอนุมัติ </b>    <br/>
                                    <b style='color:black'>สาเหตุที่ไม่อนุมัติ : </b> {ApproveRemark} <br/>
                                    <b>Reject By : </b>{RejectBy}
                                  ",
                             EmaiReject,
                             new List<string> { }
                       );

                        if (sendEmail.Result == false)
                        {
                            throw new Exception(sendEmail.Message);
                        }

                    }
                    //isApprove And Send to Next Level
                    else
                    {
                        //UPDATE PCREQUEST_TB (HEAD TABLE)
                        AssessmentBYID.CURRENTAPPROVESTEP += 1;
                        AssessmentBYID.APPROVESTATUS = RequestStatusModel.WaitingForApprove;
                        if (AssessmentBYID.ISECOTOXIC == 0)
                        {
                            AssessmentBYID.ISECOTOXIC = isEcotoxic;
                            AssessmentBYID.ISECOTOXICREMARK = isEcotoxicRemark;
                        }
                        if (AssessmentBYID.ISHUMANTOXIC == 0)
                        {
                            AssessmentBYID.ISHUMANTOXIC = isHumantoxic;
                            AssessmentBYID.ISHUMANTOXICREMARK = isHumantoxicRemark;
                        }

                        //GENERATE NONCE
                        var nonceKey = Guid.NewGuid().ToString();
                        await nonceRepo.InsertAsync(new S2ERMAssessmentNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = DatetimeNow,
                            EXPIREDATE = DatetimeNow.AddDays(7),
                            ISUSED = 0
                        });

                        //NEXT APPROVE LEVEL
                        var nextALL = new GenericRepository<S2ERMAssessmentApproveTrans_TB>(unitOfWork.Transaction);
                        var nextAllLevel = await nextALL.GetAllAsync();
                        var nextLevel = nextAllLevel.Where(x => x.ASSESSMENTID == AssessmentID &&
                                                            x.APPROVELEVEL == AssessmentBYID.CURRENTAPPROVESTEP &&
                                                            x.APPROVEMASTERID == approvemasterid &&
                                                            x.ISCURRENTAPPROVE == 1 &&
                                                            x.APPROVEGROUPID == ApproveGroupID);
                        foreach (var next in nextLevel)
                        {
                            var BodyEmail = "";
                            if (NewRequestByID.ISCOMPAIRE == 1)
                            {
                                var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                                var NewRequestCompaireALL = await NewRequestCompaireRepo.GetAllAsync();
                                var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == NewRequestByID.ID).FirstOrDefault();

                                BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายละเอียดผู้ขาย/วัตถุดิบ (ที่มีอยู่)  </b><br/>
                                   <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestCompaireByRequestID.VENDORIDREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestCompaireByRequestID.SUPPLIERNAMEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {NewRequestCompaireByRequestID.DEALERREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {NewRequestCompaireByRequestID.PRODUCTIONSITEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestCompaireByRequestID.DEALERADDRESSREF.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestCompaireByRequestID.ITEMCODEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {NewRequestCompaireByRequestID.ITEMNAMEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestCompaireByRequestID.PRICEREF)} {NewRequestCompaireByRequestID.CURRENCYCODEREF} / {NewRequestCompaireByRequestID.PERUNITREF}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b> รายการวัตถุดิบที่นำเข้า / นำมาเปรียบเทียบ  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.VENDORID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {NewRequestByID.DEALER}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {NewRequestByID.PRODUCTIONSITE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestByID.ITEMCODE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {NewRequestByID.ITEMNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)} {NewRequestByID.CURRENCYCODE} / {NewRequestByID.PERUNIT}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/RMAssessmentTodolist?Email={next.EMAIL}'> คลิกที่นี่ </a>
                                    <br/>
                                ";
                            }
                            else
                            {
                                BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายการวัตถุดิบที่นำเข้า / นำมาเปรียบเทียบ  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.VENDORID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {NewRequestByID.DEALER}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {NewRequestByID.PRODUCTIONSITE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestByID.ITEMCODE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {NewRequestByID.ITEMNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)} {NewRequestByID.CURRENCYCODE} / {NewRequestByID.PERUNIT}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/RMAssessmentTodolist?Email={next.EMAIL}'> คลิกที่นี่ </a>
                                    <br/>
                                ";
                            }

                            var sendEmail = _emailService.SendEmail(
                                  $"{NewRequestByID.REQUESTCODE} / ร้องขอเพื่ออนุมัติเอกสารขอประเมินวัตถุดิบ",
                                  BodyEmail,
                                  new List<string> { next.EMAIL },
                                  new List<string> { },
                                  "",
                                  "",
                                  new List<string> { }
                            );

                            if (sendEmail.Result == false)
                            {
                                throw new Exception(sendEmail.Message);
                            }

                            var approveTrans_next = await ApproveTransRepo.GetAsync(next.ID);
                            approveTrans_next.SENDEMAILDATE = DatetimeNow;
                            await ApproveTransRepo.UpdateAsync(approveTrans_next);
                        }

                    }

                    await AssessmentRepo.UpdateAsync(AssessmentBYID);
                    await nonceRepo.UpdateAsync(_nonce);

                    unitOfWork.Complete();
                    AlertSuccess = "Approve Success.";
                    return Redirect($"/S2E/Purchase/RMAssessmentTodolist?Email={email}");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Purchase/RMAssessmentTodolist?Email={email}");
            }
        }
    }
}
