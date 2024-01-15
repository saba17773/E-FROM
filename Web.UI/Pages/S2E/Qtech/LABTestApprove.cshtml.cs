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

namespace Web.UI.Pages.S2E.Qtech
{
    public class LABTestApproveModel : PageModel
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
        public int isKeyin { get; set; }
        [BindProperty]
        [Required]
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
        public int LabLineID { get; set; }
        [BindProperty]
        public string PurchaseRemark { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public LABTestApproveModel(
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
        public async Task<IActionResult> OnGetAsync(int LABID, int LABLINEID, int TranID, string nonce, string email, int isKeyinWhenApprove)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var _nonce = await unitOfWork.S2EControl.GetNonceLABTestByKey(nonce);

                    if (_nonce.ISUSED == 1)
                    {
                        AlertError = "Link Is Used.";
                        return Redirect($"/");
                    }

                    LabID = LABID;
                    LabLineID = LABLINEID;
                    isKeyin = isKeyinWhenApprove;

                    DepartmentMaster = await GetDepartmentMaster();
                    ResonTestMaster = await GetResonTestMaster();
                    TypeOfRMMaster = await GetTypeOfRMMaster();
                    UnitApproveMaster = await GetUnitApproveMaster();

                    await GetData(LABID, LABLINEID);

                    if (isKeyin == 0)
                    {
                        await GetQty(LABID,LABLINEID);
                    }

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
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DEPARTMENTDESC
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
                UnitApprove = NewRequestByID.UNIT;

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
        public async Task GetQty(int LABID, int LABLINEID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                var LABTestLineByID = await LABTestLineRepo.GetAsync(LABLINEID);


                QtyApprove = LABTestLineByID.QTY;
                isPurchaseSample = LABTestLineByID.ISPURCHASESAMPLE == 1 ? 1 : 2;
                PurchaseRemark = LABTestLineByID.PurchaseRemark;

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
        public async Task<IActionResult> OnPostGridViewApproveAsync(int LABID, int LABLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<GetApproveLogsGridViewModel>($@"
                       SELECT EMAIL,
                        CONVERT(NVARCHAR,APPROVEDATE,103) + ' ' + CONVERT(NVARCHAR,APPROVEDATE,108) AS APPROVEDATE
                        ,REMARK
                    FROM TB_S2ELABTestApproveTrans
                    WHERE LABID = {LABID} AND LABLINEID = {LABLINEID} AND ISCURRENTAPPROVE = 1
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
        public async Task<IActionResult> OnPostAsync(int LABID, int LABLINEID, int TranID, string nonce, string email, int isKeyinWhenApprove)
        {
            try
            {
                if (ApproveResult == 0)
                {
                    AlertError = "กรุณาเลือกว่าจะอนุมัติ หรือ ไม่อนุมัติ !!";
                    return Redirect($"/S2E/Qtech/LABTestApprove?LABID={LABID}&LABLINEID={LABLINEID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");
                }
                if (ApproveResult == 2 && ApproveRemark == null)
                {
                    AlertError = "กรุณาใส่เหตุผลที่ต้องการ Reject !!";
                    return Redirect($"/S2E/Qtech/LABTestApprove?LABID={LABID}&LABLINEID={LABLINEID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");
                }
                
                if (!ModelState.IsValid)
                {
                    LabID = LABID;
                    LabLineID = LABLINEID;
                    isKeyin = isKeyinWhenApprove;

                    DepartmentMaster = await GetDepartmentMaster();
                    ResonTestMaster = await GetResonTestMaster();
                    TypeOfRMMaster = await GetTypeOfRMMaster();
                    UnitApproveMaster = await GetUnitApproveMaster();

                    await GetData(LABID, LABLINEID);

                    if (isKeyin == 0)
                    {
                        await GetQty(LABID, LABLINEID);
                    }
                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LABTestLineByID = await LABTestLineRepo.GetAsync(LABLINEID);

                    if (ApproveResult == 1 && isKeyinWhenApprove == 1 && LABTestLineByID.TESTRESULT == 1)
                    {
                        if (isPurchaseSample == 0)
                        {
                            AlertError = "กรุณาเลือกว่าต้องการซื้อวัตถุดิบเพื่อทดสอบเพิ่มหรือไม่ ? !!";
                            return Redirect($"/S2E/Qtech/LABTestApprove?LABID={LABID}&LABLINEID={LABLINEID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");
                        }

                        if (isPurchaseSample == 1 && (QtyApprove == 0))
                        {
                            AlertError = "กรุณาใส่จำนวนที่ต้องการซื้อเพิ่ม !!";
                            return Redirect($"/S2E/Qtech/LABTestApprove?LABID={LABID}&LABLINEID={LABLINEID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");
                        }
                    }

                    var RequestID = LABTestHeadByID.REQUESTID;
                    var AssessmentID = LABTestHeadByID.ASSESSMENTID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var DatetimeNow = DateTime.Now;
                    int approvemasterid = LABTestLineByID.APPROVEMASTERID;

                    //UPDATE OLD DATA
                    var nonceRepo = new GenericRepository<S2ELABTestNonce_TB>(unitOfWork.Transaction);
                    var _nonce = await unitOfWork.S2EControl.GetNonceLABTestByKey(nonce);
                    if (_nonce.ISUSED == 1)
                    {
                        throw new Exception("Link Is Used.");
                    }
                    _nonce.ISUSED = 1;

                    //UPDATE Approve Trans
                    var LABTestTransRepo = new GenericRepository<S2ELABTestApproveTrans_TB>(unitOfWork.Transaction);
                    var LABTestTransByID = await LABTestTransRepo.GetAsync(TranID);

                    var ApproveLevel = LABTestTransByID.APPROVELEVEL;
                    var ApproveGroupID = LABTestTransByID.APPROVEGROUPID;

                    var LABTestApproveTransRepo = new GenericRepository<S2ELABTestApproveTrans_TB>(unitOfWork.Transaction);
                    var LABTestApproveTransALL = await LABTestApproveTransRepo.GetAllAsync();
                    var LABTestApproveTranLevel = LABTestApproveTransALL.Where(x => x.LABID == LABID &&
                                                                    x.LABLINEID == LABLINEID &&
                                                                    x.APPROVEMASTERID == approvemasterid &&
                                                                    x.APPROVELEVEL == ApproveLevel &&
                                                                    x.ISCURRENTAPPROVE == 1 &&
                                                                    x.APPROVEGROUPID == ApproveGroupID);

                    foreach (var UpdateApproveTrans in LABTestApproveTranLevel)
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
                        await LABTestApproveTransRepo.UpdateAsync(UpdateApproveTrans);
                    }

                    //GET REQUEST BY FULL NAME
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(LABTestLineByID.CREATEBY);

                    //GET APPROVE TRANS ALL LEVEL
                    var ApproveTransAll = await unitOfWork.S2EControl.GetApproveTransByLABIDAllLevel(LABID, LABLINEID, approvemasterid, ApproveGroupID);
                    var AllLevel = ApproveTransAll.ToList().Count;

                    // CHECK IS FINAL APPROVE?
                    var Subject = "";
                    var Body = "";
                    var PurchaseSampleDetail = "";

                    if (LABTestLineByID.TESTRESULT == 1)
                    {
                        Subject = "ผ่านการทดสอบ";
                        Body = "<b style='color:green'> ผ่านการทดสอบ </b>";
                        PurchaseSampleDetail = $"<b>ต้องการวัตถุดิบเพิ่มเพื่อทดสอบ : </b> {String.Format("{0:#,##0.#0}", QtyApprove)}  {NewRequestByID.UNIT}";
                    }
                    else
                    {
                        Subject = "ไม่ผ่านการทดสอบ";
                        Body = "<b style='color:red'> ไม่ผ่านการทดสอบ </b>";
                        PurchaseSampleDetail = $"";
                    }
                    //isFinal
                    if (LABTestLineByID.CURRENTAPPROVESTEP == AllLevel && ApproveResult == 1)
                    {
                        if (LABTestLineByID.COMPLETEDATE == null)
                        {

                            if (LABTestLineByID.TESTRESULT == 1)
                            {
                                LABTestLineByID.APPROVESTATUS = RequestStatusModel.Complete;
                                LABTestLineByID.COMPLETEDATE = DatetimeNow;
                            }
                            else
                            {
                                LABTestLineByID.APPROVESTATUS = RequestStatusModel.isNotPass;
                            }

                            //UPDATE (HEAD TABLE)
                            if (LABTestLineByID.ISPURCHASESAMPLE == 0)
                            {
                                //update ispurchase sample remark
                                LABTestLineByID.ISPURCHASESAMPLE = isPurchaseSample == 1 ? 1 : 0;
                                LABTestLineByID.PurchaseRemark = PurchaseRemark;

                            }
                            if (LABTestLineByID.QTY == 0)
                            {
                                LABTestLineByID.QTY = QtyApprove;
                            }

                            //GET EMAIL SUCCESSFULLY
                            //var EmailSuccess = new List<string>();
                            var ApproveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                            var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();

                            var ApproveFlowReceiveComplete = ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid &&
                                                                                  x.ReceiveWhenComplete == 1 && x.IsActive == 1);

                            var LabTestLogsSendEmailSuccessRepo = new GenericRepository<S2ELABTestLogsSendEmailSuccess_TB>(unitOfWork.Transaction);
                            var LabTestLogsSendEmailSuccessALL = await LabTestLogsSendEmailSuccessRepo.GetAllAsync();

                            var BodyEmail = "";
                            //CASE WHEN SET IN FLOW MASTER
                            if (ApproveFlowReceiveComplete != null)
                            {
                                foreach (var emaillog in ApproveFlowReceiveComplete)
                                {
                                    // insert to logs send mail
                                    await LabTestLogsSendEmailSuccessRepo.InsertAsync(new S2ELABTestLogsSendEmailSuccess_TB
                                    {
                                        LABID = (int)LABID,
                                        LABLINEID = LABLINEID,
                                        EMAIL = emaillog.Email,
                                        APPROVELEVEL = emaillog.ApproveLevel,
                                        SENDEMAILDATE = DatetimeNow,
                                        ISCREATOR = 0
                                    });
                                    
                                    //send mail
                                    if (NewRequestByID.ISCOMPAIRE == 1)
                                    {
                                        var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                                        var NewRequestCompaireALL = await NewRequestCompaireRepo.GetAllAsync();
                                        var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == RequestID).FirstOrDefault();

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
                                            <b>ผลการทดสอบ : </b> {Body}
                                            <br/>
                                            {PurchaseSampleDetail}
                                            <br/><br/> 
                                            <a href='{_configuration["Config:BaseUrl"]}/S2E/LABTestViewInfo/?LABID={LABID}&LABLINEID={LABLINEID}&EMAILAPPROVE={emaillog.Email}'> คลิกที่นี่ เพื่อดูรายละเอียด </a>
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
                                            <b>ผลการทดสอบ : </b> {Body}
                                            <br/>
                                            {PurchaseSampleDetail}
                                            <br/><br/> 
                                            <a href='{_configuration["Config:BaseUrl"]}/S2E/LABTestViewInfo/?LABID={LABID}&LABLINEID={LABLINEID}&EMAILAPPROVE={emaillog.Email}'> คลิกที่นี่ เพื่อดูรายละเอียด </a>
                                        ";
                                    }

                                    var sendEmailFlow = _emailService.SendEmail(
                                          $"{NewRequestByID.REQUESTCODE} / แจ้งผลการทดสอบวัตถุดิบ Lab / {Subject}",
                                          BodyEmail,
                                          new List<string> { emaillog.Email },
                                          new List<string> { },
                                          "",
                                          "",
                                          new List<string> { }
                                    );

                                    if (sendEmailFlow.Result == false)
                                    {
                                        throw new Exception(sendEmailFlow.Message);
                                    }

                                }
                            }

                            //LAB TEST Logs Send Email Success Email Requester
                            await LabTestLogsSendEmailSuccessRepo.InsertAsync(new S2ELABTestLogsSendEmailSuccess_TB
                            {
                                LABID = (int)LABID,
                                LABLINEID = LABLINEID,
                                EMAIL = UserALL.Email,
                                APPROVELEVEL = 0,
                                SENDEMAILDATE = DatetimeNow,
                                ISCREATOR = 1
                            });

                            //send mail Requester
                            if (NewRequestByID.ISCOMPAIRE == 1)
                            {
                                var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                                var NewRequestCompaireALL = await NewRequestCompaireRepo.GetAllAsync();
                                var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == RequestID).FirstOrDefault();

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
                                    <b>ผลการทดสอบ : </b> {Body}
                                    <br/>
                                    {PurchaseSampleDetail}
                                    <br/><br/> 
                                    <a href='{_configuration["Config:BaseUrl"]}/S2E/LABTestViewInfo/?LABID={LABID}&LABLINEID={LABLINEID}&EMAILAPPROVE={UserALL.Email}'> คลิกที่นี่ เพื่อดูรายละเอียด </a>
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
                                    <b>ผลการทดสอบ : </b> {Body}
                                    <br/>
                                    {PurchaseSampleDetail}
                                    <br/><br/> 
                                    <a href='{_configuration["Config:BaseUrl"]}/S2E/LABTestViewInfo/?LABID={LABID}&LABLINEID={LABLINEID}&EMAILAPPROVE={UserALL.Email}'> คลิกที่นี่ เพื่อดูรายละเอียด </a>
                                ";
                            }

                            var sendEmailRequester = _emailService.SendEmail(
                                  $"{NewRequestByID.REQUESTCODE} / แจ้งผลการทดสอบวัตถุดิบ Lab / {Subject}",
                                  BodyEmail,
                                  new List<string> { UserALL.Email },
                                  new List<string> { },
                                  "",
                                  "",
                                  new List<string> { }
                            );

                            if (sendEmailRequester.Result == false)
                            {
                                throw new Exception(sendEmailRequester.Message);
                            }


                        }
                    }
                    //isReject or More Detail
                    else if ((ApproveResult == 2 && ApproveRemark != null))
                    {

                        //UPDATE PCREQUEST_TB (HEAD TABLE)
                        LABTestLineByID.APPROVESTATUS = RequestStatusModel.Reject;
                        LABTestLineByID.ISPURCHASESAMPLE = 0;
                        LABTestLineByID.PurchaseRemark = "";
                        LABTestLineByID.QTY = 0;
                        Subject = $"{NewRequestByID.REQUESTCODE} / Reject / แจ้งผลการทดสอบวัตถุดิบ (Lab Test) / {Subject} ";

                        //GET EMAIL REJECT
                        var EmailReject = new List<string>();
                        //CASE SET IN FLOW
                        var ApproveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                        var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();
                        //foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenFailed == 1 && x.IsActive == 1 && x.ApproveLevel < PCRequestBYID.CURRENTAPPROVESTEP))
                        //{
                        //    EmaiReject.Add(emaillog.Email);
                        //}
                        EmailReject.Add(UserALL.Email);

                        //GET Reject BY
                        var approveFlowNameALL = ApproveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                        x.ApproveLevel == LABTestLineByID.CURRENTAPPROVESTEP &&
                                                                        x.IsActive == 1);

                        var RejectByFirstName = approveFlowNameALL.Select(s => s.Name).FirstOrDefault();
                        var RejectByLastName = approveFlowNameALL.Select(s => s.LastName).FirstOrDefault();
                        var RejectBy = RejectByFirstName + " " + RejectByLastName;

                        var BodyEmail = "";
                        if (LABTestLineByID.TESTRESULT == 1)
                        {
                            BodyEmail = $@"
                                    <b> REQUEST NO : {NewRequestByID.REQUESTCODE}  </b><br/>
                                    <b> สถานะ : </b><b style='color:red'> ไม่ผ่านการอนุมัติ </b>    <br/>
                                    <b style='color:black'> สาเหตุที่ไม่อนุมัติ : </b> {ApproveRemark} <br/>
                                    <b> Reject By : </b>{RejectBy}
                                ";
                        }
                        else
                        {
                            BodyEmail = $@"
                                    <b> REQUEST NO : {NewRequestByID.REQUESTCODE}  </b><br/>
                                    <b> PROJECT REF. NO : {LABTestLineByID.PROJECTREFNO}  </b><br/>
                                    <b> สถานะ : </b><b style='color:red'> ไม่ผ่านการอนุมัติ </b>    <br/>
                                    <b style='color:black'> สาเหตุที่ไม่อนุมัติ : </b> {ApproveRemark} <br/>
                                    <b> Reject By : </b>{RejectBy}
                                ";
                        }
                        

                        var sendEmail = _emailService.SendEmail(
                            $"{Subject}",
                            BodyEmail,
                            EmailReject,
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
                    //isApprove And Not Final Approve
                    else
                    {
                        //UPDATE PCREQUEST_TB (HEAD TABLE)
                        LABTestLineByID.CURRENTAPPROVESTEP += 1;
                        LABTestLineByID.APPROVESTATUS = RequestStatusModel.WaitingForApprove;

                        if (LABTestLineByID.ISPURCHASESAMPLE == 0)
                        {
                            LABTestLineByID.ISPURCHASESAMPLE = isPurchaseSample == 1 ? 1 : 0;
                            LABTestLineByID.PurchaseRemark = PurchaseRemark;
                        }
                        if (LABTestLineByID.QTY == 0)
                        {
                            LABTestLineByID.QTY = QtyApprove;
                        }

                        //GENERATE NONCE
                        var nonceKey = Guid.NewGuid().ToString();
                        await nonceRepo.InsertAsync(new S2ELABTestNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = DatetimeNow,
                            EXPIREDATE = DatetimeNow.AddDays(7),
                            ISUSED = 0
                        });

                        //NEXT APPROVE LEVEL
                        var nextALL = new GenericRepository<S2ELABTestApproveTrans_TB>(unitOfWork.Transaction);
                        var nextAllLevel = await nextALL.GetAllAsync();
                        var nextLevel = nextAllLevel.Where(x => x.LABID == LABID &&
                                                            x.LABLINEID == LABLINEID &&
                                                            x.APPROVELEVEL == LABTestLineByID.CURRENTAPPROVESTEP &&
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
                                var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == RequestID).FirstOrDefault();

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
                                    <b>ผลการทดสอบ : </b> {Body}
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/LABTestTodolist?Email={next.EMAIL}'> คลิกที่นี่ </a>
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
                                    <b>ผลการทดสอบ : </b> {Body}
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/LABTestTodolist?Email={next.EMAIL}'> คลิกที่นี่ </a>
                                ";
                            }

                            var sendEmail = _emailService.SendEmail(
                                  $"{NewRequestByID.REQUESTCODE} / แจ้งผลการทดสอบวัตถุดิบ Lab / {Subject}",
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

                            var approveTrans_next = await LABTestTransRepo.GetAsync(next.ID);
                            approveTrans_next.SENDEMAILDATE = DatetimeNow;
                            await LABTestTransRepo.UpdateAsync(approveTrans_next);

                        }

                    }

                    await LABTestHeadRepo.UpdateAsync(LABTestHeadByID);
                    await LABTestLineRepo.UpdateAsync(LABTestLineByID);
                    await nonceRepo.UpdateAsync(_nonce);

                    unitOfWork.Complete();
                    AlertSuccess = "Approve Success.";
                    return Redirect($"/S2E/Qtech/LABTestTodolist?Email={email}");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/");
            }
        }

    }
}
