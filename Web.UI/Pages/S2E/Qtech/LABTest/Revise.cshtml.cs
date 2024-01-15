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

namespace Web.UI.Pages.S2E.Qtech.LABTest
{
    public class ReviseModel : PageModel
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
        public List<IFormFile> FileUpload { get; set; }
        [BindProperty]
        public int LabID { get; set; }
        [BindProperty]
        public int LabLineID { get; set; }
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


        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public ReviseModel(
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
        public async Task<IActionResult> OnGetAsync(int LABID, int LABLINEID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_LABTEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    DepartmentMaster = await GetDepartmentMaster();
                    ResonTestMaster = await GetResonTestMaster();
                    TypeOfRMMaster = await GetTypeOfRMMaster();
                    //ApprovalForIDMaster = await GetApprovalForIDMaster();

                    LabID = LABID;
                    LabLineID = LABLINEID;
                    await GetData(LABID, LABLINEID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/LABTest/Main");
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
        public async Task<JsonResult> OnPostItemAXGridAsync(string ItemGroup)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        ITEMID = "ITEMID",
                        ITEMNAME = "ITEMNAME"
                    };

                    var filter = _datatableService.Filter(Request, field);

                    var ItemAXByGroup = await unitOfWork.Transaction.Connection.QueryAsync<GetItemCodeFromAXGridModel>(@"
                        SELECT ITEMGROUPID,ITEMID,ITEMNAME
                            FROM TB_MasterAX_Item
                            WHERE DATAAREAID = 'DV' AND
                                ITEMGROUPID = @ItemGroup AND
                                ITEMID LIKE 'I%'
                            AND " + filter + @" 
                        ",
                        new
                        {
                            @ItemGroup = ItemGroup
                        },
                        unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, ItemAXByGroup.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<JsonResult> OnPostItemSampleAXGridAsync(string ItemGroup)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        ITEMID = "ITEMID",
                        ITEMNAME = "ITEMNAME"
                    };

                    var filter = _datatableService.Filter(Request, field);

                    var ItemAXByGroup = await unitOfWork.Transaction.Connection.QueryAsync<GetItemCodeFromAXGridModel>(@"
                        SELECT ITEMGROUPID,ITEMID,ITEMNAME
                            FROM TB_MasterAX_Item
                            WHERE DATAAREAID = 'DV' AND
                                ITEMGROUPID = @ItemGroup AND
                                ITEMID LIKE 'S%'
                            AND " + filter + @" 
                        ",
                        new
                        {
                            @ItemGroup = ItemGroup
                        },
                        unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, ItemAXByGroup.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<JsonResult> OnPostLabEvaluationGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<LabTestResultGridViewModel>(@"
                        SELECT ID,LABRESULTDESC,ISACTIVE,
                            ISREMARK1 AS ISREMARKA,ISREMARK2 AS ISREMARKB
                            FROM TB_S2EMaster_LABTestEvaluation
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
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EMaster_LABTestProcEvaluation_TB>(@"
                        SELECT *
                            FROM TB_S2EMaster_LABTestProcEvaluation
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
        public async Task GetData(int LABID, int LABLINEID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                var LABTestLineByID = await LABTestLineRepo.GetAsync(LABLINEID);

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(LABTestHeadByID.REQUESTID);

                ItemGroup = LABTestHeadByID.ITEMGROUP;

                RequestCode = NewRequestByID.REQUESTCODE;
                ItemCode = NewRequestByID.ITEMCODE;
                TradeName = NewRequestByID.ITEMNAME;
                Country = NewRequestByID.PRODUCTIONSITE;
                Manufacture = NewRequestByID.SUPPLIERNAME;
                Agent = NewRequestByID.DEALER;
                isCompaire = NewRequestByID.ISCOMPAIRE;

                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentByID = await RMAssessmentRepo.GetAsync(LABTestHeadByID.ASSESSMENTID);

                PlantCodeD1D2 = RMAssessmentByID.PLANTCODED1D2;
                PlantCodeD3 = RMAssessmentByID.PLANTCODED3;
                PlantCodeD4 = RMAssessmentByID.PLANTCODED4;
                PlantCodeD5 = RMAssessmentByID.PLANTCODED5;

                Plant = RMAssessmentByID.PLANT;
                string[] PlantList = RMAssessmentByID.PLANT.Split(",");
                foreach (var d in PlantList)
                {
                    if (d == "DSL") { Plant1 = true; }
                    if (d == "DRB") { Plant2 = true; }
                    if (d == "DSI") { Plant3 = true; }
                    if (d == "DSR") { Plant4 = true; }
                    if (d == "STR") { Plant5 = true; }
                }


                if (NewRequestByID.ISCOMPAIRE == 1)
                {
                    var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                    var NewRequestCompaireALL = await NewRequestCompaireRepo.GetAllAsync();
                    var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == LABTestHeadByID.REQUESTID).FirstOrDefault();

                    ItemCodeRef = NewRequestCompaireByRequestID.ITEMCODEREF;
                    TradeNameRef = NewRequestCompaireByRequestID.ITEMNAMEREF;
                    CountryRef = NewRequestCompaireByRequestID.PRODUCTIONSITEREF;
                    ManufactureRef = NewRequestCompaireByRequestID.SUPPLIERNAMEREF;
                    AgentRef = NewRequestCompaireByRequestID.DEALERREF;

                    PlantCodeD1D2Ref = RMAssessmentByID.PLANTCODED1D2;
                    PlantCodeD3Ref = RMAssessmentByID.PLANTCODED3;
                    PlantCodeD4Ref = RMAssessmentByID.PLANTCODED4;
                    PlantCodeD5Ref = RMAssessmentByID.PLANTCODED5;

                }


                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostAsync(int LABID, int LABLINEID, string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                DepartmentMaster = await GetDepartmentMaster();
                ResonTestMaster = await GetResonTestMaster();
                TypeOfRMMaster = await GetTypeOfRMMaster();

                LabID = LABID;
                LabLineID = LABLINEID;
                await GetData(LABID, LABLINEID);

                return Page();
            }

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var DateTimeNow = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;
                    //LAB TEST Haead
                    var LabTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LabTestHeadALL = await LabTestHeadRepo.GetAllAsync();
                    var LabTestHeadByID = await LabTestHeadRepo.GetAsync(LABID);
                    //LAB TEST Line
                    var LabTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LabTestLineALL = await LabTestLineRepo.GetAllAsync();
                    var LabTestLineByID = await LabTestLineRepo.GetAsync(LABLINEID);

                    //UPDATE LAB TEST Line
                    var LABTestLineOLDLogs = LabTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1);
                    foreach (var updateOLDLogs in LABTestLineOLDLogs)
                    {
                        var LabTestLineOldUpdate = await LabTestLineRepo.GetAsync(updateOLDLogs.ID);
                        LabTestLineOldUpdate.ISCURRENTLOGS = 0;
                        LabTestLineOldUpdate.COMPLETEBY = CreateBy;
                        LabTestLineOldUpdate.COMPLETEDATE = DateTimeNow;
                        await LabTestLineRepo.UpdateAsync(LabTestLineOldUpdate);

                    }

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByCreateBy = approveMapALL.Where(x => x.CreateBy == CreateBy &&
                                                                   x.STEP == 1 &&
                                                                   x.ISLABTEST == 1
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


                    //UPDATE LAB TEST HEADER
                    LabTestHeadByID.REQUESTDATE = DateTimeNow;

                    var ProjectRefNo = "";
                    if (TestResult == 1)
                    {
                        ProjectRefNo = null;
                    }
                    else
                    {
                        //GENERATE PROJECT REF NO.
                        var YearNow = @DateTime.Now.ToString("yyyy", new CultureInfo("en-US"));
                        var LABTestNOTNULL = LabTestLineALL.Where(x => x.PROJECTREFNO != null);
                        var chkYear = LABTestNOTNULL.Where(x => x.PROJECTREFNO.Substring(4, 4) == YearNow)
                                                .Max(a => a.PROJECTREFNO.Substring(12, 3));
                        int ProjectRef_Autorun;
                        if (chkYear == null)
                        {
                            ProjectRefNo = "MER#" + YearNow + "/" + LabTestHeadByID.ITEMGROUP + "-001";
                        }
                        else
                        {
                            ProjectRef_Autorun = Int32.Parse(chkYear) + 1;
                            ProjectRefNo = "MER#" + YearNow + "/" + LabTestHeadByID.ITEMGROUP + "-" + ProjectRef_Autorun.ToString().PadLeft(3, '0');
                        }

                    }
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(LabTestHeadByID.REQUESTID);

                    //INSERT LAB TEST Line TABLE
                    var LabTestLineInsert = new S2ELABTestLine_TB
                    {
                        LABID = LABID,
                        PROJECTREFNO = ProjectRefNo,
                        DEPARTMENTID = DepartmentID,
                        DEPARTMENTREMARK = DepartmentRemark,
                        RESONTESTID = ResonTestID,
                        RESONTESTREMARK = ResonTestRemark,
                        TYPEOFRMID = TypeOfRMID,
                        CHEMICALNAME = ChemicalName,
                        CHEMICALNAMEREF = NewRequestByID.ISCOMPAIRE == 1 ? ChemicalNameRef : null,
                        ITEMCODE = ItemCode,
                        TRADENAME = TradeName,
                        COUNTRY = Country,
                        MANUFACTURE = Manufacture,
                        AGENT = Agent,
                        PLANTCODED1D2 = PlantCodeD1D2,
                        PLANTCODED3 = PlantCodeD3,
                        PLANTCODED4 = PlantCodeD4,
                        PLANTCODED5 = PlantCodeD5,
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
                        CREATEDATE = DateTimeNow,
                        ISCURRENTLOGS = 1
                    };
                    var LABTestLineID = await LabTestLineRepo.InsertAsync(LabTestLineInsert);

                    //INSERT LOGS DOC
                    var S2ELogsDocRepo = new GenericRepository<S2ELABTestLogsDoc_TB>(unitOfWork.Transaction);
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

                            await S2ELogsDocRepo.InsertAsync(new S2ELABTestLogsDoc_TB
                            {
                                LABID = LABID,
                                LABLINEID = (int)LABTestLineID,
                                DOCID = str,
                                DOCDESCRIPTION = DocRef_Name,
                                REMARK = Remark,
                                CREATEBY = CreateBy,
                                CREATEDATE = DateTimeNow
                            });
                        }
                    }

                    //INSERT LAB TEST RESULT
                    var S2ELabResultMasterRepo = new GenericRepository<S2EMaster_LABTestEvaluation_TB>(unitOfWork.Transaction);
                    var S2ELabResultMasterALL = await S2ELabResultMasterRepo.GetAllAsync();
                    var S2ELogsLABResultRepo = new GenericRepository<S2ELABTestLogsTestResult_TB>(unitOfWork.Transaction);
                    //Lab Evaluation
                    int row = LabResultID.Count();
                    for (int i = 0; i < row; i++)
                    {
                        var LABResultID = LabResultID[i];
                        var S2ELabResultMaster = S2ELabResultMasterALL.Where(x => x.ISACTIVE == 1 && x.ID == (LABResultID)).FirstOrDefault();
                        var LABDesc = S2ELabResultMaster.LABRESULTDESC;
                        var isRemark1 = S2ELabResultMaster.ISREMARK1;
                        var isRemark2 = S2ELabResultMaster.ISREMARK2;
                        var LABEvaluationID = S2ELabResultMaster.ID;
                        var Remark1 = ""; var Remark2 = "";
                        if (isRemark1 == 1 && isRemark2 == 1)
                        {
                            Remark1 = LABTestResultRemark1;
                            Remark2 = LABTestResultRemark2;
                        }

                        await S2ELogsLABResultRepo.InsertAsync(new S2ELABTestLogsTestResult_TB
                        {
                            LABID = LABID,
                            LABLINEID = (int)LABTestLineID,
                            LABEVALUATIONID = LABEvaluationID,
                            LABEVALUATIONDESC = LABDesc,
                            ISPASS = LabIsPass[i],
                            REMARK1 = Remark1,
                            REMARK2 = Remark2,
                            CREATEBY = CreateBy,
                            CREATEDATE = DateTimeNow
                        });
                    }

                    //INSERT PROCESS TEST RESULT
                    var S2EProcResultMasterRepo = new GenericRepository<S2EMaster_LABTestProcEvaluation_TB>(unitOfWork.Transaction);
                    var S2EProcResultMasterALL = await S2EProcResultMasterRepo.GetAllAsync();
                    var S2ELogsProcResultRepo = new GenericRepository<S2ELABTestLogsProcTestResult_TB>(unitOfWork.Transaction);
                    //Process Evaluation
                    int row2 = ProcResultID.Count();
                    for (int i = 0; i < row2; i++)
                    {
                        var ProcessResultID = ProcResultID[i];
                        var S2EProcResultMaster = S2EProcResultMasterALL.Where(x => x.ISACTIVE == 1 && x.ID == (ProcessResultID)).FirstOrDefault();
                        var ProcDesc = S2EProcResultMaster.PROCESSDESC;
                        var ProcID = S2EProcResultMaster.ID;

                        await S2ELogsProcResultRepo.InsertAsync(new S2ELABTestLogsProcTestResult_TB
                        {
                            LABID = LABID,
                            LABLINEID = (int)LABTestLineID,
                            PROCESSID = ProcID,
                            PROCESSDESC = ProcDesc,
                            ISPASS = ProcIsPass[i],
                            CREATEBY = CreateBy,
                            CREATEDATE = DateTimeNow
                        });
                    }

                    //INSERT NEW PRODUCTION TEST
                    var S2ELogsProdResultInsertRepo = new GenericRepository<S2ELABTestLogsPrdTestResult_TB>(unitOfWork.Transaction);
                    int row4 = ProdIsPass.Count();
                    //Product Evaluartion
                    for (int i = 0; i < row4; i++)
                    {
                        if (ProdList[i] != null && ProdIsPass != null)
                        {
                            var IsPass = ProdIsPass[i] == "1" ? 1 : 0;
                            await S2ELogsProdResultInsertRepo.InsertAsync(new S2ELABTestLogsPrdTestResult_TB
                            {
                                LABID = LABID,
                                LABLINEID = (int)LABTestLineID,
                                PRODUCTTESTDESC = ProdList[i],
                                ISPASS = IsPass,
                                CREATEBY = CreateBy,
                                CREATEDATE = DateTimeNow
                            });
                        }

                    }

                    //UPLOAD FILE 
                    var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                       NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);
                    int rowfile = FileUpload.Count();
                    string basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/LABTest/{(int)LABTestLineID}";
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }
                    //var fileok = new List<string>();
                    var filePath = Path.GetTempFileName();
                    string fileName = "";
                    var LABTestLogsFileRepo = new GenericRepository<S2ELABTestLogsFile_TB>(unitOfWork.Transaction);
                    for (int i = 0; i < rowfile; i++)
                    {
                        if (FileUpload[i] != null)
                        {
                            fileName = Path.GetFileName(FileUpload[i].FileName);
                            using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                            {
                                await FileUpload[i].CopyToAsync(stream);
                                await LABTestLogsFileRepo.InsertAsync(new S2ELABTestLogsFile_TB
                                {
                                    LABID = LABID,
                                    LABLINEID = (int)LABTestLineID,
                                    FILENAME = fileName,
                                    CREATEBY = CreateBy,
                                    CREATEDATE = DateTimeNow
                                });
                            }

                        }
                    }

                    await LabTestHeadRepo.UpdateAsync(LabTestHeadByID);

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
                        var nonceRepo = new GenericRepository<S2ELABTestNonce_TB>(unitOfWork.Transaction);
                        await nonceRepo.InsertAsync(new S2ELABTestNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = DateTimeNow,
                            EXPIREDATE = DateTimeNow.AddDays(7),
                            ISUSED = 0
                        });

