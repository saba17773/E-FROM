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
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Qtech.TRIALTest
{
    public class CreateModel : PageModel
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
        public decimal QtyTotal { get; set; }
        [BindProperty]
        public string Unit { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public CreateModel(
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
        public async Task<IActionResult> OnGetAsync(int RMREQID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_TRIALTEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var TrialTestHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                    var TrialTestHeadALL = await TrialTestHeadRepo.GetAllAsync();
                    var CheckRequest = TrialTestHeadALL.Where(x => x.RMREQID == RMREQID).FirstOrDefault();
                    if (CheckRequest != null)
                    {
                        AlertError = "Request �����ӡ�����ҧ����";
                        return Redirect($"/S2E/Qtech/TRIALTest/Main");
                    }

                    DepartmentMaster = await GetDepartmentMaster();
                    ResonTestMaster = await GetResonTestMaster();
                    TypeOfRMMaster = await GetTypeOfRMMaster();
                    ApprovalForIDMaster = await GetApprovalForIDMaster();

                    RMReqID = RMREQID;
                    await GetData(RMREQID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/TRIALTest/Main");
            }
        }
        public async Task GetData(int RMREQID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
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

                PlantCodeD1D2 = LABTestLineByID.PLANTCODED1D2;
                PlantCodeD3 = LABTestLineByID.PLANTCODED3;
                PlantCodeD4 = LABTestLineByID.PLANTCODED4;
                PlantCodeD5 = LABTestLineByID.PLANTCODED5;

                DepartmentID = LABTestLineByID.DEPARTMENTID;
                DepartmentRemark = LABTestLineByID.DEPARTMENTREMARK;
                ResonTestID = LABTestLineByID.RESONTESTID;
                ResonTestRemark = LABTestLineByID.RESONTESTREMARK;
                TypeOfRMID = LABTestLineByID.TYPEOFRMID;

                ChemicalName = LABTestLineByID.CHEMICALNAME;

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

                    ChemicalNameRef = LABTestLineByID.CHEMICALNAMEREF;
                    PlantCodeD1D2Ref = LABTestLineByID.PLANTCODED1D2REF;
                    PlantCodeD3Ref = LABTestLineByID.PLANTCODED3REF;
                    PlantCodeD4Ref = LABTestLineByID.PLANTCODED4REF;
                    PlantCodeD5Ref = LABTestLineByID.PLANTCODED5REF;

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
        public async Task<JsonResult> OnPostLabEvaluationGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<TrialTestResultGridViewModel>(@"
                             SELECT ID,TRIALRESULTDESC,ISACTIVE,
                            ISREMARK1 AS ISREMARKA,ISREMARK2 AS ISREMARKB
                            FROM TB_S2EMaster_TrialTestEvaluation
                            WHERE ISACTIVE = 1 ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<JsonResult> OnPostProcessEvaluationGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EMaster_TRIALTestProcEvaluation_TB>(@"
                        SELECT *
                            FROM TB_S2EMaster_TrialTestProcEvaluation
                            WHERE ISACTIVE = 1 ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<IActionResult> OnPostAsync(int RMREQID, string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                DepartmentMaster = await GetDepartmentMaster();
                ResonTestMaster = await GetResonTestMaster();
                TypeOfRMMaster = await GetTypeOfRMMaster();
                ApprovalForIDMaster = await GetApprovalForIDMaster();

                RMReqID = RMREQID;
                await GetData(RMREQID);

                return Page();
            }

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RequestDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;

                    var RMReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                    var RMReqHeadByID = await RMReqHeadRepo.GetAsync(RMREQID);

                    var ADDRMID = RMReqHeadByID.ADDRMID;

                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                    var RequestID = AddRMHeadByID.REQUESTID;
                    var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                    var PCSampleID = AddRMHeadByID.PCSAMPLEID;
                    var LABID = AddRMHeadByID.LABID;
                    var LABLINEID = AddRMHeadByID.LABLINEID;

                    var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                    var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                    var AddRMLine = AddRMLineALL.Where(x => x.ADDRMID == AddRMHeadByID.ID && x.ISCURRENTLOGS == 1).FirstOrDefault();
                    var AddRMLineByID = await AddRMLineRepo.GetAsync(AddRMLine.ID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                    var LABTestNOTNULL = LABTestLineALL.Where(x => x.PROJECTREFNO != null);

                    ItemGroup = RMReqHeadByID.ITEMGROUP;

                    //UPDATE 
                    RMReqHeadByID.REQUESTSTATUS = RequestStatusModel.Successfully;
                    await RMReqHeadRepo.UpdateAsync(RMReqHeadByID);

                    var RMReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                    var RMReqLineALL = await RMReqLineRepo.GetAllAsync();
                    var RMReqLineByHeadID = RMReqLineALL.Where(x => x.RMREQID == RMREQID);
                    foreach (var Line in RMReqLineByHeadID)
                    {
                        var UpdateLine = await RMReqLineRepo.GetAsync(Line.ID);
                        
                        UpdateLine.COMPLETEBY = CreateBy;
                        UpdateLine.COMPLETEDATE = RequestDate;
                        await RMReqLineRepo.UpdateAsync(UpdateLine);
                    }

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByCreateBy = approveMapALL.Where(x => x.CreateBy == CreateBy &&
                                                                   x.STEP == 1 &&
                                                                   x.ISTRIALTEST == 1
                                                              ).FirstOrDefault();

                    var approvemasterid = approveMapByCreateBy.APPROVEMASTERID;
                    var ApproveGroupID = approveMapByCreateBy.APPROVEGROUPID;

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


                    var ProjectRefNo = "";
                    //GENERATE PROJECT REF NO.
                    var YearNow = @DateTime.Now.ToString("yyyy", new CultureInfo("en-US"));

                    var chkYear = LABTestNOTNULL.Where(x => x.PROJECTREFNO.Substring(4, 4) == YearNow)
                                            .Max(a => a.PROJECTREFNO.Substring(12, 3));
                    int ProjectRef_Autorun;
                    if (chkYear == null)
                    {
                        ProjectRefNo = "MER#" + YearNow + "/" + ItemGroup + "-001";
                    }
                    else
                    {
                        ProjectRef_Autorun = Int32.Parse(chkYear) + 1;
                        ProjectRefNo = "MER#" + YearNow + "/" + ItemGroup + "-" + ProjectRef_Autorun.ToString().PadLeft(3, '0');
                    }
                    //UPDATE LAB Test
                    var LABTestLineOLDRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var UpdateLABLine = await LABTestLineOLDRepo.GetAsync(LABLINEID);
                    UpdateLABLine.PROJECTREFNO = ProjectRefNo;
                    await LABTestLineOLDRepo.UpdateAsync(UpdateLABLine);

                    //INSERT TRIAL TEST HEAD TABLE
                    var TrialTestHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                    var TrialTestHeadInsert = new S2ETrialTestHead_TB
                    {
                        RMREQID = RMREQID,
                        REQUESTDATE = RequestDate
                    };
                    var TrialID = await TrialTestHeadRepo.InsertAsync(TrialTestHeadInsert);

                    //INSERT TRIAL TEST LINE TABLE
                    var TrialTestLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                    var TrialTestLineInsert = new S2ETrialTestLine_TB
                    {
                        TRIALID = (int)TrialID,
                        PROJECTREFNO = ProjectRefNo,
                        CHEMICALNAME = ChemicalName,
                        PLANTCODED1D2 = PlantCodeD1D2,
                        PLANTCODED3 = PlantCodeD3,
                        PLANTCODED4 = PlantCodeD4,
                        PLANTCODED5 = PlantCodeD5,
                        CHEMICALNAMEREF = NewRequestByID.ISCOMPAIRE == 1 ? ChemicalNameRef : null,
                        PLANTCODED1D2REF = NewRequestByID.ISCOMPAIRE == 1 ? PlantCodeD1D2Ref : null,
                        PLANTCODED3REF = NewRequestByID.ISCOMPAIRE == 1 ? PlantCodeD3Ref : null,
                        PLANTCODED4REF = NewRequestByID.ISCOMPAIRE == 1 ? PlantCodeD4Ref : null,
                        PLANTCODED5REF = NewRequestByID.ISCOMPAIRE == 1 ? PlantCodeD5Ref : null,
                        TESTRESULT = TestResult == 1 ? 1 : 0,
                        APPROVALFORID = ApprovalForID,
                        PLANT = Plant,
                        APPROVEMASTERID = approvemasterid,
                        APPROVEGROUPID = ApproveGroupID,
                        CURRENTAPPROVESTEP = 1,
                        APPROVESTATUS = ApproveStatus,
                        ISACTIVE = 1,
                        CREATEBY = CreateBy,
                        CREATEDATE = RequestDate,
                        ISCURRENTLOGS = 1
                    };
                    var TrialLineID = await TrialTestLineRepo.InsertAsync(TrialTestLineInsert);

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
                                TRIALID = (int)TrialID,
                                TRIALLINEID = (int)TrialLineID,
                                DOCID = str,
                                DOCDESCRIPTION = DocRef_Name,
                                REMARK = Remark,
                                CREATEBY = CreateBy,
                                CREATEDATE = RequestDate
                            });
                        }
                    }

                    //INSERT LAB TEST RESULT
                    var S2ETrialEvaluationMasterRepo = new GenericRepository<S2EMaster_TrialTestEvaluation_TB>(unitOfWork.Transaction);
                    var S2ETrialEvaluationMasterALL = await S2ETrialEvaluationMasterRepo.GetAllAsync();
                    var S2ELogsTrialResultRepo = new GenericRepository<S2ETrialTestLogsTestResult_TB>(unitOfWork.Transaction);
                    //Lab Evaluation
                    int row = TrialResultID.Count();
                    for (int i = 0; i < row; i++)
                    {
                        var TRIALResultID = TrialResultID[i];
                        var S2ETrialResultMaster = S2ETrialEvaluationMasterALL.Where(x => x.ISACTIVE == 1 && x.ID == (TRIALResultID)).FirstOrDefault();
                        var TRIALDesc = S2ETrialResultMaster.TRIALRESULTDESC;
                        var isRemark1 = S2ETrialResultMaster.ISREMARK1;
                        var isRemark2 = S2ETrialResultMaster.ISREMARK2;
                        var TrialEvaluationID = S2ETrialResultMaster.ID;
                        var Remark1 = ""; var Remark2 = "";
                        if (isRemark1 == 1 && isRemark2 == 1)
                        {
                            Remark1 = TRIALTestResultRemark1;
                            Remark2 = TRIALTestResultRemark2;
                        }

                        await S2ELogsTrialResultRepo.InsertAsync(new S2ETrialTestLogsTestResult_TB
                        {
                            TRIALID = (int)TrialID,
                            TRIALLINEID = (int)TrialLineID,
                            TRIALEVALUATIONID = TrialEvaluationID,
                            TRIALEVALUATIONDESC = TRIALDesc,
                            ISPASS = LabIsPass[i],
                            REMARK1 = Remark1,
                            REMARK2 = Remark2,
                            CREATEBY = CreateBy,
                            CREATEDATE = RequestDate
                        });
                    }

                    //INSERT PROCESS TEST RESULT
                    var S2EProcResultMasterRepo = new GenericRepository<S2EMaster_TRIALTestProcEvaluation_TB>(unitOfWork.Transaction);
                    var S2EProcResultMasterALL = await S2EProcResultMasterRepo.GetAllAsync();
                    var S2ELogsProcResultRepo = new GenericRepository<S2ETrialTestLogsProcTestResult_TB>(unitOfWork.Transaction);
                    //Process Evaluation
                    int row2 = ProcResultID.Count();
                    for (int i = 0; i < row2; i++)
                    {
                        var ProcessResultID = ProcResultID[i];
                        var S2EProcResultMaster = S2EProcResultMasterALL.Where(x => x.ISACTIVE == 1 && x.ID == (ProcessResultID)).FirstOrDefault();
                        var ProcDesc = S2EProcResultMaster.PROCESSDESC;
                        var ProcID = S2EProcResultMaster.ID;

                        await S2ELogsProcResultRepo.InsertAsync(new S2ETrialTestLogsProcTestResult_TB
                        {
                            TRIALID = (int)TrialID,
                            TRIALLINEID = (int)TrialLineID,
                            PROCESSID = ProcID,
                            PROCESSDESC = ProcDesc,
                            ISPASS = ProcIsPass[i],
                            CREATEBY = CreateBy,
                            CREATEDATE = RequestDate
                        });
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
                                TRIALID = (int)TrialID,
                                TRIALLINEID = (int)TrialLineID,
                                PRODUCTTESTDESC = ProdList[i],
                                ISPASS = IsPass,
                                CREATEBY = CreateBy,
                                CREATEDATE = RequestDate
                            });
                        }

                    }

                    ////UPLOAD FILE 
                    var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                       NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);
                    int rowfile = FileUpload.Count();
                    string basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/{(int)TrialLineID}";
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }
                    //var fileok = new List<string>();
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
                                    TRIALID = (int)TrialID,
                                    TRIALLINEID = (int)TrialLineID,
                                    FILENAME = fileName,
                                    CREATEBY = CreateBy,
                                    CREATEDATE = RequestDate
                                });
                            }

                        }
                    }

                    //Save 
                    if (!string.IsNullOrEmpty(save))
                    {
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
                        var TrialTestAppTranRepo = new GenericRepository<S2ETrialTestApproveTrans_TB>(unitOfWork.Transaction);
                        foreach (var AppFlow in approveFlow_data)
                        {

                            await TrialTestAppTranRepo.InsertAsync(new S2ETrialTestApproveTrans_TB
                            {
                                TRIALID = (int)TrialID,
                                TRIALLINEID = (int)TrialLineID,
                                APPROVEMASTERID = AppFlow.ApproveMasterId,
                                APPROVEGROUPID = ApproveGroupID,
                                EMAIL = AppFlow.Email,
                                APPROVELEVEL = AppFlow.ApproveLevel,
                                ISCURRENTAPPROVE = 1
                            });
                        }

                        var currentRecord = await TrialTestLineRepo.GetAsync((int)TrialLineID);
                        currentRecord.CURRENTAPPROVESTEP = 1;
                        await TrialTestLineRepo.UpdateAsync(currentRecord);

                        //GET APPROVE TRANS LEVEL 1
                        var AppTransByRequestID = await unitOfWork.S2EControl.GetApproveTransByTrialID((int)TrialID, (int)TrialLineID, approvemasterid, ApproveGroupID);
                        var AppTransLevel1 = AppTransByRequestID.Where(x => x.APPROVELEVEL == 1);
                        foreach (var AppTrans in AppTransLevel1)
                        {
                            var approveFlowApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                       x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                       x.Email == AppTrans.EMAIL);

                            var FName = approveFlowApproveBy.Select(s => s.Name).FirstOrDefault();
                            var LName = approveFlowApproveBy.Select(s => s.LastName).FirstOrDefault();
                            var ApproveBy = FName + " " + LName;

                            var AppTransByALL = await TrialTestAppTranRepo.GetAllAsync();
                            var AppTransByID = AppTransByALL.Where(x => x.ID == AppTrans.ID).FirstOrDefault();

                            AppTransByID.SENDEMAILDATE = RequestDate;
                            await TrialTestAppTranRepo.UpdateAsync(AppTransByID);

                            var Subject = "";
                            var Body = "";
                            if (TestResult == 1)
                            {
                                Subject = "��ҹ��÷��ͺ";
                                Body = "<b style='color:green'> ��ҹ��÷��ͺ </b>";
                            }
                            else
                            {
                                Subject = "����ҹ��÷��ͺ";
                                Body = "<b style='color:red'> ����ҹ��÷��ͺ </b>";
                            }

                            var BodyEmail = "";
                            if (NewRequestByID.ISCOMPAIRE == 1)
                            {
                                var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                                var NewRequestCompaireALL = await NewRequestCompaireRepo.GetAllAsync();
                                var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == RequestID).FirstOrDefault();

                                BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> ��������´�����/�ѵ�شԺ (���������)  </b><br/>
                                   <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���ʼ����/����Ե :</b> {NewRequestCompaireByRequestID.VENDORIDREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���ͼ����/����Ե :</b> {NewRequestCompaireByRequestID.SUPPLIERNAMEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���᷹��˹��� :</b> {NewRequestCompaireByRequestID.DEALERREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���觼�Ե :</b> {NewRequestCompaireByRequestID.PRODUCTIONSITEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> �������͵��᷹��˹��� :</b> {NewRequestCompaireByRequestID.DEALERADDRESSREF.Replace("\n", "<br>")}
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
                                                <b> �Ҥ� :</b> {String.Format("{0:#,##0.#0}", NewRequestCompaireByRequestID.PRICEREF)}  {NewRequestCompaireByRequestID.CURRENCYCODEREF}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b> ��¡���ѵ�شԺ������� / �������º��º  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���ʼ����/����Ե :</b> {AddRMHeadByID.VENDORID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���ͼ����/����Ե :</b> {NewRequestByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���᷹��˹��� :</b> {AddRMHeadByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���觼�Ե :</b> {NewRequestByID.PRODUCTIONSITE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> �������͵��᷹��˹��� :</b> {NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}
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
                                                <b> �Ҥ� :</b> {String.Format("{0:#,##0.#0}", AddRMLineByID.PRICE)}  {AddRMHeadByID.CURRENCYCODE}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>�š�÷��ͺ : </b> {Body}
                                    <br/>
                                    <b>Link ���ʹ��Թ���:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/TRIALTestTodolist?Email={AppTrans.EMAIL}'> ��ԡ����� </a>
                                    <br/>
                                ";
                            }
                            else
                            {
                                BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> ��¡���ѵ�شԺ������� / �������º��º  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���ʼ����/����Ե :</b> {AddRMHeadByID.VENDORID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���ͼ����/����Ե :</b> {NewRequestByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���᷹��˹��� :</b> {AddRMHeadByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ���觼�Ե :</b> {NewRequestByID.PRODUCTIONSITE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> �������͵��᷹��˹��� :</b> {NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}
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
                                                <b> �Ҥ� :</b> {String.Format("{0:#,##0.#0}", AddRMLineByID.PRICE)}  {AddRMHeadByID.CURRENCYCODE}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>�š�÷��ͺ : </b> {Body}
                                    <br/>
                                    <b>Link ���ʹ��Թ���:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/TRIALTestTodolist?Email={AppTrans.EMAIL}'> ��ԡ����� </a>
                                    <br/>
                                ";
                            }

                            var sendEmail = _emailService.SendEmail(
                                   $"{NewRequestByID.REQUESTCODE} / �駼š�÷��ͺ�ѵ�شԺ Trial / {Subject}",
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
                    AlertSuccess = "Create Trial test Success";
                    return Redirect("/S2E/Qtech/TRIALTest/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/TRIALTest/" + RMREQID + "/Create");
            }
        }
    }
}
