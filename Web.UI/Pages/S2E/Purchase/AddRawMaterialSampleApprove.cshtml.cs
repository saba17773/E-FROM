using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    public class AddRawMaterialSampleApproveModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int AddRMSampleId { get; set; }
        [BindProperty]
        public decimal Qty { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public string ProjectRefNo { get; set; }
        [BindProperty]
        public string VendorID { get; set; }
        [BindProperty]
        public string SupplierName { get; set; }
        [BindProperty]
        public string DealerAddress { get; set; }
        [BindProperty]
        public string ProductionSite { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public decimal Price { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public string CurrencyCode { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public List<IFormFile> FileUpload { get; set; }
        [BindProperty]
        public int isKeyin { get; set; }
        [BindProperty]
        public int ApproveResult { get; set; }
        [BindProperty]
        public string ApproveRemark { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public AddRawMaterialSampleApproveModel(
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
        public async Task<IActionResult> OnGetAsync(int AddRMSampleID, int TranID, string nonce, string email, int isKeyinWhenApprove)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    AddRMSampleId = AddRMSampleID;
                    isKeyin = isKeyinWhenApprove;
                    await GetData(AddRMSampleID);

                    return Page();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task GetData(int AddRMSampleID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AddRMSampleByID.ASSESSMENTID);

                var RequestID = RMAssessmentByID.REQUESTID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;
                VendorID = NewRequestByID.VENDORID;

                SupplierName = NewRequestByID.SUPPLIERNAME;
                ProductionSite = NewRequestByID.PRODUCTIONSITE;
                DealerAddress = NewRequestByID.DEALERADDRESS;
                ItemName = NewRequestByID.ITEMNAME;
                Price = NewRequestByID.PRICE;
                Unit = NewRequestByID.UNIT;
                Qty = NewRequestByID.QTY;
                CurrencyCode = NewRequestByID.CURRENCYCODE;
                Plant = NewRequestByID.PLANT;

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostGridViewFileUploadAsync(int AddRMSampleID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EAddRawMaterialSampleLogsFileGridViewModel>($@"
                        SELECT *
                        FROM
                        (
                              SELECT ID,
		                        ADDRMSAMPLEID,
		                        FILENAME,
                                CREATEBY,
		                        CONVERT(NVARCHAR,CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,CREATEDATE,108) AS CREATEDATE,
		                        ISACTIVE
                            FROM TB_S2EAddRawMaterialSampleLogsFile 
                            WHERE ADDRMSAMPLEID = {AddRMSampleID} AND ISACTIVE = 1
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
        public async Task<IActionResult> OnPostGridViewApproveAsync(int AddRMSampleID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<GetApproveLogsGridViewModel>($@"
                         SELECT EMAIL,
	                        CONVERT(NVARCHAR,APPROVEDATE,103) + ' ' + CONVERT(NVARCHAR,APPROVEDATE,108) AS APPROVEDATE,
	                        REMARK
                        FROM TB_S2EAddRawMaterialSampleApproveTrans
                        WHERE ADDRMSAMPLEID = {AddRMSampleID}
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
        public async Task<IActionResult> OnGetDownloadFileUploadAsync(int AddRMSampleID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMSampleLogFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                    var AddRMSampleLogFileByFileID = await AddRMSampleLogFileRepo.GetAsync(Fileid);

                    var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                    var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                    var RequestID = AddRMSampleByID.REQUESTID;
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + RequestCode.Substring(4, 3) + "_" +
                        RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2) + "/AddRawMaterialSample/" + AddRMSampleID;

                    var fileName = AddRMSampleLogFileByFileID.FILENAME;

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
        public async Task<IActionResult> OnPostAsync(int AddRMSampleID, int TranID, string nonce, string email, int isKeyinWhenApprove)
        {
            try
            {
                if (ApproveResult == 0)
                {
                    AlertError = "กรุณาเลือกว่าจะอนุมัติ หรือ ไม่อนุมัติ !!";
                    return Redirect($"/S2E/Purchase/AddRawMaterialSampleApprove?AddRMSampleID={AddRMSampleID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");
                }
                if (ApproveResult == 2 && ApproveRemark == null)
                {
                    AlertError = "กรุณาใส่เหตุผลที่ต้องการขอข้อมูลเพิ่มเติม !!";
                    return Redirect($"/S2E/Purchase/AddRawMaterialApprove?AddRMSampleID={AddRMSampleID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");

                }

                if (!ModelState.IsValid)
                {
                    AddRMSampleId = AddRMSampleID;
                    isKeyin = isKeyinWhenApprove;

                    await GetData(AddRMSampleID);

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                    var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                    var RequestID = AddRMSampleByID.REQUESTID;
                    var AssessmentID = AddRMSampleByID.ASSESSMENTID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                   
                    var DatetimeNow = DateTime.Now;
                    int approvemasterid = AddRMSampleByID.APPROVEMASTERID;

                    //UPDATE OLD DATA
                    var nonceRepo = new GenericRepository<S2EAddRawMaterialSampleNonce_TB>(unitOfWork.Transaction);
                    var _nonce = await unitOfWork.S2EControl.GetNonceAddRawMaterialSampleByKey(nonce);
                    if (_nonce.ISUSED == 1)
                    {
                        throw new Exception("Link Is Used.");
                    }

                    _nonce.ISUSED = 1;

                    //UPDATE Approve Trans
                    var AddRMSampleTransRepo = new GenericRepository<S2EAddRawMaterialSampleApproveTrans_TB>(unitOfWork.Transaction);
                    var AddRMSampleTransByID = await AddRMSampleTransRepo.GetAsync(TranID);

                    var ApproveLevel = AddRMSampleTransByID.APPROVELEVEL;
                    var ApproveGroupID = AddRMSampleTransByID.APPROVEGROUPID;

                    var AddRMSampleApproveTransRepo = new GenericRepository<S2EAddRawMaterialSampleApproveTrans_TB>(unitOfWork.Transaction);
                    var AddRMSampleApproveTransaLL = await AddRMSampleApproveTransRepo.GetAllAsync();
                    var AddRMSampleApproveTranLevel = AddRMSampleApproveTransaLL.Where(x => x.ADDRMSAMPLEID == AddRMSampleID &&
                                                                    x.APPROVEMASTERID == approvemasterid &&
                                                                    x.APPROVELEVEL == ApproveLevel &&
                                                                    x.ISCURRENTAPPROVE == 1 &&
                                                                    x.APPROVEGROUPID == ApproveGroupID);

                    foreach (var UpdateApproveTrans in AddRMSampleApproveTranLevel)
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
                        await AddRMSampleApproveTransRepo.UpdateAsync(UpdateApproveTrans);
                    }

                    //GET REQUEST BY FULL NAME
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(AddRMSampleByID.CREATEBY);

                    //GET APPROVE TRANS ALL LEVEL
                    var ApproveTransAll = await unitOfWork.S2EControl.GetApproveTransByAddRMSampleID(AddRMSampleID, approvemasterid, ApproveGroupID);
                    var AllLevel = ApproveTransAll.ToList().Count;

                    // CHECK IS FINAL APPROVE?
                    //isFinal
                    if (AddRMSampleByID.CURRENTAPPROVESTEP == AllLevel && ApproveResult == 1)
                    {
                        if (AddRMSampleByID.COMPLETEDATE == null)
                        {
                            AddRMSampleByID.APPROVESTATUS = RequestStatusModel.Complete;
                            AddRMSampleByID.COMPLETEDATE = DatetimeNow;

                            var AddRMSampleLogFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                            var AddRMSampleLogFileALL = await AddRMSampleLogFileRepo.GetAllAsync();

                            if (isKeyinWhenApprove == 1)
                            {
                                //UPLOAD FILE & INSERT ADD RAW MATERIAL SAMPLE LOG FILE
                                var RequestCodefilePath = "S2E_" + RequestCode.Substring(4, 3) + "_" +
                                   RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2);
                                int row = FileUpload.Count();
                                string basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{(int)AddRMSampleID}";
                                if (!System.IO.Directory.Exists(basePath))
                                {
                                    System.IO.Directory.CreateDirectory(basePath);
                                }
                                var filePath = Path.GetTempFileName();
                                string fileName = "";
                                var AddRMSampleLogsFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                                for (int i = 0; i < row; i++)
                                {
                                    if (FileUpload[i] != null)
                                    {
                                        fileName = Path.GetFileName(FileUpload[i].FileName);
                                        //fileok.Add($"{basePath}/{fileName}");
                                        using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                                        {
                                            await FileUpload[i].CopyToAsync(stream);
                                            await AddRMSampleLogsFileRepo.InsertAsync(new S2EAddRawMaterialSampleLogsFile_TB
                                            {
                                                ADDRMSAMPLEID = (int)AddRMSampleID,
                                                FILENAME = fileName,
                                                CREATEBY = email,
                                                CREATEDATE = DatetimeNow,
                                                ISACTIVE = 1
                                            });
                                        }

                                    }
                                }
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

                            //GET ALL Approve By
                            string ALLApproveby = "";
                            var TransALLApproveRepo = new GenericRepository<S2EAddRawMaterialSampleApproveTrans_TB>(unitOfWork.Transaction);
                            var TransALLApproveALL = await TransALLApproveRepo.GetAllAsync();
                            var TransALLApprove = TransALLApproveALL.Where(x => x.ADDRMSAMPLEID == AddRMSampleID &&
                                                                                x.ISCURRENTAPPROVE == 1 &&
                                                                                x.APPROVEGROUPID == ApproveGroupID);
                            foreach (var i in TransALLApprove)
                            {
                                ALLApproveby += i.EMAIL + ",";
                            }

                            ALLApproveby = ALLApproveby.Substring(0, ALLApproveby.Length - 1);


                            var BodyEmail = "";
                            BodyEmail = $@"
                                <b>รายการ {NewRequestByID.REQUESTCODE} ได้ทำการจัดเก็บที่ Store {AddRMSampleByID.PLANT} เรียบร้อยแล้ว </b> <br/>
                                <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                <b> รายละเอียดผู้ขาย </b><br/>
                                <table style='text-align:left;'>
                                    <tr>
                                        <td style='text-align:right;'>Request Code : </td>
                                        <td>{NewRequestByID.REQUESTCODE}</td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>รหัสผู้ขาย/ผู้ผลิต : </td>
                                        <td>{NewRequestByID.VENDORID}</td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>ตัวแทนจำหน่าย : </td>
                                        <td colspan='3'>{NewRequestByID.SUPPLIERNAME}</td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>ที่อยู่ของตัวแทนจำหน่าย : </td>
                                        <td colspan='3'>{NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}</td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>Item Name : </td>
                                        <td colspan='3'>{NewRequestByID.ITEMNAME}</td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>ราคา : </td>
                                        <td>{String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)}  {NewRequestByID.CURRENCYCODE}</td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>จำนวน : </td>
                                        <td>{String.Format("{0:#,##0.#0}", NewRequestByID.QTY)}</td>
                                        <td style='text-align:right;'>หน่วย : </td>
                                        <td>{NewRequestByID.UNIT}</td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>สถานที่จัดเก็บ : </td>
                                        <td> {NewRequestByID.PLANT} </td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </table>
                                <br/>
                                <b>ชื่อผู้รับ : </b> {ALLApproveby}
                            ";

                            var sendEmail = _emailService.SendEmail(
                                  $"{NewRequestByID.REQUESTCODE} /เพื่อแจ้งให้หน่วยงาน store รับทราบ และจัดเก็บวัตถุดิบตัวอย่าง (LAB Sample)",
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
                    //isReject or More Detail
                    else if ((ApproveResult == 2 && ApproveRemark != null))
                    {
                        //UPDATE PCREQUEST_TB (HEAD TABLE)
                        AddRMSampleByID.APPROVESTATUS = RequestStatusModel.MoreDetail;

                        var AddRMSampleLogFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                        var AddRMSampleLogFileALL = await AddRMSampleLogFileRepo.GetAllAsync();
                        var AddRMSampleByAddRMSamID = AddRMSampleLogFileALL.Where(x => x.ADDRMSAMPLEID == AddRMSampleID);
                        var CheckFile = AddRMSampleByAddRMSamID.Where(x => x.ADDRMSAMPLEID == AddRMSampleID).FirstOrDefault();
                        if (CheckFile != null)
                        {
                            foreach (var item in AddRMSampleByAddRMSamID)
                            {

                                var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                                    NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/AddRawMaterialSample/" + AddRMSampleID;

                                var fileName = item.FILENAME;

                                System.IO.File.Delete($"{filePath}/{fileName}");

                                await AddRMSampleLogFileRepo.DeleteAsync(item);
                              
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
                                                                        x.ApproveLevel == AddRMSampleByID.CURRENTAPPROVESTEP &&
                                                                        x.IsActive == 1);

                        var RejectByFirstName = approveFlowNameALL.Select(s => s.Name).FirstOrDefault();
                        var RejectByLastName = approveFlowNameALL.Select(s => s.LastName).FirstOrDefault();
                        var RejectBy = RejectByFirstName + " " + RejectByLastName;

                        var BodyEmail = "";
                        BodyEmail = $@"
                                    <b> REQUEST NO : {NewRequestByID.REQUESTCODE}  </b><br/>
                                    <b> สถานะ : </b><b style='color:red'> ขอข้อมูลเพิ่มเติม </b>    <br/>
                                    <b style='color:black'> สาเหตุที่ขอข้อมูลเพิ่มเติม : </b> {ApproveRemark} <br/>
                                    <b> Reject By : </b>{RejectBy}
                                ";

                        var sendEmail = _emailService.SendEmail(
                            $"{NewRequestByID.REQUESTCODE} / More Detail / เพื่อแจ้งให้หน่วยงาน store รับทราบ และจัดเก็บวัตถุดิบตัวอย่าง  (LAB Sample)",
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
                        AddRMSampleByID.CURRENTAPPROVESTEP += 1;
                        AddRMSampleByID.APPROVESTATUS = RequestStatusModel.WaitingForApprove;

                        var AddRMSampleLogFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                        var AddRMSampleLogFileALL = await AddRMSampleLogFileRepo.GetAllAsync();

                        if (isKeyinWhenApprove == 1)
                        {
                            //UPLOAD FILE & INSERT ADD RAW MATERIAL SAMPLE LOG FILE
                            var RequestCodefilePath = "S2E_" + RequestCode.Substring(4, 3) + "_" +
                               RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2);
                            int row = FileUpload.Count();
                            string basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterialSample/{(int)AddRMSampleID}";
                            if (!System.IO.Directory.Exists(basePath))
                            {
                                System.IO.Directory.CreateDirectory(basePath);
                            }
                            var filePath = Path.GetTempFileName();
                            string fileName = "";
                            var AddRMSampleLogsFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                            for (int i = 0; i < row; i++)
                            {
                                if (FileUpload[i] != null)
                                {
                                    fileName = Path.GetFileName(FileUpload[i].FileName);
                                    //fileok.Add($"{basePath}/{fileName}");
                                    using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                                    {
                                        await FileUpload[i].CopyToAsync(stream);
                                        await AddRMSampleLogsFileRepo.InsertAsync(new S2EAddRawMaterialSampleLogsFile_TB
                                        {
                                            ADDRMSAMPLEID = (int)AddRMSampleID,
                                            FILENAME = fileName,
                                            CREATEBY = email,
                                            CREATEDATE = DatetimeNow,
                                            ISACTIVE = 1
                                        });
                                    }

                                }
                            }
                        }

                        //GENERATE NONCE
                        var nonceKey = Guid.NewGuid().ToString();
                        await nonceRepo.InsertAsync(new S2EAddRawMaterialSampleNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = DatetimeNow,
                            EXPIREDATE = DatetimeNow.AddDays(7),
                            ISUSED = 0
                        });

                        //NEXT APPROVE LEVEL
                        var nextALL = new GenericRepository<S2EAddRawMaterialSampleApproveTrans_TB>(unitOfWork.Transaction);
                        var nextAllLevel = await nextALL.GetAllAsync();
                        var nextLevel = nextAllLevel.Where(x => x.ADDRMSAMPLEID == AddRMSampleID &&
                                                            x.APPROVELEVEL == AddRMSampleByID.CURRENTAPPROVESTEP &&
                                                            x.APPROVEMASTERID == approvemasterid &&
                                                            x.ISCURRENTAPPROVE == 1 &&
                                                            x.APPROVEGROUPID == ApproveGroupID);
                        foreach (var next in nextLevel)
                        {
                            var sendEmail = _emailService.SendEmail(
                                $"{NewRequestByID.REQUESTCODE} / เพื่อแจ้งให้หน่วยงาน Store รับทราบ และจัดเก็บวัตถุดิบตัวอย่าง (LAB Sample)",
                                   $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายละเอียดผู้ขาย </b><br/>
                                    <table style='text-align:left;'>
                                        <tr>
                                            <td style='text-align:right;'>Request Code : </td>
                                            <td>{NewRequestByID.REQUESTCODE}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>รหัสผู้ขาย/ผู้ผลิต : </td>
                                            <td>{NewRequestByID.VENDORID}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ตัวแทนจำหน่าย : </td>
                                            <td colspan='3'>{NewRequestByID.SUPPLIERNAME}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ที่อยู่ของตัวแทนจำหน่าย : </td>
                                            <td colspan='3'>{NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>Item Name : </td>
                                            <td colspan='3'>{NewRequestByID.ITEMNAME}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ราคา : </td>
                                            <td>{String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)}  {NewRequestByID.CURRENCYCODE}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>จำนวน : </td>
                                            <td>{String.Format("{0:#,##0.#0}", NewRequestByID.QTY)}</td>
                                            <td style='text-align:right;'>หน่วย : </td>
                                            <td>{NewRequestByID.UNIT}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>สถานที่จัดเก็บ : </td>
                                            <td> {NewRequestByID.PLANT} </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/AddRawMaterialSampleTodolist?Email={next.EMAIL}'>  คลิกที่นี่ </a> <br/>
                                ",
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

                            var approveTrans_next = await AddRMSampleTransRepo.GetAsync(next.ID);
                            approveTrans_next.SENDEMAILDATE = DatetimeNow;
                            await AddRMSampleTransRepo.UpdateAsync(approveTrans_next);

                        }

                    }

                    await AddRMSampleRepo.UpdateAsync(AddRMSampleByID);
                    await nonceRepo.UpdateAsync(_nonce);

                    unitOfWork.Complete();
                    AlertSuccess = "Approve Success.";
                    return Redirect($"/S2E/Purchase/AddRawMaterialSampleTodolist?Email={email}");
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