                        // INSERT APPROVE TRANSECTION
                        var LABTestAppTranRepo = new GenericRepository<S2ELABTestApproveTrans_TB>(unitOfWork.Transaction);
                        foreach (var AppFlow in approveFlow_data)
                        {

                            await LABTestAppTranRepo.InsertAsync(new S2ELABTestApproveTrans_TB
                            {
                                LABID = LABID,
                                LABLINEID = (int)LABTestLineID,
                                APPROVEMASTERID = AppFlow.ApproveMasterId,
                                APPROVEGROUPID = ApproveGroupID,
                                EMAIL = AppFlow.Email,
                                APPROVELEVEL = AppFlow.ApproveLevel,
                                ISCURRENTAPPROVE = 1,
                                ISKEYINWHENAPPROVE = AppFlow.IsKeyinWhenApprove
                            });
                        }

                        var currentRecord = await LabTestLineRepo.GetAsync((int)LABTestLineID);
                        currentRecord.CURRENTAPPROVESTEP = 1;
                        await LabTestLineRepo.UpdateAsync(currentRecord);

                        //GET APPROVE TRANS LEVEL 1
                        var AppTransByRequestID = await unitOfWork.S2EControl.GetApproveTransByLABID(LABID, (int)LABTestLineID, approvemasterid, ApproveGroupID);
                        var AppTransLevel1 = AppTransByRequestID.Where(x => x.APPROVELEVEL == 1);
                        foreach (var AppTrans in AppTransLevel1)
                        {
                            var approveFlowApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                       x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                       x.Email == AppTrans.EMAIL);

