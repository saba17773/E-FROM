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
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E
{
    public class LABTestViewInfoModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int RequestId { get; set; }
        [BindProperty]
        public int AssessmentId { get; set; }
        [BindProperty]
        public string ItemGroup { get; set; }
        [BindProperty]
        public int DepartmentID { get; set; }
        [BindProperty]
        public string DepartmentRemark { get; set; }
        public List<SelectListItem> DepartmentMaster { get; set; }
        [BindProperty]
        public int ResonTestID { get; set; }
        [BindProperty]
        public string ResonTestRemark { get; set; }
        public List<SelectListItem> ResonTestMaster { get; set; }
        [BindProperty]
        public int TypeOfRMID { get; set; }
        public List<SelectListItem> TypeOfRMMaster { get; set; }
        [BindProperty]
        public string ChemicalNameRef { get; set; }
        [BindProperty]
        public string ItemCodeRef { get; set; }
        [BindProperty]
        public string TradeNameRef { get; set; }
        [BindProperty]
        public string CountryRef { get; set; }
        [BindProperty]
        public string ManufactureRef { get; set; }
        [BindProperty]
        public string AgentRef { get; set; }
        [BindProperty]
        public string ChemicalName { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string TradeName { get; set; }
        [BindProperty]
        public string Country { get; set; }
        [BindProperty]
        public string Manufacture { get; set; }
        [BindProperty]
        public string Agent { get; set; }
        [BindProperty]
        public string PlantCodeD1D2Ref { get; set; }
        [BindProperty]
        public string PlantCodeD3Ref { get; set; }
        [BindProperty]
        public string PlantCodeD4Ref { get; set; }
        [BindProperty]
        public string PlantCodeD5Ref { get; set; }
        [BindProperty]
        public int TestResult { get; set; }
        [BindProperty]
        public int ApprovalForID { get; set; }
        public List<SelectListItem> ApprovalForIDMaster { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public bool Plant1 { get; set; }
        [BindProperty]
        public bool Plant2 { get; set; }
        [BindProperty]
        public bool Plant3 { get; set; }
        [BindProperty]
        public bool Plant4 { get; set; }
        [BindProperty]
        public bool Plant5 { get; set; }
        [BindProperty]
        public string PlantCodeD1D2 { get; set; }
        [BindProperty]
        public string PlantCodeD3 { get; set; }
        [BindProperty]
        public string PlantCodeD4 { get; set; }
        [BindProperty]
        public string PlantCodeD5 { get; set; }
        [BindProperty]
        public bool DocRef1 { get; set; }
        [BindProperty]
        public bool DocRef2 { get; set; }
        [BindProperty]
        public bool DocRef3 { get; set; }
        [BindProperty]
        public bool DocRef4 { get; set; }
        [BindProperty]
        public bool DocRef5 { get; set; }
        [BindProperty]
        public bool DocRef6 { get; set; }
        [BindProperty]
        public bool DocRef7 { get; set; }
        [BindProperty]
        public bool DocRef8 { get; set; }
        [BindProperty]
        public bool DocRef9 { get; set; }
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
        public bool DocRef17 { get; set; }
        [BindProperty]
        public bool DocRef18 { get; set; }
        [BindProperty]
        public bool DocRef19 { get; set; }
        [BindProperty]
        public bool DocRef20 { get; set; }
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
        public bool DocRef28 { get; set; }
        [BindProperty]
        public bool DocRef29 { get; set; }
        [BindProperty]
        public List<int> LabIsPass { get; set; }
        [BindProperty]
        public List<int> ProcIsPass { get; set; }
        [BindProperty]
        public List<string> ProdList { get; set; }
        [BindProperty]
        public List<string> ProdIsPass { get; set; }
        [BindProperty]
        public string LABTestResultRemark1 { get; set; }
        [BindProperty]
        public string LABTestResultRemark2 { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public List<int> LabResultID { get; set; }
        [BindProperty]
        public List<int> ProcResultID { get; set; }
        [BindProperty]
        public int isCompaire { get; set; }
        [BindProperty]
        public int LabID { get; set; }
        [BindProperty]
        public string ProjectRefNo { get; set; }
        [BindProperty]
        public List<string> LabResultLogsID { get; set; }
        [BindProperty]
        public List<string> ProcResultLogsID { get; set; }
        [BindProperty]
        public List<string> ProdResultLogsID { get; set; }
        [BindProperty]
        public List<int> ProdIsPassOLD { get; set; }
        [BindProperty]
        public int ApproveResult { get; set; }
        [BindProperty]
        public string ApproveRemark { get; set; }
        [BindProperty]
        public decimal QtyApprove { get; set; }
        [BindProperty]
        public string UnitApprove { get; set; }
        [BindProperty]
        public int isPurchaseSample { get; set; }
        public List<SelectListItem> UnitApproveMaster { get; set; }
        [BindProperty]
        public string EmailApprove { get; set; }
        [BindProperty]
        public int LabLineID { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public LABTestViewInfoModel(
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
        public async Task<IActionResult> OnGetAsync(int LABID, int LABLINEID, string EMAILAPPROVE)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection())) 
                {
                    var LABTestLogsSendEmailRepo = new GenericRepository<S2ELABTestLogsSendEmailSuccess_TB>(unitOfWork.Transaction);
                    var LABTestLogsSendEmailALL = await LABTestLogsSendEmailRepo.GetAllAsync();
                    var CheckView = LABTestLogsSendEmailALL.Where(x => x.LABID == LABID && x.LABLINEID == LABLINEID && x.EMAIL == EMAILAPPROVE).FirstOrDefault();
                    if (CheckView == null)
                    {
                        AlertError = "ไม่สามารถดูข้อมูลได้ เนื่องจากไม่มีสิทธิ์การเข้าถึง";
                        return Redirect($"/S2E/Index");
                    }

                    LabID = LABID;
                    LabLineID = LABLINEID;
                    EmailApprove = EMAILAPPROVE;

                    DepartmentMaster = await GetDepartmentMaster();
                    ResonTestMaster = await GetResonTestMaster();
                    TypeOfRMMaster = await GetTypeOfRMMaster();
                    UnitApproveMaster = await GetUnitApproveMaster();

                    await GetData(LABID, LABLINEID);

                    return Page();

                }
                    
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<SelectListItem>> GetResonTestMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var ResonRepo = new GenericRepository<S2EMaster_LABTestResonTest_TB>(unitOfWork.Transaction);

                var ResonALL = await ResonRepo.GetAllAsync();

                return ResonALL
                    .Where(x => x.ISACTIVE == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.RESONTESTDESC
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetTypeOfRMMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var TypeOfRMMaster = new GenericRepository<S2EMaster_LABTestTypeOfRM_TB>(unitOfWork.Transaction);

                var TypeOfRMALL = await TypeOfRMMaster.GetAllAsync();

                return TypeOfRMALL
                    .Where(x => x.ISACTIVE == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.TYPEOFRMDESC
                    })
                    .ToList();
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
                    //.OrderBy(x1 => x1.LastOption).ThenBy(x2 => x2.ID)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DEPARTMENTDESC
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetUnitApproveMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var UnitRepo = new GenericRepository<Unit_TB>(unitOfWork.Transaction);

                var UnitALL = await UnitRepo.GetAllAsync();

                return UnitALL.Select(x => new SelectListItem
                {
                    Value = x.UNIT,
                    Text = x.UNIT
                })
                    .ToList();
            }
        }
        public async Task GetData(int LABID, int LABLINEID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                var LABTestLineByID = await LABTestLineRepo.GetAsync(LABLINEID);

                var RequestID = LABTestHeadByID.REQUESTID;
                var AssessmentID = LABTestHeadByID.ASSESSMENTID;

                ItemGroup = LABTestHeadByID.ITEMGROUP;

                ChemicalName = LABTestLineByID.CHEMICALNAME;
                ItemCode = LABTestLineByID.ITEMCODE;
                TradeName = LABTestLineByID.TRADENAME;
                Country = LABTestLineByID.COUNTRY;
                Manufacture = LABTestLineByID.MANUFACTURE;
                Agent = LABTestLineByID.AGENT;
                PlantCodeD1D2 = LABTestLineByID.PLANTCODED1D2;
                PlantCodeD3 = LABTestLineByID.PLANTCODED3;
                PlantCodeD4 = LABTestLineByID.PLANTCODED4;
                PlantCodeD5 = LABTestLineByID.PLANTCODED5;
                ProjectRefNo = LABTestLineByID.PROJECTREFNO;
                DepartmentID = LABTestLineByID.DEPARTMENTID;
                DepartmentRemark = LABTestLineByID.DEPARTMENTREMARK;
                ResonTestID = LABTestLineByID.RESONTESTID;
                ResonTestRemark = LABTestLineByID.RESONTESTREMARK;
                TypeOfRMID = LABTestLineByID.TYPEOFRMID;
                TestResult = LABTestLineByID.TESTRESULT == 1 ? 1 : 2;
                if (LABTestLineByID.APPROVALFORID == 1)
                {
                    ApprovalForID = 1;
                }
                else if (LABTestLineByID.APPROVALFORID == 2)
                {
                    ApprovalForID = 2;
                }


                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;
                isCompaire = NewRequestByID.ISCOMPAIRE;

                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                Plant = LABTestLineByID.PLANT;
                if (LABTestLineByID.PLANT != null)
                {
                    string[] PlantList = LABTestLineByID.PLANT.Split(",");
                    foreach (var d in PlantList)
                    {
                        if (d == "DSL") { Plant1 = true; }
                        if (d == "DRB") { Plant2 = true; }
                        if (d == "DSI") { Plant3 = true; }
                        if (d == "DSR") { Plant4 = true; }
                        if (d == "STR") { Plant5 = true; }
                    }
                }

                UnitApprove = NewRequestByID.UNIT;
                isPurchaseSample = LABTestLineByID.ISPURCHASESAMPLE == 1 ? 1 : 2;
                QtyApprove = LABTestLineByID.QTY;

                if (NewRequestByID.ISCOMPAIRE == 1)
                {
                    var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                    var NewRequestCompaireALL = await NewRequestCompaireRepo.GetAllAsync();
                    var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == RequestID).FirstOrDefault();

                    ItemCodeRef = NewRequestCompaireByRequestID.ITEMCODEREF;
                    TradeNameRef = NewRequestCompaireByRequestID.ITEMNAMEREF;
                    CountryRef = NewRequestCompaireByRequestID.PRODUCTIONSITEREF;
                    ManufactureRef = NewRequestCompaireByRequestID.SUPPLIERNAMEREF;
                    AgentRef = NewRequestCompaireByRequestID.DEALERREF;

                    ChemicalNameRef = LABTestLineByID.CHEMICALNAMEREF;
                    PlantCodeD1D2Ref = LABTestLineByID.PLANTCODED1D2REF;
                    PlantCodeD3Ref = LABTestLineByID.PLANTCODED3REF;
                    PlantCodeD4Ref = LABTestLineByID.PLANTCODED4REF;
                    PlantCodeD5Ref = LABTestLineByID.PLANTCODED5REF;

                }

                //Doc Ref
                var LABTestLogDocRepo = new GenericRepository<S2ELABTestLogsDoc_TB>(unitOfWork.Transaction);
                var LABTestLogDocALL = await LABTestLogDocRepo.GetAllAsync();
                foreach (var docLog in LABTestLogDocALL.Where(x => x.LABID == LABID && x.LABLINEID == LABLINEID))
                {
                    if (docLog.DOCID == 1) { DocRef1 = true; }
                    if (docLog.DOCID == 2) { DocRef2 = true; }
                    if (docLog.DOCID == 3) { DocRef3 = true; }
                    if (docLog.DOCID == 4) { DocRef4 = true; }
                    if (docLog.DOCID == 5) { DocRef5 = true; }
                    if (docLog.DOCID == 6) { DocRef6 = true; }
                    if (docLog.DOCID == 7) { DocRef7 = true; }
                    if (docLog.DOCID == 8) { DocRef8 = true; }
                    if (docLog.DOCID == 9) { DocRef9 = true; }
                    if (docLog.DOCID == 10) { DocRef10 = true; }
                    if (docLog.DOCID == 11) { DocRef11 = true; }
                    if (docLog.DOCID == 12) { DocRef12 = true; }
                    if (docLog.DOCID == 13) { DocRef13 = true; }
                    if (docLog.DOCID == 14) { DocRef14 = true; }
                    if (docLog.DOCID == 15) { DocRef15 = true; }
                    if (docLog.DOCID == 16) { DocRef16 = true; }
                    if (docLog.DOCID == 17) { DocRef17 = true; }
                    if (docLog.DOCID == 18) { DocRef18 = true; }
                }

                unitOfWork.Complete();
            }
        }
        public async Task<JsonResult> OnPostLabEvaluationGridAsync(int LABID, int LABLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<LabTestLogTestResultGridViewModel>(@"
                        SELECT *
                        FROM
                        (
	                        SELECT L.LABID,L.LABEVALUATIONDESC,L.ISPASS,
		                        L.REMARK1 AS REMARKA ,
		                        L.REMARK2 AS REMARKB ,
		                        M.ISREMARK1 AS ISREMARKA ,
		                        M.ISREMARK2 AS ISREMARKB
	                        FROM TB_S2ELABTestLogsTestResult L JOIN
	                        TB_S2EMaster_LABTestEvaluation M ON L.LABEVALUATIONID = M.ID
	                        WHERE L.LABID = @LABID AND L.LABLINEID = @LABLINEID
                        )T",
                        new
                        {
                            @LABID = LABID,
                            @LABLINEID = LABLINEID
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
        public async Task<JsonResult> OnPostProcessEvaluationGridAsync(int LABID, int LABLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<LabTestLogProcTestResultGridViewModel>(@"
                        SELECT *
                        FROM
                        (
	                        SELECT LABID,PROCESSID,
                            PROCESSDESC,ISPASS
                            FROM TB_S2ELABTestLogsProcTestResult 
                            WHERE LABID = @LABID AND LABLINEID = @LABLINEID
                        )T",
                        new
                        {
                            @LABID = LABID,
                            @LABLINEID = LABLINEID
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
        public async Task<JsonResult> OnPostProductEvaluationGridAsync(int LABID, int LABLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<LabTestLogProdTestResultGridViewModel>(@"
                        SELECT *
                        FROM
                        (
	                        SELECT LABID,PRODUCTTESTDESC,ISPASS
                            FROM TB_S2ELABTestLogsPrdTestResult 
                            WHERE LABID = @LABID AND LABLINEID = @LABLINEID
                        )T",
                        new
                        {
                            @LABID = LABID,
                            @LABLINEID = LABLINEID
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
        public async Task<IActionResult> OnPostGridViewFileUploadAsync(int LABID, int LABLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ELABTestLogsFileGridViewModel>($@"
                        SELECT ID,
		                    LABID,
		                    FILENAME,
                            CREATEBY,
		                    CONVERT(NVARCHAR,CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,CREATEDATE,108) AS CREATEDATE
                        FROM TB_S2ELABTestLogsFile 
                        WHERE LABID = {LABID}  AND LABLINEID = {LABLINEID}
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
        public async Task<IActionResult> OnGetDownloadFileUploadAsync(int LABID, int LABLINEID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var LABTestLogFileRepo = new GenericRepository<S2ELABTestLogsFile_TB>(unitOfWork.Transaction);
                    var LABTestLogFileByID = await LABTestLogFileRepo.GetAsync(Fileid);

                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LABTestLineByID = await LABTestLineRepo.GetAsync(LABLINEID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(LABTestHeadByID.REQUESTID);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/LABTest/" + LABLINEID;

                    var fileName = LABTestLogFileByID.FILENAME;

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

    }
}
