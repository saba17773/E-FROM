using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Purchase.RMAssessment
{
    public class EditModel : PageModel
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
        public List<IFormFile> FileUpload { get; set; }
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
        public List<SelectListItem> ProductGroupMaster { get; set; }
        public List<SelectListItem> ProductSubGroupMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public EditModel(
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
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_RMASSESSMENT));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var EditBy = _authService.GetClaim().UserId;
                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessmentALL = await RMAssessmentRepo.GetAllAsync();
                    var CheckCreateBy = RMAssessmentALL.Where(x => x.ID == AssessmentID &&
                                                                    x.CREATEBY == EditBy).FirstOrDefault();
                    if (CheckCreateBy == null)
                    {
                        AlertError = "ไม่สามารถเข้าไปแก้ไข Request นี้ได้";
                        return Redirect($"/S2E/Purchase/RMAssessment/Main");
                    }

                    DepartmentMaster = await GetDepartmentMaster();
                    ProductGroupMaster = await GetProductGroupMaster();
                    ProductSubGroupMaster = await GetProductSubGroupMaster();

                    AssessmentId = AssessmentID;
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
        public async Task<List<SelectListItem>> GetProductGroupMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var ProductGrpRepo = new GenericRepository<S2EMasterAX_ProductGroup_TB>(unitOfWork.Transaction);

                var ProductGrpALL = await ProductGrpRepo.GetAllAsync();

                return ProductGrpALL
                    .Where(x => x.DATAAREAID == "dv")
                    .Select(x => new SelectListItem
                    {
                        Value = x.DSGPRODUCTGROUPID,
                        Text = x.DSGPRODUCTGROUPID
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetProductSubGroupMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var ProductSubGrpRepo = new GenericRepository<S2EMasterAX_ProductSubGroup_TB>(unitOfWork.Transaction);

                var ProductSubGrpALL = await ProductSubGrpRepo.GetAllAsync();

                return ProductSubGrpALL
                    .Where(x => x.DATAAREAID == "dv")
                    .Select(x => new SelectListItem
                    {
                        Value = x.DSGSUBGROUPID,
                        Text = x.DSGSUBGROUPID
                    })
                    .ToList();
            }
        }
        public async Task<IActionResult> OnGetDepartmentISRemark(int DeptID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var DeptMasterRepo = new GenericRepository<S2EMaster_LABTestDepartment_TB>(unitOfWork.Transaction);
                    var DeptMasterByID = await DeptMasterRepo.GetAsync(DeptID);

                    unitOfWork.Complete();

                    if (DeptMasterByID.ISREMARK == 1)
                    {
                        return new JsonResult(true);
                    }
                    else
                    {
                        return new JsonResult(false);
                    }

                }
            }
            catch (Exception)
            {
                return new JsonResult(false);
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
                    if (docLog.DOCID == 1) { DocRef1 = true; ReasonOfEvaluation = "1"; }
                    if (docLog.DOCID == 2) { DocRef2 = true; ReasonOfEvaluation = "2"; }
                    if (docLog.DOCID == 3) { DocRef3 = true; ReasonOfEvaluation = "3"; }
                    if (docLog.DOCID == 4) { DocRef4 = true; DocRef4_Desc = docLog.REMARK; ReasonOfEvaluation = "4"; }
                    if (docLog.DOCID == 5) { DocRef5 = true; Urgent = "5"; }
                    if (docLog.DOCID == 6) { DocRef6 = true; Urgent = "6"; }
                    if (docLog.DOCID == 7) { DocRef7 = true; Urgent = "7"; }
                    if (docLog.DOCID == 10) { DocRef10 = true; }
                    if (docLog.DOCID == 11) { DocRef11 = true; }
                    if (docLog.DOCID == 12) { DocRef12 = true; }
                    if (docLog.DOCID == 13) { DocRef13 = true; }
                    if (docLog.DOCID == 14) { DocRef14 = true; }
                    if (docLog.DOCID == 15) { DocRef15 = true; }
                    if (docLog.DOCID == 16) { DocRef16 = true; DocRef16_Desc = docLog.REMARK; }
                    if (docLog.DOCID == 17) { DocRef17 = true; Ecotoxic = "17"; }
                    if (docLog.DOCID == 18) { DocRef18 = true; DocRef18_Desc = docLog.REMARK; Ecotoxic = "18"; }
                    if (docLog.DOCID == 19) { DocRef19 = true; Humantoxic = "19"; }
                    if (docLog.DOCID == 20) { DocRef20 = true; DocRef20_Desc = docLog.REMARK; Humantoxic = "20"; }
                    if (docLog.DOCID == 21) { DocRef21 = true; }
                    if (docLog.DOCID == 22) { DocRef22 = true; }
                    if (docLog.DOCID == 23) { DocRef23 = true; }
                    if (docLog.DOCID == 24) { DocRef24 = true; }
                    if (docLog.DOCID == 25) { DocRef25 = true; }
                    if (docLog.DOCID == 26) { DocRef26 = true; }
                    if (docLog.DOCID == 27) { DocRef27 = true; DocRef27_Desc = docLog.REMARK; }
                    if (docLog.DOCID == 28) { DocRef28 = true; RFQ = "28"; }
                    if (docLog.DOCID == 29) { DocRef29 = true; RFQ = "29"; }
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
        public async Task<IActionResult> OnGetDelelteFile(int FileID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RMLogFileRepo = new GenericRepository<S2ERMAssessmentLogsFile_TB>(unitOfWork.Transaction);
                    var RMLogFile = await RMLogFileRepo.GetAsync(FileID);

                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessment = await RMAssessmentRepo.GetAsync(RMLogFile.ASSESSMENTID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RMAssessment.REQUESTID);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/RMAssessment";
                    var fileName = RMLogFile.FILENAME;

                    await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        DELETE FROM TB_S2ERMAssessmentLogsFile 
                        WHERE ID = {FileID}
                    ", null, unitOfWork.Transaction);

                    System.IO.File.Delete($"{filePath}/{fileName}");

                    unitOfWork.Complete();

                    AlertSuccess = "ลบไฟล์สำเร็จ";

                    return new JsonResult(true);

                }
            }
            catch (Exception)
            {
                return new JsonResult(false);
            }

        }
        public async Task<IActionResult> OnPostAsync(int AssessmentID, string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                AssessmentId = AssessmentID;
                await GetData(AssessmentID);

                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var EditDate = DateTime.Now;
                    var EditBy = _authService.GetClaim().UserId;

                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessment = await RMAssessmentRepo.GetAsync(AssessmentID);

                    RequestId = RMAssessment.REQUESTID;
                    

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RMAssessment.REQUESTID);

                    RequestCode = NewRequestByID.REQUESTCODE;
                    isCompaire = NewRequestByID.ISCOMPAIRE;

                    var ApproveStatusAssessment = 0;
                    if (!string.IsNullOrEmpty(draft))
                    {
                        ApproveStatusAssessment = RequestStatusModel.Draft;
                    }
                    if (!string.IsNullOrEmpty(save))
                    {
                        ApproveStatusAssessment = RequestStatusModel.WaitingForApprove;

                    }

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByUpdateBy = approveMapALL.Where(x => x.CreateBy == EditBy &&
                                                                   x.STEP == 1 &&
                                                                   x.ISRMASSESSMENT == 1).FirstOrDefault();
                    var approvemasterid = approveMapByUpdateBy.APPROVEMASTERID;
                    var ApproveGroupID = approveMapByUpdateBy.APPROVEGROUPID;

                    RMAssessment.APPROVEMASTERID = approvemasterid;
                    RMAssessment.REQUESTDATE = EditDate;
                    RMAssessment.PLANT = Plant;
                    RMAssessment.DEPARTMENTID = DepartmentID;
                    RMAssessment.DEPARTMENTREMARK = DepartmentRemark;
                    RMAssessment.MATERIALGROUP = MaterialGroup;
                    RMAssessment.MATERIALNAME = MaterialName;
                    RMAssessment.ITEMGROUP = ItemGroup;
                    RMAssessment.SUPPLIERD1 = SupplierD1;
                    RMAssessment.SUPPLIERD2 = SupplierD2;
                    RMAssessment.SUPPLIERD3 = SupplierD3;
                    RMAssessment.SUPPLIERD4 = SupplierD4;
                    RMAssessment.SUPPLIERD5 = SupplierD5;
                    RMAssessment.SUPPLIERREFD1 = SupplierRefD1;
                    RMAssessment.SUPPLIERREFD2 = SupplierRefD2;
                    RMAssessment.SUPPLIERREFD3 = SupplierRefD3;
                    RMAssessment.SUPPLIERREFD4 = SupplierRefD4;
                    RMAssessment.SUPPLIERREFD5 = SupplierRefD5;
                    RMAssessment.MONTHSAVECOST = MonthSaveCost;
                    RMAssessment.YEARSAVECOST = YearSaveCost;
                    RMAssessment.PLANTCODED1D2 = PlantCodeD1D2;
                    RMAssessment.PLANTCODED3 = PlantCodeD3;
                    RMAssessment.PLANTCODED4 = PlantCodeD4;
                    RMAssessment.PLANTCODED5 = PlantCodeD5;
                    RMAssessment.ISSTARTTEST = isStartTest == 1 ? 1 : 0;
                    RMAssessment.ISSTARTTESTREMARK = isStartTestRemark;
                    RMAssessment.UPDATEBY = EditBy;
                    RMAssessment.UPDATEDATE = EditDate;
                    RMAssessment.APPROVESTATUS = ApproveStatusAssessment;

                    //UPLOAD FILE & INSERT LOG FILE
                    var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                       NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);
                    string basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/RMAssessment";

                    //New File Upload
                    int row = FileUpload.Count();
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }

                    string fileName = "";
                    var S2EAssessmentLogsFileRepo = new GenericRepository<S2ERMAssessmentLogsFile_TB>(unitOfWork.Transaction);
                    for (int i = 0; i < row; i++)
                    {
                        if (FileUpload[i] != null)
                        {
                            fileName = Path.GetFileName(FileUpload[i].FileName);
                            using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                            {
                                await FileUpload[i].CopyToAsync(stream);
                                await S2EAssessmentLogsFileRepo.InsertAsync(new S2ERMAssessmentLogsFile_TB
                                {
                                    ASSESSMENTID = AssessmentID,
                                    FILENAME = fileName,
                                    CREATEBY = EditBy,
                                    CREATEDATE = EditDate
                                });
                            }

                        }
                    }

                    //DELETE OLD LOG DOC
                    using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection()))
                    {
                        var deleteOLDLogsDoc = unitOfWork2.Transaction.Connection.Execute(@"DELETE 
                                FROM TB_S2ERMAssessmentLogsDoc
                                WHERE ASSESSMENTID = @ASSESSMENTID",
                            new
                            {
                                @ASSESSMENTID = AssessmentID
                            },
                            unitOfWork2.Transaction
                        );
                        unitOfWork2.Complete();
                    }


                    //INSERT RM ASSESSMENT LOG DOC
                    var S2EAssessmentLogsDocRepo = new GenericRepository<S2ERMAssessmentLogsDoc_TB>(unitOfWork.Transaction);
                    var S2EAssessmentLogsDocALL = await S2EAssessmentLogsDocRepo.GetAllAsync();
                    var S2EAssessmentMasterRepo = new GenericRepository<S2EMaster_RMAssessDocRef_TB>(unitOfWork.Transaction);
                    var S2EAssessmentMasterALL = await S2EAssessmentMasterRepo.GetAllAsync();

                    int doc1, doc2, doc3, doc4, doc5, doc6, doc7, doc10,
                        doc11, doc12, doc13, doc14, doc15, doc16, doc17, doc18, doc19,
                        doc20, doc21, doc22, doc23, doc24, doc25, doc26, doc27, doc28, doc29;

                    doc1 = DocRef1 == true ? 1 : 0; doc2 = DocRef2 == true ? 2 : 0; doc3 = DocRef3 == true ? 3 : 0;
                    doc5 = DocRef5 == true ? 5 : 0; doc6 = DocRef6 == true ? 6 : 0; doc7 = DocRef7 == true ? 7 : 0;
                    doc10 = DocRef10 == true ? 10 : 0; doc11 = DocRef11 == true ? 11 : 0;
                    doc12 = DocRef12 == true ? 12 : 0; doc13 = DocRef13 == true ? 13 : 0; doc14 = DocRef14 == true ? 14 : 0;
                    doc15 = DocRef15 == true ? 15 : 0; doc17 = DocRef17 == true ? 17 : 0; doc19 = DocRef19 == true ? 19 : 0;
                    doc21 = DocRef21 == true ? 21 : 0; doc22 = DocRef22 == true ? 22 : 0; doc23 = DocRef23 == true ? 23 : 0;
                    doc24 = DocRef24 == true ? 24 : 0; doc25 = DocRef25 == true ? 25 : 0; doc26 = DocRef26 == true ? 26 : 0;
                    doc28 = DocRef28 == true ? 28 : 0; doc29 = DocRef29 == true ? 29 : 0;

                    if (DocRef4 == true)
                    {
                        if (DocRef4_Desc == null || DocRef4_Desc == "")
                        {
                            throw new Exception("ใส่เงื่อนไขการชำระเงินก่อน");
                        }
                        else
                        {
                            doc4 = 4;
                            DocRef4_Desc = DocRef4_Desc;
                        }
                    }
                    else
                    {
                        doc4 = 0;
                        DocRef4_Desc = "";
                    }

                    if (DocRef16 == true)
                    {
                        if (DocRef16_Desc == null || DocRef16_Desc == "")
                        {
                            throw new Exception("ใส่เงื่อนไขการชำระเงินก่อน");
                        }
                        else
                        {
                            doc16 = 16;
                            DocRef16_Desc = DocRef16_Desc;
                        }
                    }
                    else
                    {
                        doc16 = 0;
                        DocRef16_Desc = "";
                    }

                    if (DocRef18 == true)
                    {
                        if (DocRef18_Desc == null || DocRef18_Desc == "")
                        {
                            throw new Exception("ใส่เงื่อนไขการชำระเงินก่อน");
                        }
                        else
                        {
                            doc18 = 18;
                            DocRef18_Desc = DocRef18_Desc;
                        }
                    }
                    else
                    {
                        doc18 = 0;
                        DocRef18_Desc = "";
                    }

                    if (DocRef20 == true)
                    {
                        if (DocRef20_Desc == null || DocRef20_Desc == "")
                        {
                            throw new Exception("ใส่เงื่อนไขการชำระเงินก่อน");
                        }
                        else
                        {
                            doc20 = 20;
                            DocRef20_Desc = DocRef20_Desc;
                        }
                    }
                    else
                    {
                        doc20 = 0;
                        DocRef20_Desc = "";
                    }

                    if (DocRef27 == true)
                    {
                        if (DocRef27_Desc == null || DocRef27_Desc == "")
                        {
                            throw new Exception("ใส่เงื่อนไขการชำระเงินก่อน");
                        }
                        else
                        {
                            doc27 = 27;
                            DocRef27_Desc = DocRef27_Desc;
                        }
                    }
                    else
                    {
                        doc27 = 0;
                        DocRef27_Desc = "";
                    }

                    int[] doc_all = new int[27] { doc1, doc2, doc3, doc4, doc5, doc6, doc7, doc10,
                                                  doc11, doc12, doc13, doc14, doc15, doc16, doc17, doc18, doc19,
                                                  doc20, doc21, doc22, doc23, doc24, doc25, doc26, doc27, doc28, doc29};
                    foreach (int str in doc_all)
                    {
                        if (str > 0)
                        {
                            var DocRef_Name = S2EAssessmentMasterALL.Where(x => x.ID == str)
                                                                    .Select(s => s.DESCRIPTION).FirstOrDefault();
                            var Remark = "";

                            if (str == 4) { Remark = DocRef4_Desc; }
                            //else if (str == 9) { Remark = DocRef9_Desc; }
                            else if (str == 16) { Remark = DocRef16_Desc; }
                            else if (str == 18) { Remark = DocRef18_Desc; }
                            else if (str == 20) { Remark = DocRef20_Desc; }
                            else if (str == 27) { Remark = DocRef27_Desc; }
                            else { Remark = null; }

                            await S2EAssessmentLogsDocRepo.InsertAsync(new S2ERMAssessmentLogsDoc_TB
                            {
                                ASSESSMENTID = AssessmentID,
                                DOCID = str,
                                DOCDESCRIPTION = DocRef_Name,
                                REMARK = Remark,
                                CREATEBY = EditBy,
                                CREATEDATE = EditDate
                            });
                        }
                    }

                    await RMAssessmentRepo.UpdateAsync(RMAssessment);

                    if (!string.IsNullOrEmpty(save))
                    {
                        //UPDATE OLD APPROVE TRANS
                        var ApproveTransOldRepo = new GenericRepository<S2ERMAssessmentApproveTrans_TB>(unitOfWork.Transaction);
                        var ApproveTransOldALL = ApproveTransOldRepo.GetAll();
                        var ApproveTransOld = ApproveTransOldALL.Where(x => x.ASSESSMENTID == AssessmentID && x.APPROVEGROUPID == ApproveGroupID);
                        if (ApproveTransOld.ToList().Count != 0)
                        {
                            foreach (var App in ApproveTransOld)
                            {
                                var AppTransOldUpdate = await ApproveTransOldRepo.GetAsync(App.ID);
                                AppTransOldUpdate.ISCURRENTAPPROVE = 0;
                                await ApproveTransOldRepo.UpdateAsync(AppTransOldUpdate);
                            }
                        }

                        //GET APPROVE FLOW BY APPROVE MASTER ID
                        var appoveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                        var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                        var approveFlow_data = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                        x.ApproveLevel != 0 &&
                                                                        x.IsActive == 1
                                                                   ).OrderBy(o => o.ApproveLevel);

                        // GENERATE NONCE
                        var nonceKey = Guid.NewGuid().ToString();
                        var nonceRepo = new GenericRepository<S2ERMAssessmentNonce_TB>(unitOfWork.Transaction);
                        await nonceRepo.InsertAsync(new S2ERMAssessmentNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = EditDate,
                            EXPIREDATE = EditDate.AddDays(7),
                            ISUSED = 0
                        });

                        // INSERT PC APPROVE TRANSECTION
                        var RMAssessmentAppTranRepo = new GenericRepository<S2ERMAssessmentApproveTrans_TB>(unitOfWork.Transaction);
                        foreach (var AppFlow in approveFlow_data)
                        {
                            await RMAssessmentAppTranRepo.InsertAsync(new S2ERMAssessmentApproveTrans_TB
                            {
                                ASSESSMENTID = AssessmentID,
                                APPROVEMASTERID = AppFlow.ApproveMasterId,
                                APPROVEGROUPID = ApproveGroupID,
                                EMAIL = AppFlow.Email,
                                APPROVELEVEL = AppFlow.ApproveLevel,
                                ISCURRENTAPPROVE = 1,
                                ISKEYINWHENAPPROVE = AppFlow.IsKeyinWhenApprove
                            });
                        }

                        var currentRecord = await RMAssessmentRepo.GetAsync(AssessmentID);
                        currentRecord.CURRENTAPPROVESTEP = 1;
                        await RMAssessmentRepo.UpdateAsync(currentRecord);

                        //GET APPROVE TRANS LEVEL 1
                        var AppTransByRequestID = await unitOfWork.S2EControl.GetApproveTransByAssessmentID(AssessmentID, approvemasterid, ApproveGroupID);
                        var AppTransLevel1 = AppTransByRequestID.Where(x => x.APPROVELEVEL == 1);
                        foreach (var AppTrans in AppTransLevel1)
                        {
                            var approveFlowApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                       x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                       x.Email == AppTrans.EMAIL);

                            var FName = approveFlowApproveBy.Select(s => s.Name).FirstOrDefault();
                            var LName = approveFlowApproveBy.Select(s => s.LastName).FirstOrDefault();
                            var ApproveBy = FName + " " + LName;

                            var AppTransByALL = await RMAssessmentAppTranRepo.GetAllAsync();
                            var AppTransByID = AppTransByALL.Where(x => x.ID == AppTrans.ID).FirstOrDefault();

                            AppTransByID.SENDEMAILDATE = EditDate;
                            await RMAssessmentAppTranRepo.UpdateAsync(AppTransByID);

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
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/RMAssessmentTodolist?Email={AppTrans.EMAIL}'> คลิกที่นี่ </a>
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
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/RMAssessmentTodolist?Email={AppTrans.EMAIL}'> คลิกที่นี่ </a>
                                    <br/>
                                ";
                            }

                            var sendEmail = _emailService.SendEmail(
                                  $"{NewRequestByID.REQUESTCODE} / ร้องขอเพื่ออนุมัติเอกสารขอประเมินวัตถุดิบ",
                                  BodyEmail,
                                  new List<string> { AppTrans.EMAIL },
                                  new List<string> { },
                                  "",
                                  "",
                                  new List<string> { }
                            );

                            if (sendEmail.Result == false)
                            {
                                throw new Exception(sendEmail.Message);
                            }
                        }


                    }

                    unitOfWork.Complete();
                    AlertSuccess = "แก้ไขข้อมูลใบขอประเมินวัตถุดิบสำเร็จ";
                    return Redirect($"/S2E/Purchase/RMAssessment/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Purchase/RMAssessment/{AssessmentID}/Edit");
            }
        }
    }
}