                            var FName = approveFlowApproveBy.Select(s => s.Name).FirstOrDefault();
                            var LName = approveFlowApproveBy.Select(s => s.LastName).FirstOrDefault();
                            var ApproveBy = FName + " " + LName;

                            var AppTransByALL = await LABTestAppTranRepo.GetAllAsync();
                            var AppTransByID = AppTransByALL.Where(x => x.ID == AppTrans.ID).FirstOrDefault();

                            AppTransByID.SENDEMAILDATE = DateTimeNow;
                            await LABTestAppTranRepo.UpdateAsync(AppTransByID);

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
                                var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == LabTestHeadByID.REQUESTID).FirstOrDefault();

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
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/LABTestTodolist?Email={AppTrans.EMAIL}'> คลิกที่นี่ </a>
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
                                    <b>ผลการทดสอบ : </b> {Body}
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/LABTestTodolist?Email={AppTrans.EMAIL}'> คลิกที่นี่ </a>
                                    <br/>
                                ";
                            }

                            var sendEmail = _emailService.SendEmail(
                                   $"{NewRequestByID.REQUESTCODE} / แจ้งผลการทดสอบวัตถุดิบ Lab / {Subject}",
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
                    AlertSuccess = "Create Lab test Success";
                    return Redirect("/S2E/Qtech/LABTest/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/LABTest/" + LABID + "/" + LABLINEID + "/Revise");
            }
        }
    }
}
