using System;
using System.Collections.Generic;
using System.Globalization;
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
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Qtech.TRIALTest
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
        public List<int> TrialResultID { get; set; }
        [BindProperty]
        public List<int> ProcResultID { get; set; }
        [BindProperty]
        public int isCompaire { get; set; }
        [BindProperty]
        public List<IFormFile> FileUpload { get; set; }
        [BindProperty]
        public int RMReqID { get; set; }
        [BindProperty]
        public string ProjectRefNo { get; set; }
        [BindProperty]
        public int TrialID { get; set; }
        [BindProperty]
        public List<string> LabResultLogsID { get; set; }
        [BindProperty]
        public List<string> ProcResultLogsID { get; set; }
        [BindProperty]
        public List<string> ProdResultLogsID { get; set; }
        [BindProperty]
        public List<int> ProdIsPassOLD { get; set; }
        [BindProperty]
        public decimal QtyTotal { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public int TrialLineID { get; set; } 

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
        public async Task<IActionResult> OnGetAsync(int TRIALID, int TRIALLINEID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_TRIALTEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
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
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/TRIALTest/Main");
            }
        }
        public async Task GetData(int TRIALID,int TRIALLINEID)
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
                var LABLINEID = AddRMHeadByID.LABLINEID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;
                isCompaire = NewRequestByID.ISCOMPAIRE;

                var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                //var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                var LABTestLineByID = await LABTestLineRepo.GetAsync(LABLINEID);

                Country = LABTestLineByID.COUNTRY;
                Manufacture = LABTestLineByID.MANUFACTURE;
                Agent = LABTestLineByID.AGENT;
                ProjectRefNo = LABTestLineByID.PROJECTREFNO;

                PlantCodeD1D2 = TrialTestLineByID.PLANTCODED1D2;
                PlantCodeD3 = TrialTestLineByID.PLANTCODED3;
                PlantCodeD4 = TrialTestLineByID.PLANTCODED4;
                PlantCodeD5 = TrialTestLineByID.PLANTCODED5;
                ChemicalName = TrialTestLineByID.CHEMICALNAME;
                TestResult = TrialTestLineByID.TESTRESULT == 1 ? 1 : 2;
                PlantCodeD1D2Ref = NewRequestByID.ISCOMPAIRE == 1 ? TrialTestLineByID.PLANTCODED1D2REF : null;
                PlantCodeD3Ref = NewRequestByID.ISCOMPAIRE == 1 ? TrialTestLineByID.PLANTCODED3REF : null;
                PlantCodeD4Ref = NewRequestByID.ISCOMPAIRE == 1 ? TrialTestLineByID.PLANTCODED4REF : null;
                PlantCodeD5Ref = NewRequestByID.ISCOMPAIRE == 1 ? TrialTestLineByID.PLANTCODED5REF : null;
                ChemicalNameRef = NewRequestByID.ISCOMPAIRE == 1 ? TrialTestLineByID.CHEMICALNAMEREF : null;

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
        public async Task<IActionResult> OnGetDelelteProdResult(int ProdID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var LogsProdTestRepo = new GenericRepository<S2ETrialTestLogsPrdTestResult_TB>(unitOfWork.Transaction);
                    var LogsProdTestByID = await LogsProdTestRepo.GetAsync(ProdID);

                    await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        DELETE FROM TB_S2ETrialTestLogsPrdTestResult  
                        WHERE ID = {ProdID}
                    ", null, unitOfWork.Transaction);

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
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/TrialTest/"+TRIALLINEID;

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
        public async Task<IActionResult> OnGetDelelteFile(int FileID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var TrialTestLogFileRepo = new GenericRepository<S2ETrialTestLogsFile_TB>(unitOfWork.Transaction);
                    var TrialTestLogFileByID = await TrialTestLogFileRepo.GetAsync(FileID);
                    var TRIALID = TrialTestLogFileByID.TRIALID;
                    var TRIALLINEID = TrialTestLogFileByID.TRIALLINEID;

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
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/TrialTest/"+TRIALLINEID;
                    var fileName = TrialTestLogFileByID.FILENAME;

                    await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        DELETE FROM TB_S2ETrialTestLogsFile  
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
        public async Task<IActionResult> OnPostAsync(int TRIALID, int TRIALLINEID, string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                DepartmentMaster = await GetDepartmentMaster();
                ResonTestMaster = await GetResonTestMaster();
                TypeOfRMMaster = await GetTypeOfRMMaster();
                ApprovalForIDMaster = await GetApprovalForIDMaster();

                TrialID = TRIALID;
                TrialLineID = TRIALLINEID;
                await GetData(TRIALID, TRIALLINEID);

                return Page();
            }

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RequestDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;

                    var TrialTestHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                    var TrialTestHeadByID = await TrialTestHeadRepo.GetAsync(TRIALID);

                    var TrialTestLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                    var TrialTestLineByID = await TrialTestLineRepo.GetAsync(TRIALLINEID);

                    var RMREQID = TrialTestHeadByID.RMREQID;

                    var RMReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                    var RMReqHeadByID = await RMReqHeadRepo.GetAsync(RMREQID);

                    var ADDRMID = RMReqHeadByID.ADDRMID;

                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                    var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                    var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                    var AddRMLine = AddRMLineALL.Where(x => x.ADDRMID == ADDRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();
                    var AddRMLineByID = await AddRMLineRepo.GetAsync(AddRMLine.ID);

                    var RequestID = AddRMHeadByID.REQUESTID;
                    var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                    var PCSampleID = AddRMHeadByID.PCSAMPLEID;
                    var LABID = AddRMHeadByID.LABID;
                    var LABLINEID = AddRMHeadByID.LABLINEID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    //var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                    var LABTestLineByID = await LABTestLineRepo.GetAsync(LABLINEID);

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByEditBy = approveMapALL.Where(x => x.CreateBy == CreateBy && x.STEP == 1 &&
                                                                    x.ISTRIALTEST == 1).FirstOrDefault();
                    var approvemasterid = approveMapByEditBy.APPROVEMASTERID;
                    var ApproveGroupID = approveMapByEditBy.APPROVEGROUPID;

                    //Draft or Save
                    var ApproveStatus = 0;
                    if (!string.IsNullOrEmpty(draft))
                    {
                        ApproveStatus = RequestStatusModel.Draft;
                    }
                    if (!string.IsNullOrEmpty(save))
                    {
                        ApproveStatus = RequestStatusModel.WaitingForApprove;
                    }

                    TrialTestHeadByID.REQUESTDATE = RequestDate;

                    TrialTestLineByID.CHEMICALNAME = ChemicalName;
                    TrialTestLineByID.PLANTCODED1D2 = PlantCodeD1D2;
                    TrialTestLineByID.PLANTCODED3 = PlantCodeD3;
                    TrialTestLineByID.PLANTCODED4 = PlantCodeD4;
                    TrialTestLineByID.PLANTCODED5 = PlantCodeD5;
                    TrialTestLineByID.CHEMICALNAMEREF = NewRequestByID.ISCOMPAIRE == 1 ? ChemicalNameRef : null;
                    TrialTestLineByID.PLANTCODED1D2REF = NewRequestByID.ISCOMPAIRE == 1 ? PlantCodeD1D2Ref : null;
                    TrialTestLineByID.PLANTCODED3REF = NewRequestByID.ISCOMPAIRE == 1 ? PlantCodeD3Ref : null;
                    TrialTestLineByID.PLANTCODED4REF = NewRequestByID.ISCOMPAIRE == 1 ? PlantCodeD4Ref : null;
                    TrialTestLineByID.PLANTCODED5REF = NewRequestByID.ISCOMPAIRE == 1 ? PlantCodeD5Ref : null;
                    TrialTestLineByID.TESTRESULT = TestResult == 1 ? 1 : 0;
                    TrialTestLineByID.APPROVALFORID = ApprovalForID;
                    TrialTestLineByID.PLANT = Plant;
                    TrialTestLineByID.APPROVEMASTERID = approvemasterid;
                    TrialTestLineByID.CURRENTAPPROVESTEP = 1;
                    TrialTestLineByID.APPROVESTATUS = ApproveStatus;
                    TrialTestLineByID.UPDATEBY = CreateBy;
                    TrialTestLineByID.UPDATEDATE = RequestDate;

                    //DELETE OLD LOG DOC
                    using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection()))
                    {
                        var deleteOLDLogsDoc = unitOfWork2.Transaction.Connection.Execute(@"DELETE 
                                FROM TB_S2ETrialTestLogsDoc
                                WHERE TRIALID = @TRIALID 
                                AND TRIALLINEID = @TRIALLINEID",
                            new
                            {
                                @TRIALID = TRIALID,
                                @TRIALLINEID = TRIALLINEID
                            },
                            unitOfWork2.Transaction
                        );
                        unitOfWork2.Complete();
                    }

                    //INSERT LOGS DOC
                    var S2ELogsDocRepo = new GenericRepository<S2ETrialTestLogsDoc_TB>(unitOfWork.Transaction);
                    var S2ELogsDocALL = await S2ELogsDocRepo.GetAllAsync();
                    var S2EMasterDocRepo = new GenericRepository<S2EMaster_LABTestDocRef_TB>(unitOfWork.Transaction);
                    var S2EMasterDocALL = await S2EMasterDocRepo.GetAllAsync();

                    int doc1, doc2, doc3, doc4, doc5, doc6, doc7, doc8, doc9, doc10,
                      doc11, doc12, doc13, doc14, doc15, doc16, doc17, doc18;

                    doc1 = DocRef1 == true ? 1 : 0; doc2 = DocRef2 == true ? 2 : 0; doc3 = DocRef3 == true ? 3 : 0;
                    doc4 = DocRef4 == true ? 4 : 0; doc5 = DocRef5 == true ? 5 : 0; doc6 = DocRef6 == true ? 6 : 0;
                    doc7 = DocRef7 == true ? 7 : 0; doc8 = DocRef8 == true ? 8 : 0; doc9 = DocRef9 == true ? 9 : 0;
                    doc10 = DocRef10 == true ? 10 : 0; doc11 = DocRef11 == true ? 11 : 0; doc12 = DocRef12 == true ? 12 : 0;
                    doc13 = DocRef13 == true ? 13 : 0; doc14 = DocRef14 == true ? 14 : 0; doc15 = DocRef15 == true ? 15 : 0;
                    doc16 = DocRef16 == true ? 16 : 0; doc17 = DocRef17 == true ? 17 : 0; doc18 = DocRef18 == true ? 18 : 0;

                    int[] doc_all = new int[18] { doc1, doc2, doc3, doc4, doc5, doc6, doc7, doc8, doc9, doc10,
                                                  doc11, doc12, doc13, doc14, doc15, doc16, doc17, doc18};

                    foreach (int str in doc_all)
                    {
                        if (str > 0)
                        {
                            var DocRef_Name = S2EMasterDocALL.Where(x => x.ID == str)
                                                                    .Select(s => s.DESCRIPTION).FirstOrDefault();
                            var Remark = "";

                            await S2ELogsDocRepo.InsertAsync(new S2ETrialTestLogsDoc_TB
                            {
                                TRIALID = TRIALID,
                                TRIALLINEID = TRIALLINEID,
                                DOCID = str,
                                DOCDESCRIPTION = DocRef_Name,
                                REMARK = Remark,
                                CREATEBY = CreateBy,
                                CREATEDATE = RequestDate
                            });
                        }
                    }

                    //UPDATE LAB TEST RESULT
                    int row = LabResultLogsID.Count();
                    int[] LABResultLogsIDToINT = LabResultLogsID.Select(s => int.Parse(s)).ToArray();
                    int[] LABResultMasterIDToINT = TrialResultID.ToArray();
                    for (int i = 0; i < row; i++)
                    {
                        var LABResultLogsID = LABResultLogsIDToINT[i];      //LogsID
                        var LABResultMasterID = LABResultMasterIDToINT[i];  //MasterID

                        //GetMaster
                        var S2ETrialResultMasterRepo = new GenericRepository<S2EMaster_TrialTestEvaluation_TB>(unitOfWork.Transaction);
                        var S2ETrialResultMasterByID = await S2ETrialResultMasterRepo.GetAsync(LABResultMasterID);
                        //GetLogs
                        var S2ELogsTrialResultRepo = new GenericRepository<S2ETrialTestLogsTestResult_TB>(unitOfWork.Transaction);
                        var S2ELogsTrialResultTrialID = await S2ELogsTrialResultRepo.GetAsync(LABResultLogsID);

                        var Remark1 = ""; var Remark2 = "";
                        if (S2ETrialResultMasterByID.ISREMARK1 == 1 && S2ETrialResultMasterByID.ISREMARK2 == 1)
                        {
                            Remark1 = TRIALTestResultRemark1;
                            Remark2 = TRIALTestResultRemark2;
                        }

                        //UPDATE
                        if (S2ELogsTrialResultTrialID.ISPASS != LabIsPass[i] ||
                            S2ELogsTrialResultTrialID.REMARK1 != Remark1 ||
                            S2ELogsTrialResultTrialID.REMARK2 != Remark2)
                        {
                            S2ELogsTrialResultTrialID.ISPASS = LabIsPass[i];
                            S2ELogsTrialResultTrialID.CREATEBY = CreateBy;
                            S2ELogsTrialResultTrialID.CREATEDATE = RequestDate;
                            S2ELogsTrialResultTrialID.REMARK1 = Remark1;
                            S2ELogsTrialResultTrialID.REMARK2 = Remark2;

                            await S2ELogsTrialResultRepo.UpdateAsync(S2ELogsTrialResultTrialID);
                        }
                    }

                    //UPDATE PROCESS TEST RESULT
                    int row2 = ProcResultLogsID.Count();
                    int[] ProcessResultLogsIDToINT = ProcResultLogsID.Select(s => int.Parse(s)).ToArray();
                    for (int i = 0; i < row2; i++)
                    {
                        var ProcessResultLogsID = ProcessResultLogsIDToINT[i];      //LogsID

                        //GetLogs
                        var S2ELogsProcResultRepo = new GenericRepository<S2ETrialTestLogsProcTestResult_TB>(unitOfWork.Transaction);
                        var S2ELogsProcResultByProcLogsID = await S2ELogsProcResultRepo.GetAsync(ProcessResultLogsID);

                        //UPDATE
                        if (S2ELogsProcResultByProcLogsID.ISPASS != ProcIsPass[i])
                        {
                            S2ELogsProcResultByProcLogsID.ISPASS = ProcIsPass[i];
                            S2ELogsProcResultByProcLogsID.CREATEBY = CreateBy;
                            S2ELogsProcResultByProcLogsID.CREATEDATE = RequestDate;

                            await S2ELogsProcResultRepo.UpdateAsync(S2ELogsProcResultByProcLogsID);
                        }

                    }

                    //UPDATE PRODUCT TEST RESULT
                    int row3 = ProdResultLogsID.Count();
                    int[] ProductResultLogsIDToINT = ProdResultLogsID.Select(s => int.Parse(s)).ToArray();
                    for (int i = 0; i < row3; i++)
                    {
                        var ProductResultLogsID = ProductResultLogsIDToINT[i];      //LogsID

                        //GetLogs
                        var S2ELogsProdResultRepo = new GenericRepository<S2ETrialTestLogsPrdTestResult_TB>(unitOfWork.Transaction);
                        var S2ELogsProdResultByProdLogsID = await S2ELogsProdResultRepo.GetAsync(ProductResultLogsID);

                        //UPDATE
                        if (S2ELogsProdResultByProdLogsID.ISPASS != ProdIsPassOLD[i])
                        {
                            S2ELogsProdResultByProdLogsID.ISPASS = ProdIsPassOLD[i];
                            S2ELogsProdResultByProdLogsID.CREATEBY = CreateBy;
                            S2ELogsProdResultByProdLogsID.CREATEDATE = RequestDate;

                            await S2ELogsProdResultRepo.UpdateAsync(S2ELogsProdResultByProdLogsID);
                        }
                    }

                    //INSERT NEW PRODUCTION TEST
                    var S2ELogsProdResultInsertRepo = new GenericRepository<S2ETrialTestLogsPrdTestResult_TB>(unitOfWork.Transaction);
                    int row4 = ProdIsPass.Count();
                    //Product Evaluartion
                    for (int i = 0; i < row4; i++)
                    {
                        if (ProdList[i] != null && ProdIsPass != null)
                        {
                            var IsPass = ProdIsPass[i] == "1" ? 1 : 0;
                            await S2ELogsProdResultInsertRepo.InsertAsync(new S2ETrialTestLogsPrdTestResult_TB
                            {
                                TRIALID = TRIALID,
                                TRIALLINEID = TRIALLINEID,
                                PRODUCTTESTDESC = ProdList[i],
                                ISPASS = IsPass,
                                CREATEBY = CreateBy,
                                CREATEDATE = RequestDate
                            });
                        }

                    }

                    //UPLOAD FILE 
                    var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                       NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);
                    int rowfile = FileUpload.Count();
                    string basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/"+TRIALLINEID;
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }
                    var filePath = Path.GetTempFileName();
                    string fileName = "";
                    var TrialTestLogsFileRepo = new GenericRepository<S2ETrialTestLogsFile_TB>(unitOfWork.Transaction);
                    for (int i = 0; i < rowfile; i++)
                    {
                        if (FileUpload[i] != null)
                        {
                            fileName = Path.GetFileName(FileUpload[i].FileName);
                            using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                            {
                                await FileUpload[i].CopyToAsync(stream);
                                await TrialTestLogsFileRepo.InsertAsync(new S2ETrialTestLogsFile_TB
                                {
                                    TRIALID = TRIALID,
                                    TRIALLINEID = TRIALLINEID,
                                    FILENAME = fileName,
                                    CREATEBY = CreateBy,
                                    CREATEDATE = RequestDate
                                });
                            }

                        }
                    }

                    await TrialTestHeadRepo.UpdateAsync(TrialTestHeadByID);
                    await TrialTestLineRepo.UpdateAsync(TrialTestLineByID);

                    //Save 
                    if (!string.IsNullOrEmpty(save))
                    {
                        //UPDATE OLD APPROVE TRANS
                        var ApproveTransOldRepo = new GenericRepository<S2ETrialTestApproveTrans_TB>(unitOfWork.Transaction);
                        var ApproveTransOldALL = ApproveTransOldRepo.GetAll();
                        var ApproveTransOld = ApproveTransOldALL.Where(x => x.TRIALID == TRIALID && 
                                                                            x.TRIALLINEID == TRIALLINEID && 
                                                                            x.APPROVEGROUPID == ApproveGroupID);
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
                        var nonceRepo = new GenericRepository<S2ETrialTestNonce_TB>(unitOfWork.Transaction);
                        await nonceRepo.InsertAsync(new S2ETrialTestNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = RequestDate,
                            EXPIREDATE = RequestDate.AddDays(7),
                            ISUSED = 0
                        });

                        // INSERT APPROVE TRANSECTION
                        var AppTranRepo = new GenericRepository<S2ETrialTestApproveTrans_TB>(unitOfWork.Transaction);
                        foreach (var AppFlow in approveFlow_data)
                        {

                            await AppTranRepo.InsertAsync(new S2ETrialTestApproveTrans_TB
                            {
                                TRIALID = TRIALID,
                                TRIALLINEID = TRIALLINEID,
                                APPROVEMASTERID = AppFlow.ApproveMasterId,
                                APPROVEGROUPID = ApproveGroupID,
                                EMAIL = AppFlow.Email,
                                APPROVELEVEL = AppFlow.ApproveLevel,
                                ISCURRENTAPPROVE = 1
                            });
                        }

                        var currentRecord = await TrialTestLineRepo.GetAsync(TRIALLINEID);
                        currentRecord.CURRENTAPPROVESTEP = 1;
                        await TrialTestLineRepo.UpdateAsync(currentRecord);

                        //GET APPROVE TRANS LEVEL 1
                        var AppTransByRequestID = await unitOfWork.S2EControl.GetApproveTransByTrialID(TRIALID, TRIALLINEID, approvemasterid, ApproveGroupID);
                        var AppTransLevel1 = AppTransByRequestID.Where(x => x.APPROVELEVEL == 1);
                        foreach (var AppTrans in AppTransLevel1)
                        {
                            var approveFlowApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                       x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                       x.Email == AppTrans.EMAIL);

                            var FName = approveFlowApproveBy.Select(s => s.Name).FirstOrDefault();
                            var LName = approveFlowApproveBy.Select(s => s.LastName).FirstOrDefault();
                            var ApproveBy = FName + " " + LName;

                            var AppTransByALL = await AppTranRepo.GetAllAsync();
                            var AppTransByID = AppTransByALL.Where(x => x.ID == AppTrans.ID).FirstOrDefault();

                            AppTransByID.SENDEMAILDATE = RequestDate;
                            await AppTranRepo.UpdateAsync(AppTransByID);

                            //Send Mail
                            var Subject = "";
                            var Body = "";
                            if (TestResult == 1)
                            {
                                Subject = "ผ่านการทดสอบ";
                                Body = "<b style='color:green'> ผ่านการทดสอบ </b>";
                            }
                            else
                            {
                                Subject = "ไม่ผ่านการทดสอบ";
                                Body = "<b style='color:red'> ไม่ผ่านการทดสอบ </b>";
                            }

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
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestCompaireByRequestID.PRICEREF)}  {NewRequestCompaireByRequestID.CURRENCYCODEREF}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b> รายการวัตถุดิบที่นำเข้า / นำมาเปรียบเทียบ  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {AddRMHeadByID.VENDORID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {AddRMHeadByID.SUPPLIERNAME}
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
                                                <b> Item Code :</b> {PCSampleByID.ITEMCODE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {PCSampleByID.ITEMNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", AddRMLineByID.PRICE)}  {AddRMHeadByID.CURRENCYCODE}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>ผลการทดสอบ : </b> {Body}
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/TRIALTestTodolist?Email={AppTrans.EMAIL}'> คลิกที่นี่ </a>
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
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {AddRMHeadByID.VENDORID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {AddRMHeadByID.SUPPLIERNAME}
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
                                                <b> Item Code :</b> {PCSampleByID.ITEMCODE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {PCSampleByID.ITEMNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", AddRMLineByID.PRICE)}  {AddRMHeadByID.CURRENCYCODE}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>ผลการทดสอบ : </b> {Body}
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/TRIALTestTodolist?Email={AppTrans.EMAIL}'> คลิกที่นี่ </a>
                                    <br/>
                                ";
                            }

                            var sendEmail = _emailService.SendEmail(
                                   $"{NewRequestByID.REQUESTCODE} / แจ้งผลการทดสอบวัตถุดิบ Trial / {Subject}",
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
                    AlertSuccess = "Edit Trial test Success";
                    return Redirect("/S2E/Qtech/TRIALTest/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/TRIALTest/" + TRIALID + "/" + TRIALLINEID + "/Edit");
            }
        }
    }
}
