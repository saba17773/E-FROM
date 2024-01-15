using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.AllTransaction
{
    public class EditFileModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int GroupID { get; set; }
        [BindProperty]
        public int RequestID { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public int ID { get; set; }
        [BindProperty]
        public string GroupDescription { get; set; }
        [BindProperty]
        public List<string> txtIsDelete { get; set; }
        [BindProperty]
        public List<int> txtFileID { get; set; }
        [BindProperty]
        public List<string> txtFileName { get; set; }
        [BindProperty]
        public string Remark { get; set; }
        [BindProperty]
        public List<IFormFile> FileUpload { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public EditFileModel(
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
        public async Task<IActionResult> OnGetAsync(int groupid,int id,int requestid)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_ALLTRANSACTION));

                GroupID = groupid;
                ID = id;
                RequestID = requestid;

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(requestid);

                    var ApproveGroupRepo = new GenericRepository<S2EApproveGroup_TB>(unitOfWork.Transaction);
                    var ApproveGroupByID = await ApproveGroupRepo.GetAsync(groupid);

                    RequestCode = NewRequestByID.REQUESTCODE;
                    GroupDescription = ApproveGroupByID.GROUPDESCRIPTION;
                    unitOfWork.Complete();
                }

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/AllTransaction/"+requestid+"/ViewList");
            }
        }
        public async Task<IActionResult> OnPostGridViewFileUploadAsync(int groupid, int id, int requestid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ELogFileEditGridViewModel>(@"
                            EXEC S2EAllTransactionEditFile @GroupID,@ID,@RequestID
                        ",
                        new
                        {
                            @GroupID = groupid,
                            @ID = id,
                            @RequestID = requestid
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
        public async Task<IActionResult> OnPostAsync(int groupid, int id, int requestid)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var CreateDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;
                    //Log File Edit Head
                    var LogFileEditHeadRepo = new GenericRepository<S2ELogFileEditHead_TB>(unitOfWork.Transaction);
                    var LogFileEditHeadALL = await LogFileEditHeadRepo.GetAllAsync();

                    //Log File Edit Line
                    var LogFileEditLineRepo = new GenericRepository<S2ELogFileEditLine_TB>(unitOfWork.Transaction);
                    var LogFileEditLineALL = await LogFileEditLineRepo.GetAllAsync();

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByCreateBy = approveMapALL.Where(x => x.CreateBy == CreateBy &&
                                                                   x.STEP == 1 &&
                                                                   x.APPROVEGROUPID == groupid
                                                              ).FirstOrDefault();

                    var approvemasterid = approveMapByCreateBy.APPROVEMASTERID;
                    var ApproveGroupID = approveMapByCreateBy.APPROVEGROUPID;
                    var ApproveStatus = RequestStatusModel.WaitingForApprove;

                    var checkHead = LogFileEditHeadALL.Where(x => x.REQUESTID == requestid && x.APPROVEGROUPID == groupid).ToList();

                    int LogFileHeadID;

                    if (checkHead.Count() == 0)
                    {
                        //INSERT Log File Edit Head
                        var LogFileHeadInsert = new S2ELogFileEditHead_TB
                        {
                            REQUESTID = requestid,
                            APPROVEMASTERID = approvemasterid,
                            APPROVEGROUPID = groupid,
                            CURRENTAPPROVESTEP = 1,
                            APPROVESTATUS = ApproveStatus,
                            REMARK = Remark,
                            CREATEBY = CreateBy,
                            CREATEDATE = CreateDate
                        };
                        var HeadID = await LogFileEditHeadRepo.InsertAsync(LogFileHeadInsert);
                        LogFileHeadID = (int)HeadID;
                    }
                    else
                    {
                        LogFileHeadID = checkHead.Select(x=>x.ID).FirstOrDefault();
                        var LogEditHeadByID = await LogFileEditHeadRepo.GetAsync(LogFileHeadID);
                        LogEditHeadByID.APPROVEMASTERID = approvemasterid;
                        LogEditHeadByID.CURRENTAPPROVESTEP = 1;
                        LogEditHeadByID.APPROVESTATUS = ApproveStatus;
                        LogEditHeadByID.REMARK = Remark;
                        LogEditHeadByID.UPDATEBY = CreateBy;
                        LogEditHeadByID.UPDATEDATE = CreateDate;
                        await LogFileEditHeadRepo.UpdateAsync(LogEditHeadByID);
                    }
                    

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(requestid);


                    //check old file
                    var LogFileOldRepo = new GenericRepository<S2ELogFileEditLine_TB>(unitOfWork.Transaction);
                    var LogFileOldALL = LogFileOldRepo.GetAll();
                    var LogFileOld = LogFileOldALL.Where(x => x.LOGFILEHEADID == LogFileHeadID && x.ISCURRENTLOGS == 1);
                    if (LogFileOld.ToList().Count != 0)
                    {
                        foreach (var oldLog in LogFileOld)
                        {
                            var LogFileOldUpdate = await LogFileOldRepo.GetAsync(oldLog.ID);
                            LogFileOldUpdate.ISCURRENTLOGS = 0;
                            await LogFileOldRepo.UpdateAsync(LogFileOldUpdate);
                        }
                    }

                    //INSERT Log File Edit Line
                    //File Delete
                    int row = txtIsDelete.Count();
                    for (int i = 0; i < row; i++)
                    {
                        if (txtIsDelete[i] == "1")
                        {
                            await LogFileEditLineRepo.InsertAsync(new S2ELogFileEditLine_TB
                            {
                                LOGFILEHEADID = (int)LogFileHeadID,
                                FILEREFID = (int)txtFileID[i],
                                FILENAME = txtFileName[i],
                                ISDELETE = 1,
                                ISCURRENTLOGS = 1,
                                CREATEBY = CreateBy,
                                CREATEDATE = CreateDate
                            });
                        }
                    }

                    //UPLOAD FILE 
                    var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                       NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);

                    string basePath = "";
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
                        var LABLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                        var LABLineALL = await LABLineRepo.GetAllAsync();
                        var LABLineID = LABLineALL.Where(x => x.LABID == id && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/LABTest/{(int)LABLineID}/tempEditFile";
                    }
                    else if (groupid == 4)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/PurchaseSample/tempEditFile";
                    }
                    else if (groupid == 5)
                    {
                        var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                        var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                        var AddRMLineID = AddRMLineALL.Where(x => x.ADDRMID == id && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{(int)AddRMLineID}/tempEditFile";
                    }
                    else if (groupid == 8)
                    {
                        var TrialLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                        var TrialLineALL = await TrialLineRepo.GetAllAsync();
                        var TrialLineID = TrialLineALL.Where(x => x.TRIALID == id && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/{(int)TrialLineID}/tempEditFile";
                    }
                    else if (groupid == 9)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{id}/tempEditFile";
                    }
                    else
                    {
                        AlertError = "มีข้อผิดพลาด ติดต่อ Admin";
                        return Redirect($"/S2E/AllTransaction/{groupid}/{id}/{requestid}/EditFile");
                    }

                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }

                    int rowfile = FileUpload.Count();
                    var filePath = Path.GetTempFileName();
                    string fileName = "";
                    for (int i = 0; i < rowfile; i++)
                    {
                        if (FileUpload[i] != null)
                        {
                            fileName = Path.GetFileName(FileUpload[i].FileName);
                            using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                            {
                                await FileUpload[i].CopyToAsync(stream);
                                await LogFileEditLineRepo.InsertAsync(new S2ELogFileEditLine_TB
                                {
                                    LOGFILEHEADID = (int)LogFileHeadID,
                                    FILENAME = fileName,
                                    ISDELETE = 0,
                                    ISCURRENTLOGS = 1,
                                    CREATEBY = CreateBy,
                                    CREATEDATE = CreateDate
                                });
                            }

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
                    //TB_S2ELogFileEditNonce
                    var nonceKey = Guid.NewGuid().ToString();
                    var nonceRepo = new GenericRepository<S2ELogFileEditNonce_TB>(unitOfWork.Transaction);
                    await nonceRepo.InsertAsync(new S2ELogFileEditNonce_TB
                    {
                        NONCEKEY = nonceKey,
                        CREATEDATE = CreateDate,
                        EXPIREDATE = CreateDate.AddDays(7),
                        ISUSED = 0
                    });


                    //UPDATE OLD APPROVE TRANS
                    var ApproveTransOldRepo = new GenericRepository<S2ELogFileEditApproveTrans_TB>(unitOfWork.Transaction);
                    var ApproveTransOldALL = ApproveTransOldRepo.GetAll();
                    var ApproveTransOld = ApproveTransOldALL.Where(x => x.LOGFILEHEADID == (int)LogFileHeadID &&
                                                                        x.APPROVEGROUPID == ApproveGroupID &&
                                                                        x.ISCURRENTAPPROVE == 1);
                    if (ApproveTransOld.ToList().Count != 0)
                    {
                        foreach (var App in ApproveTransOld)
                        {
                            var AppTransOldUpdate = await ApproveTransOldRepo.GetAsync(App.ID);
                            AppTransOldUpdate.ISCURRENTAPPROVE = 0;
                            await ApproveTransOldRepo.UpdateAsync(AppTransOldUpdate);
                        }
                    }

                    // INSERT APPROVE TRANSECTION
                    var LogFileEditAppTranRepo = new GenericRepository<S2ELogFileEditApproveTrans_TB>(unitOfWork.Transaction);
                    foreach (var AppFlow in approveFlow_data)
                    {
                        await LogFileEditAppTranRepo.InsertAsync(new S2ELogFileEditApproveTrans_TB
                        {
                            LOGFILEHEADID = (int)LogFileHeadID,
                            APPROVEMASTERID = AppFlow.ApproveMasterId,
                            APPROVEGROUPID = ApproveGroupID,
                            EMAIL = AppFlow.Email,
                            APPROVELEVEL = AppFlow.ApproveLevel,
                            ISCURRENTAPPROVE = 1
                        });
                    }

                    //GET APPROVE TRANS LEVEL 1
                    var AppTransByRequestID = await unitOfWork.S2EControl.GetApproveTransLogFileEdit(groupid, (int)LogFileHeadID, approvemasterid);
                    var AppTransLevel1 = AppTransByRequestID.Where(x => x.APPROVELEVEL == 1);

                    foreach (var AppTrans in AppTransLevel1)
                    {
                        var approveFlowApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                           x.ApproveLevel == 1 && 
                                                                           x.IsActive == 1 &&
                                                                           x.Email == AppTrans.EMAIL);

                        var AppTransByALL = await LogFileEditAppTranRepo.GetAllAsync();
                        var AppTransByID = AppTransByALL.Where(x => x.ID == AppTrans.ID).FirstOrDefault();

                        AppTransByID.SENDEMAILDATE = CreateDate;
                        await LogFileEditAppTranRepo.UpdateAsync(AppTransByID);

                        var BodyEmail = "";
                        if (NewRequestByID.ISCOMPAIRE == 1)
                        {
                            var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                            var NewRequestCompaireALL = await NewRequestCompaireRepo.GetAllAsync();
                            var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == RequestID).FirstOrDefault();

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
                                    <b>กด Link </b> <a href='{_configuration["Config:BaseUrl"]}/S2E/AllTransaction/EditFileTodolist?Email={AppTrans.EMAIL}'> เพื่อดำเนินการ </a>
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
                                    <b>กด Link </b> <a href='{_configuration["Config:BaseUrl"]}/S2E/AllTransaction/EditFileTodolist?Email={AppTrans.EMAIL}'> เพื่อดำเนินการ </a>
                                ";
                        }

                        var sendEmail = _emailService.SendEmail(
                               $"{NewRequestByID.REQUESTCODE}/แจ้งแก้ไขไฟล์เอกสารแนบ",
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

                    unitOfWork.Complete();
                    AlertSuccess = "Edit File Success";
                    return Redirect($"/S2E/AllTransaction/{requestid}/ViewList");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/AllTransaction/{groupid}/{id}/{requestid}/EditFile");
            }
        }
        public async Task<IActionResult> OnGetDownloadFileUploadAsync(int groupid, int id, int requestid, int fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
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
                        var LABLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                        var LABLineALL = await LABLineRepo.GetAllAsync();
                        var LABLineID = LABLineALL.Where(x => x.LABID == id && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
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
                        var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                        var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                        var AddRMLineID = AddRMLineALL.Where(x => x.ADDRMID == id && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{(int)AddRMLineID}";
                        var LogFileRepo = new GenericRepository<S2EAddRawMaterialLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 8)
                    {
                        var TrialLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                        var TrialLineALL = await TrialLineRepo.GetAllAsync();
                        var TrialLineID = TrialLineALL.Where(x => x.TRIALID == id && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/{(int)TrialLineID}";
                        var LogFileRepo = new GenericRepository<S2ETrialTestLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 9)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{id}";
                        var LogFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else
                    {
                        AlertError = "มีข้อผิดพลาด ติดต่อ Admin";
                        return Redirect($"/S2E/AllTransaction/{groupid}/{id}/{requestid}/EditFile");
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
        public async Task<IActionResult> OnGetViewFileUploadAsync(int groupid, int id, int requestid, int fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
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
                        var LABLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                        var LABLineALL = await LABLineRepo.GetAllAsync();
                        var LABLineID = LABLineALL.Where(x => x.LABID == id && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
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
                        var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                        var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                        var AddRMLineID = AddRMLineALL.Where(x => x.ADDRMID == id && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{(int)AddRMLineID}";
                        var LogFileRepo = new GenericRepository<S2EAddRawMaterialLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 8)
                    {
                        var TrialLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                        var TrialLineALL = await TrialLineRepo.GetAllAsync();
                        var TrialLineID = TrialLineALL.Where(x => x.TRIALID == id && x.ISCURRENTLOGS == 1).Select(x => x.ID).FirstOrDefault();
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/TrialTest/{(int)TrialLineID}";
                        var LogFileRepo = new GenericRepository<S2ETrialTestLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else if (groupid == 9)
                    {
                        basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{id}";
                        var LogFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                        var LogFileByID = await LogFileRepo.GetAsync(fileid);
                        fileName = LogFileByID.FILENAME;
                    }
                    else
                    {
                        AlertError = "มีข้อผิดพลาด ติดต่อ Admin";
                        return Redirect($"/S2E/AllTransaction/{groupid}/{id}/{requestid}/EditFile");
                    }

                    basePath = $"{basePath}/{fileName}";

                    if (!System.IO.File.Exists(basePath))
                    {
                        //throw new Exception("File not found.");
                        return NotFound();
                    }

                    FileStream stream = new FileStream(basePath, FileMode.Open);
                    return new FileStreamResult(stream, "application/pdf");


                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
