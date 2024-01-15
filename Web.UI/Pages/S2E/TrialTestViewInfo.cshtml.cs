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
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E
{
    public class TrialTestViewInfoModel : PageModel
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
        public string TRIALTestResultRemark1 { get; set; }
        [BindProperty]
        public string TRIALTestResultRemark2 { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public int isCompaire { get; set; }
        [BindProperty]
        public int RMReqID { get; set; }
        [BindProperty]
        public string ProjectRefNo { get; set; }
        [BindProperty]
        public int TrialID { get; set; }
        public string PageBack { get; set; }
        [BindProperty]
        public decimal QtyTotal { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public string EmailApprove { get; set; }
        [BindProperty]
        public int TrialLineID { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public TrialTestViewInfoModel(
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
        public async Task<IActionResult> OnGetAsync(int TRIALID,int TRIALLINEID, string EMAILAPPROVE)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var TrialTestLogsSendEmailRepo = new GenericRepository<S2ETrialTestLogsSendEmailSuccess_TB>(unitOfWork.Transaction);
                    var TrialTestLogsSendEmailALL = await TrialTestLogsSendEmailRepo.GetAllAsync();
                    var CheckView = TrialTestLogsSendEmailALL.Where(x => x.TRIALID == TRIALID && x.EMAIL == EMAILAPPROVE).FirstOrDefault();
                    if (CheckView == null)
                    {
                        AlertError = "ไม่สามารถดูข้อมูลได้ เนื่องจากไม่มีสิทธิ์การเข้าถึง";
                        return Redirect($"/S2E/Index");
                    }

                    DepartmentMaster = await GetDepartmentMaster();
                    ResonTestMaster = await GetResonTestMaster();
                    TypeOfRMMaster = await GetTypeOfRMMaster();
                    ApprovalForIDMaster = await GetApprovalForIDMaster();

                    TrialID = TRIALID;
                    TrialLineID = TRIALLINEID;
                    await GetData(TRIALID, TRIALLINEID);

                    return Page();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task GetData(int TRIALID, int TRIALLINEID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var TrialTestHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                var TrialTestHeadByID = await TrialTestHeadRepo.GetAsync(TRIALID);

                var TrialTestLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                var TrialTestLineByID = await TrialTestLineRepo.GetAsync(TRIALLINEID);

                var RMREQID = TrialTestHeadByID.RMREQID;

                var RMReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                var RMReqHeadByID = await RMReqHeadRepo.GetAsync(RMREQID);

                var ADDRMID = RMReqHeadByID.ADDRMID;

                ItemGroup = RMReqHeadByID.ITEMGROUP;

                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                var RequestID = AddRMHeadByID.REQUESTID;
                var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                var PCSampleID = AddRMHeadByID.PCSAMPLEID;
                var LABID = AddRMHeadByID.LABID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;
                isCompaire = NewRequestByID.ISCOMPAIRE;

                var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                Country = LABTestLineByID.COUNTRY;
                Manufacture = LABTestLineByID.MANUFACTURE;
                Agent = LABTestLineByID.AGENT;
                ProjectRefNo = LABTestLineByID.PROJECTREFNO;

                PlantCodeD1D2 = TrialTestLineByID.PLANTCODED1D2;
                PlantCodeD3 = TrialTestLineByID.PLANTCODED3;
                PlantCodeD4 = TrialTestLineByID.PLANTCODED4;
                PlantCodeD5 = TrialTestLineByID.PLANTCODED5;
                ChemicalName = TrialTestLineByID.CHEMICALNAME;

                PlantCodeD1D2Ref = NewRequestByID.ISCOMPAIRE == 1 ? TrialTestLineByID.PLANTCODED1D2REF : null;
                PlantCodeD3Ref = NewRequestByID.ISCOMPAIRE == 1 ? TrialTestLineByID.PLANTCODED3REF : null;
                PlantCodeD4Ref = NewRequestByID.ISCOMPAIRE == 1 ? TrialTestLineByID.PLANTCODED4REF : null;
                PlantCodeD5Ref = NewRequestByID.ISCOMPAIRE == 1 ? TrialTestLineByID.PLANTCODED5REF : null;
                ChemicalNameRef = NewRequestByID.ISCOMPAIRE == 1 ? TrialTestLineByID.CHEMICALNAMEREF : null;

                TestResult = TrialTestLineByID.TESTRESULT == 1 ? 1 : 2;

                if (TrialTestLineByID.APPROVALFORID == 1)
                {
                    ApprovalForID = 1;
                }
                else if (TrialTestLineByID.APPROVALFORID == 2)
                {
                    ApprovalForID = 2;
                }
                Plant = TrialTestLineByID.PLANT;
                if (TrialTestLineByID.PLANT != null)
                {
                    string[] PlantList = TrialTestLineByID.PLANT.Split(",");
                    foreach (var d in PlantList)
                    {
                        if (d == "DSL") { Plant1 = true; }
                        if (d == "DRB") { Plant2 = true; }
                        if (d == "DSI") { Plant3 = true; }
                        if (d == "DSR") { Plant4 = true; }
                        if (d == "STR") { Plant5 = true; }
                    }
                }

                DepartmentID = LABTestLineByID.DEPARTMENTID;
                DepartmentRemark = LABTestLineByID.DEPARTMENTREMARK;
                ResonTestID = LABTestLineByID.RESONTESTID;
                ResonTestRemark = LABTestLineByID.RESONTESTREMARK;
                TypeOfRMID = LABTestLineByID.TYPEOFRMID;

                if (TrialTestLineByID.APPROVESTATUS == 2)
                {
                    PageBack = "MainCancel";
                }
                else
                {
                    PageBack = "Main";
                }

                var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                ItemCode = PCSampleByID.ITEMCODE;
                TradeName = PCSampleByID.ITEMNAME;

                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                var AddRMLineByID = AddRMLineALL.Where(x => x.ADDRMID == ADDRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                var MaterialReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                var MaterialReqLineALL = await MaterialReqLineRepo.GetAllAsync();
                var MaterialReqLineByReqID = MaterialReqLineALL.Where(x => x.RMREQID == RMREQID &&
                                                                            x.ADDRMLINEID == AddRMLineByID.ID &&
                                                                            x.ISACTIVE == 1 &&
                                                                            x.APPROVESTATUS != 2);
                decimal QtyUse = 0;
                if (MaterialReqLineByReqID != null)
                {
                    foreach (var MaterialReqLineQTY in MaterialReqLineByReqID)
                    {
                        QtyUse += MaterialReqLineQTY.QTY;
                    }
                }

                QtyTotal = AddRMLineByID.QTY - QtyUse;
                Unit = RMReqHeadByID.UNIT;

                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

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

                }

                //Doc Ref
                var TrialTestLogDocRepo = new GenericRepository<S2ETrialTestLogsDoc_TB>(unitOfWork.Transaction);
                var TrialTestLogDocALL = await TrialTestLogDocRepo.GetAllAsync();
                foreach (var docLog in TrialTestLogDocALL.Where(x => x.TRIALID == TRIALID && x.TRIALLINEID == TRIALLINEID))
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
        public async Task<IActionResult> OnGetResonTestISRemark(int ResonID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var ResonTestMasterRepo = new GenericRepository<S2EMaster_LABTestResonTest_TB>(unitOfWork.Transaction);
                    var ResonTestMasterByID = await ResonTestMasterRepo.GetAsync(ResonID);

                    unitOfWork.Complete();

                    if (ResonTestMasterByID.ISREMARK == 1)
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
        public async Task<List<SelectListItem>> GetApprovalForIDMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var ApprovalForMaster = new GenericRepository<S2EMaster_LABTestApprovalFor_TB>(unitOfWork.Transaction);

                var ApprovalForALL = await ApprovalForMaster.GetAllAsync();

                return ApprovalForALL
                    .Where(x => x.ISACTIVE == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.APPROVALFORDESC
                    })
                    .ToList();
            }
        }
        public async Task<JsonResult> OnPostLabEvaluationGridAsync(int TRIALID, int TRIALLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<TrialTestResultGridViewModel>(@"
                       SELECT * 
                        FROM
                        (
                        SELECT M.ID,M.TRIALRESULTDESC,M.ISACTIVE,
                            M.ISREMARK1 AS ISREMARKA,M.ISREMARK2 AS ISREMARKB,
                            L.ISPASS,L.REMARK1 AS REMARKA,L.REMARK2 AS REMARKB,
                            L.ID AS LOGID
                            FROM TB_S2EMaster_TrialTestEvaluation M JOIN
                            TB_S2ETrialTestLogsTestResult L ON M.ID = L.TRIALEVALUATIONID
                            WHERE L.TRIALID = @TRIALID AND L.TRIALLINEID = @TRIALLINEID
                        )T ",
                        new
                        {
                            @TRIALID = TRIALID,
                            @TRIALLINEID = TRIALLINEID
                        }
                        , unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<JsonResult> OnPostProcessEvaluationGridAsync(int TRIALID, int TRIALLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<TrialTestProcessResultGridViewModel>(@"
                            SELECT * 
                            FROM
                            (
                            SELECT M.ID,M.PROCESSDESC,M.ISACTIVE,P.ISPASS,P.ID AS LOGID
                                FROM TB_S2EMaster_TrialTestProcEvaluation M JOIN
                                TB_S2ETrialTestLogsProcTestResult P ON M.ID = P.PROCESSID
                                WHERE TRIALID = @TRIALID AND TRIALLINEID = @TRIALLINEID
                            )T",
                            new
                            {
                                @TRIALID = TRIALID,
                                @TRIALLINEID = TRIALLINEID
                            }
                            , unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<JsonResult> OnPostProductEvaluationGridAsync(int TRIALID, int TRIALLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<TrialTestLogProdTestResultGridViewModel>(@"
                        SELECT ROW_NUMBER() OVER(PARTITION BY TRIALID ORDER BY ID ASC) AS ROW,
							ID,TRIALID,PRODUCTTESTDESC,ISPASS
                            FROM TB_S2ETrialTestLogsPrdTestResult 
                            WHERE TRIALID = @TRIALID AND TRIALLINEID = @TRIALLINEID",
                            new
                            {
                                @TRIALID = TRIALID,
                                @TRIALLINEID = TRIALLINEID
                            }
                            , unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<IActionResult> OnPostGridViewFileUploadAsync(int TRIALID, int TRIALLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ETrialTestLogsFileGridViewModel>($@"
                        SELECT ID,
		                    TRIALID,
                            TRIALLINEID,
		                    FILENAME,
                            CREATEBY,
		                    CONVERT(NVARCHAR,CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,CREATEDATE,108) AS CREATEDATE
                        FROM TB_S2ETrialTestLogsFile 
                        WHERE TRIALID = {TRIALID} AND TRIALLINEID = {TRIALLINEID} 
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
        public async Task<IActionResult> OnGetDownloadFileUploadAsync(int TRIALID, int TRIALLINEID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var TrialTestLogFileRepo = new GenericRepository<S2ETrialTestLogsFile_TB>(unitOfWork.Transaction);
                    var TrialTestLogFileByID = await TrialTestLogFileRepo.GetAsync(Fileid);

                    var TrialTestHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                    var TrialTestHeadByID = await TrialTestHeadRepo.GetAsync(TRIALID);

                    var TrialTestLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                    var TrialTestLineByID = await TrialTestLineRepo.GetAsync(TRIALLINEID);

                    var MaterialRMHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                    var MaterialRMHeadByID = await MaterialRMHeadRepo.GetAsync(TrialTestHeadByID.RMREQID);

                    var AddRMReqHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMReqHeadByID = await AddRMReqHeadRepo.GetAsync(MaterialRMHeadByID.ADDRMID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(AddRMReqHeadByID.REQUESTID);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/TrialTest/" + TRIALLINEID;

                    var fileName = TrialTestLogFileByID.FILENAME;

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
        //File NewRequest
        public async Task<IActionResult> OnPostGridFileNewRequestAsync(int TRIALID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ELogsFileGridViewModel>($@"
                        SELECT * FROM (
                            SELECT RL.ID
                                ,TH.ID TRIALID
                                ,RL.FILENAME
                                ,RL.CREATEBY
                                ,CONVERT(NVARCHAR,RL.CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,RL.CREATEDATE,108) AS CREATEDATE
                                ,R.ID REQUESTID
                            FROM TB_S2ETrialTestHead TH
                            JOIN TB_S2EMaterialRequestHead RH ON TH.RMREQID = RH.ID
                            JOIN TB_S2EAddRawMaterialHead AH ON RH.ADDRMID = AH.ID
                            JOIN TB_S2ENewRequest R ON AH.REQUESTID = R.ID 
                            JOIN TB_S2ENewRequestLogsFile RL ON R.ID = RL.REQUESTID
                            WHERE TH.ID = {TRIALID}
                        )T1
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
        public async Task<IActionResult> OnGetDownloadFileNewRequestAsync(int RequestID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var NewRequestFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                    var NewRequestFileByID = await NewRequestFileRepo.GetAsync(Fileid);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/NewRequest";

                    var fileName = NewRequestFileByID.FILENAME;

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
        //File RMAssessment
        public async Task<IActionResult> OnPostGridFileRMAssessmentAsync(int TRIALID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ELogsFileGridViewModel>($@"
                        SELECT * FROM (
                            SELECT RL.ID
                                ,TH.ID TRIALID
                                ,RL.FILENAME
                                ,RL.CREATEBY
                                ,CONVERT(NVARCHAR,RL.CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,RL.CREATEDATE,108) AS CREATEDATE
                                ,R.ID REQUESTID
                            FROM TB_S2ETrialTestHead TH
                            JOIN TB_S2EMaterialRequestHead RH ON TH.RMREQID = RH.ID
                            JOIN TB_S2EAddRawMaterialHead AH ON RH.ADDRMID = AH.ID
                            JOIN TB_S2ERMAssessment R ON AH.ASSESSMENTID = R.ID 
                            JOIN TB_S2ERMAssessmentLogsFile RL ON R.ID = RL.ASSESSMENTID
                            WHERE TH.ID = {TRIALID}
                        )T1
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
        public async Task<IActionResult> OnGetDownloadFileRMAssessmentAsync(int AssessmentID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RMAssessmentByID.REQUESTID);

                    var RMAssessmentFileRepo = new GenericRepository<S2ERMAssessmentLogsFile_TB>(unitOfWork.Transaction);
                    var RMAssessmentFileByID = await RMAssessmentFileRepo.GetAsync(Fileid);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/RMAssessment";

                    var fileName = RMAssessmentFileByID.FILENAME;

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
        //File Add Rawmaterial Sample
        public async Task<IActionResult> OnPostGridFileAddRawMaterialSampleAsync(int TRIALID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ELogsFileGridViewModel>($@"
                        SELECT * FROM (
                            SELECT L.ID
                                ,TH.ID TRIALID
                                ,L.FILENAME
                                ,0 AS CREATEBY
                                ,CONVERT(NVARCHAR,L.CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,L.CREATEDATE,108) AS CREATEDATE
                                ,ARS.ID REQUESTID
                            FROM TB_S2ETrialTestHead TH
                            JOIN TB_S2EMaterialRequestHead RH ON TH.RMREQID = RH.ID
                            JOIN TB_S2EAddRawMaterialHead AH ON RH.ADDRMID = AH.ID
                            JOIN TB_S2ERMAssessment R ON AH.ASSESSMENTID = R.ID 
                            JOIN TB_S2EAddRawMaterialSample ARS ON R.ID = ARS.ASSESSMENTID 
                            JOIN TB_S2EAddRawMaterialSampleLogsFile L ON ARS.ID = L.ADDRMSAMPLEID AND L.ISACTIVE = 1
                            WHERE TH.ID = {TRIALID}
                        )T1
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
        public async Task<IActionResult> OnGetDownloadFileAddRawMaterialSampleAsync(int AddRMSampleID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                    var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(AddRMSampleByID.REQUESTID);

                    var AddRMSampleFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                    var AddRMSampleFileByID = await AddRMSampleFileRepo.GetAsync(Fileid);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) +
                        "/AddRawMaterialSample/" + AddRMSampleFileByID.ADDRMSAMPLEID;

                    var fileName = AddRMSampleFileByID.FILENAME;

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
        //File Purchase Sample
        public async Task<IActionResult> OnPostGridFilePurchaseSampleAsync(int TRIALID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ELogsFileGridViewModel>($@"
                        SELECT * FROM (
                            SELECT RL.ID
                                ,TH.ID TRIALID
                                ,RL.FILENAME
                                ,RL.CREATEBY
                                ,CONVERT(NVARCHAR,RL.CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,RL.CREATEDATE,108) AS CREATEDATE
                                ,R.ID REQUESTID
                            FROM TB_S2ETrialTestHead TH
                            JOIN TB_S2EMaterialRequestHead RH ON TH.RMREQID = RH.ID
                            JOIN TB_S2EAddRawMaterialHead AH ON RH.ADDRMID = AH.ID
                            JOIN TB_S2EPurchaseSample R ON AH.PCSAMPLEID = R.ID 
                            JOIN TB_S2EPurchaseSampleLogsFile RL ON R.ID = RL.PCSAMPLEID
                            WHERE TH.ID = {TRIALID}
                        )T1
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
        public async Task<IActionResult> OnGetDownloadFilePurchaseSampleAsync(int PCSampleID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(PCSampleByID.REQUESTID);

                    var PCSampleFileRepo = new GenericRepository<S2EPurchaseSampleLogsFile_TB>(unitOfWork.Transaction);
                    var PCSampleFileByID = await PCSampleFileRepo.GetAsync(Fileid);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/PurchaseSample";

                    var fileName = PCSampleFileByID.FILENAME;

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
        //File Add Rawmaterial
        public async Task<IActionResult> OnPostGridFileAddRawMaterialAsync(int TRIALID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ELogsFileGridViewModel>($@"
                        SELECT * FROM (
                            SELECT AL.ID
                                ,TH.ID TRIALID
                                ,AL.FILENAME
                                ,AL.CREATEBY
                                ,CONVERT(NVARCHAR,AL.CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,AL.CREATEDATE,108) AS CREATEDATE
                                ,AH.ID REQUESTID
                            FROM TB_S2ETrialTestHead TH
                            JOIN TB_S2EMaterialRequestHead RH ON TH.RMREQID = RH.ID
                            JOIN TB_S2EAddRawMaterialHead AH ON RH.ADDRMID = AH.ID
                            JOIN TB_S2EAddRawMaterialLogsFile AL ON AH.ID = AL.ADDRMID
                            WHERE TH.ID = {TRIALID}
                        )T1
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
        public async Task<IActionResult> OnGetDownloadFileAddRawMaterialAsync(int AddRMID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(AddRMHeadByID.REQUESTID);

                    var AddRMFileRepo = new GenericRepository<S2EAddRawMaterialLogsFile_TB>(unitOfWork.Transaction);
                    var AddRMFileByID = await AddRMFileRepo.GetAsync(Fileid);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) +
                        "/AddRawMaterial/" + AddRMFileByID.ADDRMLINEID;

                    var fileName = AddRMFileByID.FILENAME;

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
        // Grid Approve
        public async Task<IActionResult> OnPostGridViewApproveAsync(int TRIALID, int TRIALLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<GetApproveLogsGridViewModel>($@"
                        SELECT EMAIL,
					           CONVERT(NVARCHAR,APPROVEDATE,103) + ' ' + CONVERT(NVARCHAR,APPROVEDATE,108) AS APPROVEDATE,
                               REMARK 
                        FROM TB_S2ETrialTestApproveTrans
                        WHERE TRIALID = {TRIALID}
                        AND TRIALLINEID = {TRIALLINEID}
                        AND ISCURRENTAPPROVE = 1
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
    }
}
