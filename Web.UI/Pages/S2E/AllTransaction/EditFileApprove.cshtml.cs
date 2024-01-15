using System;
using System.Collections.Generic;
using System.IO;
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
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.AllTransaction
{
    public class EditFileApproveModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int GroupID { get; set; }
        [BindProperty]
        public int RequestId { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public int ID { get; set; }
        [BindProperty]
        public string GroupDescription { get; set; }
        [BindProperty]
        public string Remark { get; set; }
        [BindProperty]
        public int ApproveResult { get; set; }
        [BindProperty]
        public string ApproveRemark { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public EditFileApproveModel(
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
        public async Task<IActionResult> OnGetAsync(int RequestID, int TranID, string nonce, string email)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var _nonce = await unitOfWork.S2EControl.GetLogFileNonceByKey(nonce);

                    if (_nonce.ISUSED == 1)
                    {
                        AlertError = "Link Is Used.";
                        return Redirect($"/S2E/AllTransaction/EditFileTodolist?Email={email}");
                    }
                    await GetData(RequestID);


                    return Page();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task GetData(int RequestID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                RequestId = RequestID;

                var LogFileHeadRepo = new GenericRepository<S2ELogFileEditHead_TB>(unitOfWork.Transaction);
                var LogFileHeadByID = await LogFileHeadRepo.GetAsync(RequestID);

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(LogFileHeadByID.REQUESTID);

                var ApproveGroupRepo = new GenericRepository<S2EApproveGroup_TB>(unitOfWork.Transaction);
                var ApproveGroupByID = await ApproveGroupRepo.GetAsync(LogFileHeadByID.APPROVEGROUPID);

                GroupID = LogFileHeadByID.APPROVEGROUPID;
                RequestCode = NewRequestByID.REQUESTCODE;
                GroupDescription = ApproveGroupByID.GROUPDESCRIPTION;

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostGridViewFileUploadAsync(int headid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ELogFileEditGridViewModel>(@"
                            EXEC S2EAllTransactionEditFileApprove @HeadID
                        ",
                        new
                        {
                            @HeadID = headid
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
        public async Task<IActionResult> OnPostGridViewFileUploadNewAsync(int headid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ELogFileEditGridViewModel>(@"
                        SELECT LH.APPROVEGROUPID AS GROUPID
			                ,LH.ID AS ID
			                ,LH.REQUESTID
			                ,O.ID AS FILEID
			                ,O.FILENAME 
			                ,O.ISDELETE
			                ,CONVERT(NVARCHAR,O.CREATEDATE,103) + ' ' + CONVERT(NVARCHAR,O.CREATEDATE,108) AS CREATEDATE
			            FROM TB_S2ELogFileEditLine O JOIN
			            TB_S2ELogFileEditHead LH ON O.LOGFILEHEADID = LH.ID
			            WHERE O.LOGFILEHEADID = @HeadID AND O.ISDELETE = 0 AND O.ISCURRENTLOGS = 1
                        ",
                        new
                        {
                            @HeadID = headid
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
        public async Task<IActionResult> OnGetDownloadFileUploadAsync(int HeadID, int fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var LogFileEditHeadRepo = new GenericRepository<S2ELogFileEditHead_TB>(unitOfWork.Transaction);
                    var LogFileEditHeadByID = await LogFileEditHeadRepo.GetAsync(HeadID);

                    var requestid = LogFileEditHeadByID.REQUESTID;
                    var groupid = LogFileEditHeadByID.APPROVEGROUPID;

                    RequestId = requestid;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(requestid);

                    string basePath = "";
                    var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                       NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);
                    var fileName = "";

                    if (groupid == 1)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/NewRequest";
                        var LogFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 2)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/RMAssessment";
                        var LogFileRepo = new GenericRepository<S2ERMAssessmentLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 3)
                    {
                        var LABHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                        var LABHeadALL = await LABHeadRepo.GetAllAsync();
                        var LABHeadByRequestID = LABHeadALL.Where(x => x.REQUESTID == requestid).Select(x=>x.ID).FirstOrDefault();

                        var LABLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                        var LABLineALL = await LABLineRepo.GetAllAsync();

                        var LABLineID = LABLineALL.Where(x => x.LABID == LABHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/LABTest/{(int)LABLineID}";
                        var LogFileRepo = new GenericRepository<S2ELABTestLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 4)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/PurchaseSample";
                        var LogFileRepo = new GenericRepository<S2EPurchaseSampleLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 5)
                    {
                        var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                        var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                        var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                        var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                        var AddRMLineID = AddRMLineALL.Where(x => x.ADDRMID == AddRMHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{(int)AddRMLineID}";
                        var LogFileRepo = new GenericRepository<S2EAddRawMaterialLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 8)
                    {
                        var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                        var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                        var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        var RMReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                        var RMReqHeadALL = await RMReqHeadRepo.GetAllAsync();
                        var RMReqHeadByAddRMID = RMReqHeadALL.Where(x => x.ADDRMID == AddRMHeadByRequestID).Select(x => x.ID).FirstOrDefault();

                        var TrialHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                        var TrialHeadALL = await TrialHeadRepo.GetAllAsync();
                        var TrialHeadByRMReqID= TrialHeadALL.Where(x => x.RMREQID == RMReqHeadByAddRMID).Select(x => x.ID).FirstOrDefault();

                        var TrialLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                        var TrialLineALL = await TrialLineRepo.GetAllAsync();
                        var TrialLineID = TrialLineALL.Where(x => x.TRIALID == TrialHeadByRMReqID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/{(int)TrialLineID}";
                        var LogFileRepo = new GenericRepository<S2ETrialTestLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 9)
                    {
                        var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                        var AddRMSampleALL = await AddRMSampleRepo.GetAllAsync();
                        var AddRMSampleByRequestID = AddRMSampleALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{AddRMSampleByRequestID}";
                        var LogFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else
                    {
                        AlertError = "มีข้อผิดพลาด ติดต่อ Admin";
                        return Redirect($"/");
                    }

                    basePath = $"{basePath}/{fileName}";

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
        public async Task<IActionResult> OnGetViewFileUploadAsync(int HeadID, int fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var LogFileEditHeadRepo = new GenericRepository<S2ELogFileEditHead_TB>(unitOfWork.Transaction);
                    var LogFileEditHeadByID = await LogFileEditHeadRepo.GetAsync(HeadID);

                    var requestid = LogFileEditHeadByID.REQUESTID;
                    var groupid = LogFileEditHeadByID.APPROVEGROUPID;

                    RequestId = requestid;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(requestid);

                    string basePath = "";
                    var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                       NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);
                    var fileName = "";

                    if (groupid == 1)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/NewRequest";
                        var LogFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 2)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/RMAssessment";
                        var LogFileRepo = new GenericRepository<S2ERMAssessmentLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 3)
                    {
                        var LABHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                        var LABHeadALL = await LABHeadRepo.GetAllAsync();
                        var LABHeadByRequestID = LABHeadALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        var LABLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                        var LABLineALL = await LABLineRepo.GetAllAsync();

                        var LABLineID = LABLineALL.Where(x => x.LABID == LABHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/LABTest/{(int)LABLineID}";
                        var LogFileRepo = new GenericRepository<S2ELABTestLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 4)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/PurchaseSample";
                        var LogFileRepo = new GenericRepository<S2EPurchaseSampleLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 5)
                    {
                        var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                        var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                        var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                        var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                        var AddRMLineID = AddRMLineALL.Where(x => x.ADDRMID == AddRMHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{(int)AddRMLineID}";
                        var LogFileRepo = new GenericRepository<S2EAddRawMaterialLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 8)
                    {
                        var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                        var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                        var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        var RMReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                        var RMReqHeadALL = await RMReqHeadRepo.GetAllAsync();
                        var RMReqHeadByAddRMID = RMReqHeadALL.Where(x => x.ADDRMID == AddRMHeadByRequestID).Select(x => x.ID).FirstOrDefault();

                        var TrialHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                        var TrialHeadALL = await TrialHeadRepo.GetAllAsync();
                        var TrialHeadByRMReqID = TrialHeadALL.Where(x => x.RMREQID == RMReqHeadByAddRMID).Select(x => x.ID).FirstOrDefault();

                        var TrialLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                        var TrialLineALL = await TrialLineRepo.GetAllAsync();
                        var TrialLineID = TrialLineALL.Where(x => x.TRIALID == TrialHeadByRMReqID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/{(int)TrialLineID}";
                        var LogFileRepo = new GenericRepository<S2ETrialTestLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 9)
                    {
                        var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                        var AddRMSampleALL = await AddRMSampleRepo.GetAllAsync();
                        var AddRMSampleByRequestID = AddRMSampleALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{AddRMSampleByRequestID}";
                        var LogFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else
                    {
                        AlertError = "มีข้อผิดพลาด ติดต่อ Admin";
                        return Redirect($"/");
                    }

                    basePath = $"{basePath}/{fileName}";

                    if (!System.IO.File.Exists(basePath))
                    {
                        throw new Exception("File not found.");
                    }

                    var stream = new FileStream(basePath, FileMode.Open);
                    return new FileStreamResult(stream, "application/pdf");

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnGetDownloadFileUploadNewAsync(int HeadID, int fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var LogFileEditHeadRepo = new GenericRepository<S2ELogFileEditHead_TB>(unitOfWork.Transaction);
                    var LogFileEditHeadByID = await LogFileEditHeadRepo.GetAsync(HeadID);

                    var requestid = LogFileEditHeadByID.REQUESTID;
                    var groupid = LogFileEditHeadByID.APPROVEGROUPID;

                    RequestId = requestid;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(requestid);

                    string basePath = "";
                    var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                       NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);
                    var fileName = "";

                    if (groupid == 1)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/NewRequest/tempEditFile";
                    }
                    else if (groupid == 2)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/RMAssessment/tempEditFile";
                    }
                    else if (groupid == 3)
                    {
                        var LABHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                        var LABHeadALL = await LABHeadRepo.GetAllAsync();
                        var LABHeadByRequestID = LABHeadALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        var LABLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                        var LABLineALL = await LABLineRepo.GetAllAsync();

                        var LABLineID = LABLineALL.Where(x => x.LABID == LABHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/LABTest/{(int)LABLineID}/tempEditFile";
                    }
                    else if (groupid == 4)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/PurchaseSample/tempEditFile";
                    }
                    else if (groupid == 5)
                    {
                        var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                        var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                        var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                        var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                        var AddRMLineID = AddRMLineALL.Where(x => x.ADDRMID == AddRMHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{(int)AddRMLineID}/tempEditFile";
                    }
                    else if (groupid == 8)
                    {
                        var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                        var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                        var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        var RMReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                        var RMReqHeadALL = await RMReqHeadRepo.GetAllAsync();
                        var RMReqHeadByAddRMID = RMReqHeadALL.Where(x => x.ADDRMID == AddRMHeadByRequestID).Select(x => x.ID).FirstOrDefault();

                        var TrialHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                        var TrialHeadALL = await TrialHeadRepo.GetAllAsync();
                        var TrialHeadByRMReqID = TrialHeadALL.Where(x => x.RMREQID == RMReqHeadByAddRMID).Select(x => x.ID).FirstOrDefault();

                        var TrialLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                        var TrialLineALL = await TrialLineRepo.GetAllAsync();
                        var TrialLineID = TrialLineALL.Where(x => x.TRIALID == TrialHeadByRMReqID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/{(int)TrialLineID}/tempEditFile";
                    }
                    else if (groupid == 9)
                    {
                        var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                        var AddRMSampleALL = await AddRMSampleRepo.GetAllAsync();
                        var AddRMSampleByRequestID = AddRMSampleALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{AddRMSampleByRequestID}/tempEditFile";
                    }
                    else
                    {
                        AlertError = "มีข้อผิดพลาด ติดต่อ Admin";
                        return Redirect($"/");
                    }

                    var LogFileRepo = new GenericRepository<S2ELogFileEditLine_TB>(unitOfWork.Transaction);
                    var LogFileByID = await LogFileRepo.GetAsync(fileid);
                    fileName = LogFileByID.FILENAME;

                    basePath = $"{basePath}/{fileName}";


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
        public async Task<IActionResult> OnGetViewFileUploadNewAsync(int HeadID, int fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var LogFileEditHeadRepo = new GenericRepository<S2ELogFileEditHead_TB>(unitOfWork.Transaction);
                    var LogFileEditHeadByID = await LogFileEditHeadRepo.GetAsync(HeadID);

                    var requestid = LogFileEditHeadByID.REQUESTID;
                    var groupid = LogFileEditHeadByID.APPROVEGROUPID;

                    RequestId = requestid;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(requestid);

                    string basePath = "";
                    var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                       NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);
                    var fileName = "";

                    if (groupid == 1)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/NewRequest/tempEditFile";
                    }
                    else if (groupid == 2)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/RMAssessment/tempEditFile";
                    }
                    else if (groupid == 3)
                    {
                        var LABHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                        var LABHeadALL = await LABHeadRepo.GetAllAsync();
                        var LABHeadByRequestID = LABHeadALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        var LABLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                        var LABLineALL = await LABLineRepo.GetAllAsync();

                        var LABLineID = LABLineALL.Where(x => x.LABID == LABHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/LABTest/{(int)LABLineID}/tempEditFile";
                    }
                    else if (groupid == 4)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/PurchaseSample/tempEditFile";
                    }
                    else if (groupid == 5)
                    {
                        var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                        var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                        var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                        var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                        var AddRMLineID = AddRMLineALL.Where(x => x.ADDRMID == AddRMHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{(int)AddRMLineID}/tempEditFile";
                    }
                    else if (groupid == 8)
                    {
                        var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                        var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                        var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        var RMReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                        var RMReqHeadALL = await RMReqHeadRepo.GetAllAsync();
                        var RMReqHeadByAddRMID = RMReqHeadALL.Where(x => x.ADDRMID == AddRMHeadByRequestID).Select(x => x.ID).FirstOrDefault();

                        var TrialHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                        var TrialHeadALL = await TrialHeadRepo.GetAllAsync();
                        var TrialHeadByRMReqID = TrialHeadALL.Where(x => x.RMREQID == RMReqHeadByAddRMID).Select(x => x.ID).FirstOrDefault();

                        var TrialLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                        var TrialLineALL = await TrialLineRepo.GetAllAsync();
                        var TrialLineID = TrialLineALL.Where(x => x.TRIALID == TrialHeadByRMReqID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/{(int)TrialLineID}/tempEditFile";
                    }
                    else if (groupid == 9)
                    {
                        var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                        var AddRMSampleALL = await AddRMSampleRepo.GetAllAsync();
                        var AddRMSampleByRequestID = AddRMSampleALL.Where(x => x.REQUESTID == requestid).Select(x => x.ID).FirstOrDefault();

                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{AddRMSampleByRequestID}/tempEditFile";
                    }
                    else
                    {
                        AlertError = "มีข้อผิดพลาด ติดต่อ Admin";
                        return Redirect($"/");
                    }

                    var LogFileRepo = new GenericRepository<S2ELogFileEditLine_TB>(unitOfWork.Transaction);
                    var LogFileByID = await LogFileRepo.GetAsync(fileid);
                    fileName = LogFileByID.FILENAME;

                    basePath = $"{basePath}/{fileName}";

                    if (!System.IO.File.Exists(basePath))
                    {
                        throw new Exception("File not found.");
                    }

                    var stream = new FileStream(basePath, FileMode.Open);
                    return new FileStreamResult(stream, "application/pdf");

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnPostAsync(int RequestID, int TranID, string nonce, string email)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    if (!ModelState.IsValid)
                    {
                        await GetData(RequestID);
                        return Page();
                    }

                    var HeadID = RequestID;

                    var LogFileEditHeadRepo = new GenericRepository<S2ELogFileEditHead_TB>(unitOfWork.Transaction);
                    var LogFileEditHeadByID = await LogFileEditHeadRepo.GetAsync(HeadID);

                    var NewRequestID = LogFileEditHeadByID.REQUESTID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(NewRequestID);

                    RequestCode = NewRequestByID.REQUESTCODE;
                    var DatetimeNow = DateTime.Now;
                    int approvemasterid = LogFileEditHeadByID.APPROVEMASTERID;

                    //UPDATE OLD DATA
                    var nonceRepo = new GenericRepository<S2ELogFileEditNonce_TB>(unitOfWork.Transaction);
                    var _nonce = await unitOfWork.S2EControl.GetLogFileNonceByKey(nonce);
                    if (_nonce.ISUSED == 1)
                    {
                        throw new Exception("Link Is Used.");
                    }

                    _nonce.ISUSED = 1;

                    //UPDATE Approve Trans
                    var LogFileEditTransRepo = new GenericRepository<S2ELogFileEditApproveTrans_TB>(unitOfWork.Transaction);
                    var LogFileEditTransByID = await LogFileEditTransRepo.GetAsync(TranID);

                    var ApproveLevel = LogFileEditTransByID.APPROVELEVEL;
                    var ApproveGroupID = LogFileEditTransByID.APPROVEGROUPID;

                    var ApproveGroupRepo = new GenericRepository<S2EApproveGroup_TB>(unitOfWork.Transaction);
                    var ApproveGroupByID = await ApproveGroupRepo.GetAsync(ApproveGroupID);

                    var LogFileEditApproveTransRepo = new GenericRepository<S2ELogFileEditApproveTrans_TB>(unitOfWork.Transaction);
                    var LogFileEditApproveTransALL = await LogFileEditApproveTransRepo.GetAllAsync();
                    var LogFileEditApproveTransLevel = LogFileEditApproveTransALL.Where(x => x.LOGFILEHEADID == HeadID &&
                                                                    x.APPROVEMASTERID == approvemasterid &&
                                                                    x.APPROVELEVEL == ApproveLevel &&
                                                                    x.ISCURRENTAPPROVE == 1 &&
                                                                    x.APPROVEGROUPID == ApproveGroupID);

                    foreach (var UpdateApproveTrans in LogFileEditApproveTransLevel)
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
                        await LogFileEditApproveTransRepo.UpdateAsync(UpdateApproveTrans);
                    }


                    //GET REQUEST BY FULL NAME
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(LogFileEditHeadByID.CREATEBY);

                    //GET APPROVE TRANS ALL LEVEL
                    var ApproveTransAll = await unitOfWork.S2EControl.GetApproveTransLogFileEdit(ApproveGroupID, HeadID, approvemasterid);
                    var AllLevel = ApproveTransAll.ToList().Count;

                    var LABTestHeadId = 0;var LABTestLineId = 0;
                    var AddRMHeadId = 0; var AddRMLineId = 0;
                    var TRIALTestHeadId = 0; var TRIALTestLineId = 0;
                    var AddRMSampleId = 0;

                    //isFinal
                    if (LogFileEditHeadByID.CURRENTAPPROVESTEP == AllLevel && ApproveResult == 1)
                    {
                        if (LogFileEditHeadByID.COMPLETEDATE == null)
                        {
                            LogFileEditHeadByID.APPROVESTATUS = RequestStatusModel.Successfully;
                            LogFileEditHeadByID.COMPLETEDATE = DatetimeNow;

                            var LogFileLineRepo = new GenericRepository<S2ELogFileEditLine_TB>(unitOfWork.Transaction);
                            var LogFileLineALL = await LogFileLineRepo.GetAllAsync();



                            string basePath = "";
                            string newPath = "";
                            var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                               NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);
                            var fileName = "";

                            if (ApproveGroupID == 1)
                            {
                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/NewRequest";
                                newPath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/NewRequest/tempEditFile";
                            }
                            else if (ApproveGroupID == 2)
                            {
                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/RMAssessment";
                                newPath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/RMAssessment/tempEditFile";
                            }
                            else if (ApproveGroupID == 3)
                            {
                                var LABHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                                var LABHeadALL = await LABHeadRepo.GetAllAsync();
                                var LABHeadByRequestID = LABHeadALL.Where(x => x.REQUESTID == NewRequestID).Select(x => x.ID).FirstOrDefault();
                                
                                var LABLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                                var LABLineALL = await LABLineRepo.GetAllAsync();

                                var LABLineID = LABLineALL.Where(x => x.LABID == LABHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();

                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/LABTest/{(int)LABLineID}";
                                newPath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/LABTest/{(int)LABLineID}/tempEditFile";

                                LABTestHeadId = (int)LABHeadByRequestID;
                                LABTestLineId = (int)LABLineID;
                            }
                            else if (ApproveGroupID == 4)
                            {
                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/PurchaseSample";
                                newPath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/PurchaseSample/tempEditFile";
                            }
                            else if (ApproveGroupID == 5)
                            {
                                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                                var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                                var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == NewRequestID).Select(x => x.ID).FirstOrDefault();

                                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                                var AddRMLineID = AddRMLineALL.Where(x => x.ADDRMID == AddRMHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{(int)AddRMLineID}";
                                newPath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{(int)AddRMLineID}/tempEditFile";

                                AddRMHeadId = AddRMHeadByRequestID;
                                AddRMLineId = AddRMLineID;
                            }
                            else if (ApproveGroupID == 8)
                            {
                                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                                var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                                var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == NewRequestID).Select(x => x.ID).FirstOrDefault();

                                var RMReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                                var RMReqHeadALL = await RMReqHeadRepo.GetAllAsync();
                                var RMReqHeadByAddRMID = RMReqHeadALL.Where(x => x.ADDRMID == AddRMHeadByRequestID).Select(x => x.ID).FirstOrDefault();

                                var TrialHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                                var TrialHeadALL = await TrialHeadRepo.GetAllAsync();
                                var TrialHeadByRMReqID = TrialHeadALL.Where(x => x.RMREQID == RMReqHeadByAddRMID).Select(x => x.ID).FirstOrDefault();

                                var TrialLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                                var TrialLineALL = await TrialLineRepo.GetAllAsync();
                                var TrialLineID = TrialLineALL.Where(x => x.TRIALID == TrialHeadByRMReqID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/{(int)TrialLineID}";
                                newPath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/{(int)TrialLineID}/tempEditFile";


                                TRIALTestHeadId = (int)TrialHeadByRMReqID;
                                TRIALTestLineId = (int)TrialLineID;
                            }
                            else if (ApproveGroupID == 9)
                            {
                                var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                                var AddRMSampleALL = await AddRMSampleRepo.GetAllAsync();
                                var AddRMSampleByRequestID = AddRMSampleALL.Where(x => x.REQUESTID == NewRequestID).Select(x => x.ID).FirstOrDefault();

                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{AddRMSampleByRequestID}";
                                newPath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{AddRMSampleByRequestID}/tempEditFile";

                                AddRMSampleId = AddRMSampleByRequestID;
                            }
                            else
                            {
                                AlertError = "มีข้อผิดพลาด ติดต่อ Admin 1";
                                return Redirect($"/");
                            }

                            //oldpath
                            var LogFileOld = LogFileLineALL.Where(x=> x.LOGFILEHEADID == HeadID && x.FILEREFID != 0 && x.ISCURRENTLOGS == 1 && x.ISDELETE == 1);
                            foreach (var old in LogFileOld)
                            {
                                if (ApproveGroupID == 1)
                                {
                                    var NewRequestLogFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                                    var NewRequestLogFileByID = await NewRequestLogFileRepo.GetAsync(old.FILEREFID);
                                    await NewRequestLogFileRepo.DeleteAsync(NewRequestLogFileByID);
                                }
                                else if (ApproveGroupID == 2)
                                {
                                    var RMAssessmentLogFileRepo = new GenericRepository<S2ERMAssessmentLogsFile_TB>(unitOfWork.Transaction);
                                    var RMAssessmentLogFileByID = await RMAssessmentLogFileRepo.GetAsync(old.FILEREFID);
                                    await RMAssessmentLogFileRepo.DeleteAsync(RMAssessmentLogFileByID);
                                }
                                else if (ApproveGroupID == 3)
                                {
                                    var LabTestLogFileRepo = new GenericRepository<S2ELABTestLogsFile_TB>(unitOfWork.Transaction);
                                    var LabTestLogFileByID = await LabTestLogFileRepo.GetAsync(old.FILEREFID);
                                    await LabTestLogFileRepo.DeleteAsync(LabTestLogFileByID);
                                }
                                else if (ApproveGroupID == 4)
                                {
                                    var PCSampleLogFileRepo = new GenericRepository<S2EPurchaseSampleLogsFile_TB>(unitOfWork.Transaction);
                                    var PCSampleLogFileByID = await PCSampleLogFileRepo.GetAsync(old.FILEREFID);
                                    await PCSampleLogFileRepo.DeleteAsync(PCSampleLogFileByID);
                                }
                                else if (ApproveGroupID == 5)
                                {
                                    var AddRMLogFileRepo = new GenericRepository<S2EAddRawMaterialLogsFile_TB>(unitOfWork.Transaction);
                                    var AddRMLogFileByID = await AddRMLogFileRepo.GetAsync(old.FILEREFID);
                                    await AddRMLogFileRepo.DeleteAsync(AddRMLogFileByID);
                                }
                                else if (ApproveGroupID == 8)
                                {
                                    var TrialLogFileRepo = new GenericRepository<S2ETrialTestLogsFile_TB>(unitOfWork.Transaction);
                                    var TrialLogFileByID = await TrialLogFileRepo.GetAsync(old.FILEREFID);
                                    await TrialLogFileRepo.DeleteAsync(TrialLogFileByID);
                                }
                                else if (ApproveGroupID == 9)
                                {
                                    var AddRMSampleLogFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                                    var AddRMSampleLogFileByID = await AddRMSampleLogFileRepo.GetAsync(old.FILEREFID);
                                    await AddRMSampleLogFileRepo.DeleteAsync(AddRMSampleLogFileByID);
                                }
                                else
                                {
                                    AlertError = "มีข้อผิดพลาด ติดต่อ Admin 2";
                                    return Redirect($"/");
                                }

                                fileName = old.FILENAME;
                                System.IO.File.Delete($"{basePath}/{fileName}");
                            }


                            //newpath
                            var LogFileNew = LogFileLineALL.Where(x => x.LOGFILEHEADID == HeadID && x.ISCURRENTLOGS == 1 && x.ISDELETE == 0);
                            foreach (var newlog in LogFileNew)
                            {
                                fileName = newlog.FILENAME;;

                                if (ApproveGroupID == 1)
                                {
                                    var NewRequestLogFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                                    await NewRequestLogFileRepo.InsertAsync(new S2ENewRequestLogsFile_TB
                                    {
                                        REQUESTID = NewRequestID,
                                        FILENAME = newlog.FILENAME,
                                        CREATEBY = newlog.CREATEBY,
                                        CREATEDATE = newlog.CREATEDATE
                                    });

                                }
                                else if (ApproveGroupID == 2)
                                {
                                    var RMAssessmentLogFileRepo = new GenericRepository<S2ERMAssessmentLogsFile_TB>(unitOfWork.Transaction);
                                    
                                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                                    var RMAssessmentALL = await RMAssessmentRepo.GetAllAsync();
                                    var RMAssessmentID = RMAssessmentALL.Where(x=>x.REQUESTID == NewRequestID).Select(x=>x.ID).FirstOrDefault();

                                    await RMAssessmentLogFileRepo.InsertAsync(new S2ERMAssessmentLogsFile_TB
                                    {
                                        ASSESSMENTID = RMAssessmentID,
                                        FILENAME = newlog.FILENAME,
                                        CREATEBY = newlog.CREATEBY,
                                        CREATEDATE = newlog.CREATEDATE
                                    });
                                }
                                else if (ApproveGroupID == 3)
                                {
                                    var LabTestLogFileRepo = new GenericRepository<S2ELABTestLogsFile_TB>(unitOfWork.Transaction);
                                    await LabTestLogFileRepo.InsertAsync(new S2ELABTestLogsFile_TB
                                    {
                                        LABID = LABTestHeadId,
                                        LABLINEID = LABTestLineId,
                                        FILENAME = newlog.FILENAME,
                                        CREATEBY = newlog.CREATEBY,
                                        CREATEDATE = newlog.CREATEDATE
                                    });
                                }
                                else if (ApproveGroupID == 4)
                                {
                                    var PCSampleLogFileRepo = new GenericRepository<S2EPurchaseSampleLogsFile_TB>(unitOfWork.Transaction);
                                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                                    var PCSampleALL = await PCSampleRepo.GetAllAsync();
                                    var PCSampleID = PCSampleALL.Where(x => x.REQUESTID == NewRequestID).Select(x => x.ID).FirstOrDefault();

                                    await PCSampleLogFileRepo.InsertAsync(new S2EPurchaseSampleLogsFile_TB
                                    {
                                        PCSAMPLEID = PCSampleID,
                                        FILENAME = newlog.FILENAME,
                                        CREATEBY = newlog.CREATEBY,
                                        CREATEDATE = newlog.CREATEDATE
                                    });
                                }
                                else if (ApproveGroupID == 5)
                                {
                                    var AddRMLogFileRepo = new GenericRepository<S2EAddRawMaterialLogsFile_TB>(unitOfWork.Transaction);
                                    await AddRMLogFileRepo.InsertAsync(new S2EAddRawMaterialLogsFile_TB
                                    {
                                        ADDRMID = AddRMHeadId,
                                        ADDRMLINEID = AddRMLineId,
                                        FILENAME = newlog.FILENAME,
                                        CREATEBY = newlog.CREATEBY,
                                        CREATEDATE = newlog.CREATEDATE
                                    });
                                }
                                else if (ApproveGroupID == 8)
                                {
                                    var TrialLogFileRepo = new GenericRepository<S2ETrialTestLogsFile_TB>(unitOfWork.Transaction);
                                    await TrialLogFileRepo.InsertAsync(new S2ETrialTestLogsFile_TB
                                    {
                                        TRIALID = TRIALTestHeadId,
                                        TRIALLINEID = TRIALTestLineId,
                                        FILENAME = newlog.FILENAME,
                                        CREATEBY = newlog.CREATEBY,
                                        CREATEDATE = newlog.CREATEDATE
                                    });
                                }
                                else if (ApproveGroupID == 9)
                                {
                                    var AddRMSampleLogFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                                    await AddRMSampleLogFileRepo.InsertAsync(new S2EAddRawMaterialSampleLogsFile_TB
                                    {
                                        ADDRMSAMPLEID = AddRMSampleId,
                                        FILENAME = newlog.FILENAME,
                                        CREATEBY = UserALL.Email,
                                        CREATEDATE = newlog.CREATEDATE
                                    });
                                }
                                else
                                {
                                    AlertError = "มีข้อผิดพลาด ติดต่อ Admin 4";
                                    return Redirect($"/");
                                }

                                //var OldPath = $"{basePath}";
                               // var NewPath = $"{newPath}/{fileName}";
                                System.IO.File.Move($"{newPath}/{fileName}", $"{basePath}/{fileName}");


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

                            var BodyEmail = "";
                            BodyEmail = $@"
                                    <b> REQUEST NO : {NewRequestByID.REQUESTCODE}  </b><br/>
                                    <b> Process : {ApproveGroupByID.GROUPDESCRIPTION}  </b><br/>
                                    <b> สถานะ : </b><b style='color:green'> ดำเนินการแก้ไขเสร็จสิ้น </b><br/><br/>
                                ";

                            var sendEmail = _emailService.SendEmail(
                                $"{NewRequestByID.REQUESTCODE}/แจ้งแก้ไขไฟล์เอกสารแนบ",
                                BodyEmail,
                                EmailSuccess,
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
                    //isReject
                    else if ((ApproveResult == 2 && ApproveRemark != null))
                    {
                        //UPDATE PCREQUEST_TB (HEAD TABLE)
                        LogFileEditHeadByID.APPROVESTATUS = RequestStatusModel.Reject;

                        var LogFileEditLineRepo = new GenericRepository<S2ELogFileEditLine_TB>(unitOfWork.Transaction);
                        var LogFileEditLineALL = await LogFileEditLineRepo.GetAllAsync(); 

                        var LogFileEditLineByHeadID = LogFileEditLineALL.Where(x => x.LOGFILEHEADID == HeadID);
                        var CheckFile = LogFileEditLineByHeadID.Where(x => x.LOGFILEHEADID == HeadID).FirstOrDefault();

                        if (CheckFile != null)
                        {
                            string basePath = "";
                            var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                               NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);
                            var fileName = "";

                            if (ApproveGroupID == 1)
                            {
                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/NewRequest/tempEditFile";
                            }
                            else if (ApproveGroupID == 2)
                            {
                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/RMAssessment/tempEditFile";
                            }
                            else if (ApproveGroupID == 3)
                            {
                                var LABHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                                var LABHeadALL = await LABHeadRepo.GetAllAsync();
                                var LABHeadByRequestID = LABHeadALL.Where(x => x.REQUESTID == NewRequestID).Select(x => x.ID).FirstOrDefault();

                                var LABLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                                var LABLineALL = await LABLineRepo.GetAllAsync();

                                var LABLineID = LABLineALL.Where(x => x.LABID == LABHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/LABTest/{(int)LABLineID}/tempEditFile";
                            }
                            else if (ApproveGroupID == 4)
                            {
                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/PurchaseSample/tempEditFile";
                            }
                            else if (ApproveGroupID == 5)
                            {
                                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                                var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                                var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == NewRequestID).Select(x => x.ID).FirstOrDefault();

                                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                                var AddRMLineID = AddRMLineALL.Where(x => x.ADDRMID == AddRMHeadByRequestID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{(int)AddRMLineID}/tempEditFile";
                            }
                            else if (ApproveGroupID == 8)
                            {
                                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                                var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                                var AddRMHeadByRequestID = AddRMHeadALL.Where(x => x.REQUESTID == NewRequestID).Select(x => x.ID).FirstOrDefault();

                                var RMReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                                var RMReqHeadALL = await RMReqHeadRepo.GetAllAsync();
                                var RMReqHeadByAddRMID = RMReqHeadALL.Where(x => x.ADDRMID == AddRMHeadByRequestID).Select(x => x.ID).FirstOrDefault();

                                var TrialHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                                var TrialHeadALL = await TrialHeadRepo.GetAllAsync();
                                var TrialHeadByRMReqID = TrialHeadALL.Where(x => x.RMREQID == RMReqHeadByAddRMID).Select(x => x.ID).FirstOrDefault();

                                var TrialLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                                var TrialLineALL = await TrialLineRepo.GetAllAsync();
                                var TrialLineID = TrialLineALL.Where(x => x.TRIALID == TrialHeadByRMReqID && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/{(int)TrialLineID}/tempEditFile";
                            }
                            else if (ApproveGroupID == 9)
                            {
                                var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                                var AddRMSampleALL = await AddRMSampleRepo.GetAllAsync();
                                var AddRMSampleByRequestID = AddRMSampleALL.Where(x => x.REQUESTID == NewRequestID).Select(x => x.ID).FirstOrDefault();

                                basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{AddRMSampleByRequestID}/tempEditFile";
                            }
                            else
                            {
                                AlertError = "มีข้อผิดพลาด ติดต่อ Admin";
                                return Redirect($"/");
                            }

                            foreach (var item in LogFileEditLineByHeadID)
                            {
                                var LogFileEditLineByID = await LogFileEditLineRepo.GetAsync(item.ID);
                                LogFileEditLineByID.ISCURRENTLOGS = 0;
                                await LogFileEditLineRepo.UpdateAsync(LogFileEditLineByID);

                                fileName = item.FILENAME;

                                System.IO.File.Delete($"{basePath}/{fileName}");

                            }
                        }

                        //GET EMAIL REJECT
                        var EmailReject = new List<string>();
                        //CASE SET IN FLOW
                        var ApproveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                        var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();

                        EmailReject.Add(UserALL.Email);

                        //GET Reject BY
                        var approveFlowNameALL = ApproveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                        x.ApproveLevel == LogFileEditHeadByID.CURRENTAPPROVESTEP &&
                                                                        x.IsActive == 1);

                        var RejectByFirstName = approveFlowNameALL.Select(s => s.Name).FirstOrDefault();
                        var RejectByLastName = approveFlowNameALL.Select(s => s.LastName).FirstOrDefault();
                        var RejectBy = RejectByFirstName + " " + RejectByLastName;

                        var BodyEmail = "";
                        BodyEmail = $@"
                                    <b> REQUEST NO : {NewRequestByID.REQUESTCODE}  </b><br/>
                                    <b> Process : {ApproveGroupByID.GROUPDESCRIPTION}  </b><br/>
                                    <b> สถานะ : </b><b style='color:red'> ไม่อนุมัติ </b><br/><br/>
                                    <b style='color:black'> เหตุผล : </b> <b style='color:red'> {ApproveRemark} </b><br/>
                                    <b style='color:black'> Reject By : </b>{RejectBy}
                                ";

                        var sendEmail = _emailService.SendEmail(
                            $"{NewRequestByID.REQUESTCODE}/แจ้งแก้ไขไฟล์เอกสารแนบ",
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
                    //Next Level
                    else
                    {
                        //UPDATE PCREQUEST_TB (HEAD TABLE)
                        LogFileEditHeadByID.CURRENTAPPROVESTEP += 1;
                        LogFileEditHeadByID.APPROVESTATUS = RequestStatusModel.WaitingForApprove;

                        //GENERATE NONCE
                        var nonceKey = Guid.NewGuid().ToString();
                        await nonceRepo.InsertAsync(new S2ELogFileEditNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = DatetimeNow,
                            EXPIREDATE = DatetimeNow.AddDays(7),
                            ISUSED = 0
                        });

                        //NEXT APPROVE LEVEL
                        var nextALL = new GenericRepository<S2ELogFileEditApproveTrans_TB>(unitOfWork.Transaction);
                        var nextAllLevel = await nextALL.GetAllAsync();
                        var nextLevel = nextAllLevel.Where(x => x.LOGFILEHEADID == HeadID &&
                                                            x.APPROVELEVEL == LogFileEditHeadByID.CURRENTAPPROVESTEP &&
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
                                var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == NewRequestID).FirstOrDefault();

                                BodyEmail = $@"
                                    <b> REQUEST NO :</b> {NewRequestByID.REQUESTCODE} <br/><br/>
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
                                    </table>
                                    <br/>
                                    <br/>
                                    <b>กด Link </b> <a href='{_configuration["Config:BaseUrl"]}/S2E/AllTransaction/EditFileTodolist?Email={next.EMAIL}'> เพื่อดำเนินการ </a>
                                ";
                            }
                            else
                            {
                                BodyEmail = $@"
                                    <b> REQUEST NO :</b> {NewRequestByID.REQUESTCODE} <br/><br/>
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
                                    </table>
                                    <br/>
                                    <br/>
                                    <b>กด Link </b> <a href='{_configuration["Config:BaseUrl"]}/S2E/AllTransaction/EditFileTodolist?Email={next.EMAIL}'> เพื่อดำเนินการ </a>
                                ";
                            }

                            var sendEmail = _emailService.SendEmail(
                               $"{NewRequestByID.REQUESTCODE}/แจ้งแก้ไขไฟล์เอกสารแนบ",
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

                            var approveTrans_next = await LogFileEditTransRepo.GetAsync(next.ID);
                            approveTrans_next.SENDEMAILDATE = DatetimeNow;
                            await LogFileEditTransRepo.UpdateAsync(approveTrans_next);

                        }
                    }

                    await LogFileEditHeadRepo.UpdateAsync(LogFileEditHeadByID);
                    await nonceRepo.UpdateAsync(_nonce);

                    unitOfWork.Complete();
                    AlertSuccess = "Approve Success.";
                    return Redirect($"/S2E/AllTransaction/EditFileTodolist?Email={email}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
