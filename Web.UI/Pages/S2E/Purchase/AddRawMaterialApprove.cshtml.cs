using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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
    public class AddRawMaterialApproveModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int PurchaseSampleID { get; set; }
        [BindProperty]
        public decimal Qty { get; set; }
        [BindProperty]
        public string PONo { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public string ProjectRefNo { get; set; }
        [BindProperty]
        public string VendorID { get; set; }
        [BindProperty]
        public string SupplierName { get; set; }
        [BindProperty]
        public string Dealer { get; set; }
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
        public int isStartTest { get; set; }
        [BindProperty]
        public string isStartTestRemark { get; set; }
        [BindProperty]
        public int isPurchaseSample { get; set; }
        [BindProperty]
        public int ADDRMID { get; set; }
        [BindProperty]
        public DateTime? RequestDate { get; set; }
        [BindProperty]
        public int ApproveResult { get; set; }
        [BindProperty]
        public string ApproveRemark { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public int isKeyin { get; set; }
        [BindProperty]
        public int ADDRMLineID { get; set; }
        [BindProperty]
        public decimal QtyPO { get; set; }
        [BindProperty]
        public string CurrencyCode { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public AddRawMaterialApproveModel(
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
        public async Task<IActionResult> OnGetAsync(int AddRMID, int TranID, string nonce, string email, int isKeyinWhenApprove)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    ADDRMID = AddRMID;
                    isKeyin = isKeyinWhenApprove;
                    await GetData(AddRMID);

                    return Page();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task GetData(int AddRMID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                var AddRMLine = AddRMLineALL.Where(x=>x.ADDRMID == AddRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();
                var AddRMLineByID = await AddRMLineRepo.GetAsync(AddRMLine.ID);

                ADDRMLineID = AddRMLine.ID;

                var RequestID = AddRMHeadByID.REQUESTID;
                var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                var LABID = AddRMHeadByID.LABID;
                var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                RequestDate = AddRMLineByID.REQUESTDATE;
                isPurchaseSample = AddRMLineByID.ISPURCHASESAMPLE;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;
                Dealer = NewRequestByID.DEALER;
                ProductionSite = NewRequestByID.PRODUCTIONSITE;
                DealerAddress = NewRequestByID.DEALERADDRESS;
               
                
                Unit = NewRequestByID.UNIT;

                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                isStartTest = RMAssessmentByID.ISSTARTTEST == 1 ? 1 : 2;
                isStartTestRemark = RMAssessmentByID.ISSTARTTESTREMARK;                

                var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                VendorID = AddRMHeadByID.VENDORID;
                SupplierName = AddRMHeadByID.SUPPLIERNAME;
                Plant = AddRMHeadByID.PLANT;
                CurrencyCode = AddRMHeadByID.CURRENCYCODE;

                ItemCode = PCSampleByID.ITEMCODE;
                ItemName = PCSampleByID.ITEMNAME;

                Price = AddRMLineByID.PRICE;
                QtyPO = AddRMLineByID.QTYPO;
                Qty = AddRMLineByID.QTY;
                PONo = AddRMLineByID.PONO;


                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostGridViewFileUploadAsync(int ADDRMID, int ADDRMLineID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EAddRawMaterialLogsFileGridViewModel>($@"
                         SELECT *
                        FROM
                        (
                              SELECT ID,
		                        ADDRMID,
                                ADDRMLINEID,
		                        FILENAME,
                                CREATEBY,
		                        CONVERT(NVARCHAR,CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,CREATEDATE,108) AS CREATEDATE,
		                        ISACTIVE
                            FROM TB_S2EAddRawMaterialLogsFile 
                            WHERE ADDRMID = {ADDRMID} AND ADDRMLINEID = {ADDRMLineID} AND ISACTIVE = 1
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
        public async Task<IActionResult> OnGetDownloadFileUploadAsync(int ADDRMID, int ADDRMLineID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMLogFileRepo = new GenericRepository<S2EAddRawMaterialLogsFile_TB>(unitOfWork.Transaction);
                    var AddRMLogFileByFileID = await AddRMLogFileRepo.GetAsync(Fileid);

                    var AddRMRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMByID = await AddRMRepo.GetAsync(ADDRMID);

                    var RequestID = AddRMByID.REQUESTID;
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + RequestCode.Substring(4, 3) + "_" +
                        RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2) + "/AddRawMaterial/"+ADDRMLineID;

                    var fileName = AddRMLogFileByFileID.FILENAME;

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
        public async Task<IActionResult> OnPostGridViewApproveAsync(int ADDRMID, int ADDRMLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<GetApproveLogsGridViewModel>($@"
                         SELECT EMAIL,
                        CONVERT(NVARCHAR,APPROVEDATE,103) + ' ' + CONVERT(NVARCHAR,APPROVEDATE,108) AS APPROVEDATE
                        ,REMARK
                    FROM TB_S2EAddRawMaterialApproveTrans
                    WHERE ADDRMID = {ADDRMID} 
                    AND ADDRMLINEID = {ADDRMLINEID} 
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
        public async Task<IActionResult> OnPostAsync(int AddRMID, int TranID, string nonce, string email, int isKeyinWhenApprove)
        {
            try
            {
                if (ApproveResult == 0)
                {
                    AlertError = "กรุณาเลือกว่าจะอนุมัติ หรือ ไม่อนุมัติ !!";
                    return Redirect($"/S2E/Purchase/AddRawMaterialApprove?AddRMID={AddRMID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");
                }
                if (ApproveResult == 2 && ApproveRemark == null)
                {
                    AlertError = "กรุณาใส่เหตุผลที่ต้องการขอข้อมูลเพิ่มเติม !!";
                    return Redirect($"/S2E/Purchase/AddRawMaterialApprove?AddRMID={AddRMID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");

                }
                if (ApproveResult == 1 && Plant == "")
                {
                    AlertError = "กรุณาเลือก Plant  !!";
                    return Redirect($"/S2E/Purchase/AddRawMaterialApprove?AddRMID={AddRMID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");

                }

                if (!ModelState.IsValid)
                {
                    ADDRMID = AddRMID;
                    isKeyin = isKeyinWhenApprove;

                    await GetData(AddRMID);

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                    var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                    var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                    var AddRMLine = AddRMLineALL.Where(x => x.ADDRMID == AddRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();
                    var AddRMLineByID = await AddRMLineRepo.GetAsync(AddRMLine.ID);

                    var RequestID = AddRMHeadByID.REQUESTID;
                    var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                    var LABID = AddRMHeadByID.LABID;
                    var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                    var LABTestLineByID = LABTestLineALL.Where(x=>x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);


                    var DatetimeNow = DateTime.Now;
                    int approvemasterid = AddRMLineByID.APPROVEMASTERID;

                    //UPDATE OLD DATA
                    var nonceRepo = new GenericRepository<S2EAddRawMaterialNonce_TB>(unitOfWork.Transaction);
                    var _nonce = await unitOfWork.S2EControl.GetNonceAddRawMaterialByKey(nonce);
                    if (_nonce.ISUSED == 1)
                    {
                        throw new Exception("Link Is Used.");
                    }

                    _nonce.ISUSED = 1;

                    //UPDATE Approve Trans
                    var AddRMTransRepo = new GenericRepository<S2EAddRawMaterialApproveTrans_TB>(unitOfWork.Transaction);
                    var AddRMTransByID = await AddRMTransRepo.GetAsync(TranID);

                    var ApproveLevel = AddRMTransByID.APPROVELEVEL;
                    var ApproveGroupID = AddRMTransByID.APPROVEGROUPID;

                    var AddRMApproveTransRepo = new GenericRepository<S2EAddRawMaterialApproveTrans_TB>(unitOfWork.Transaction);
                    var AddRMApproveTransaLL = await AddRMApproveTransRepo.GetAllAsync();
                    var AddRMApproveTranLevel = AddRMApproveTransaLL.Where(x => x.ADDRMID == AddRMID &&
                                                                    x.ADDRMLINEID == AddRMLineByID.ID &&
                                                                    x.APPROVEMASTERID == approvemasterid &&
                                                                    x.APPROVELEVEL == ApproveLevel &&
                                                                    x.ISCURRENTAPPROVE == 1 &&
                                                                    x.APPROVEGROUPID == ApproveGroupID);

                    foreach (var UpdateApproveTrans in AddRMApproveTranLevel)
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
                        await AddRMApproveTransRepo.UpdateAsync(UpdateApproveTrans);
                    }

                    //GET REQUEST BY FULL NAME
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(AddRMLineByID.CREATEBY);

                    //GET APPROVE TRANS ALL LEVEL
                    var ApproveTransAll = await unitOfWork.S2EControl.GetApproveTransByAddRMIDAllLevel(AddRMID, approvemasterid, ApproveGroupID,AddRMLine.ID);
                    var AllLevel = ApproveTransAll.ToList().Count;

                    // CHECK IS FINAL APPROVE?
                    var ischeck1 = "";
                    var ischeck2 = "";
                    if (RMAssessmentByID.ISSTARTTEST == 1)
                    {
                        ischeck1 = "checked";
                        ischeck2 = "";
                    }
                    else
                    {
                        ischeck1 = "";
                        ischeck2 = "checked";
                    }

                    var ispcsample1 = "";
                    var ispcsample2 = "";
                    if (LABTestLineByID.ISPURCHASESAMPLE == 1)
                    {
                        ispcsample1 = "checked";
                        ispcsample2 = "";
                    }
                    else
                    {
                        ispcsample1 = "";
                        ispcsample2 = "checked";
                    }
                    //isFinal
                    if (AddRMLineByID.CURRENTAPPROVESTEP == AllLevel && ApproveResult == 1)
                    {
                        if (AddRMLineByID.COMPLETEDATE == null)
                        {
                            AddRMLineByID.APPROVESTATUS = RequestStatusModel.Complete;
                            AddRMLineByID.COMPLETEDATE = DatetimeNow;

                            var PlantMail = "";
                            //UPDATE (HEAD TABLE)
                            if (AddRMHeadByID.PLANT == null)
                            {
                                AddRMHeadByID.PLANT = Plant;
                            }

                            PlantMail = AddRMHeadByID.PLANT;

                            AddRMHeadByID.ISADDMORE = 0;

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
                            var TransALLApproveRepo = new GenericRepository<S2EAddRawMaterialApproveTrans_TB>(unitOfWork.Transaction);
                            var TransALLApproveALL = await TransALLApproveRepo.GetAllAsync();
                            var TransALLApprove = TransALLApproveALL.Where(x => x.ADDRMID == AddRMID &&
                                                                                x.ADDRMLINEID == AddRMLineByID.ID &&
                                                                                x.ISCURRENTAPPROVE == 1 &&
                                                                                x.APPROVEGROUPID == ApproveGroupID);
                            foreach (var i in TransALLApprove)
                            {
                                ALLApproveby += i.EMAIL + ",";
                            }

                            ALLApproveby = ALLApproveby.Substring(0, ALLApproveby.Length - 1);


                            var BodyEmail = "";
                            BodyEmail = $@"
                                <b>รายการ {NewRequestByID.REQUESTCODE} ได้ทำการจัดเก็บที่ Store {AddRMHeadByID.PLANT} เรียบร้อยแล้ว </b> <br/>
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
                                        <td>{AddRMHeadByID.VENDORID}</td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>ตัวแทนจำหน่าย: </td>
                                        <td colspan='3'>{AddRMHeadByID.SUPPLIERNAME}</td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>ที่อยู่ของตัวแทนจำหน่าย : </td>
                                        <td colspan='3'>{NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}</td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>Item Code : </td>
                                        <td>{PCSampleByID.ITEMCODE}</td>
                                        <td>PONo. : </td>
                                        <td>{AddRMLineByID.PONO}</td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>Item Name : </td>
                                        <td colspan='3'>{PCSampleByID.ITEMNAME}</td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>ราคา : </td>
                                        <td>{String.Format("{0:#,##0.#0}", AddRMLineByID.PRICE)}  {AddRMHeadByID.CURRENCYCODE}</td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>ซื้อวัตถุดิบเพิ่มหรือไม่? : </td>
                                        <td> 
                                            <label>
                                                <input type = 'radio' id = 'isPurchaseSampleYes' name = 'isPurchaseSample' {ispcsample1} disabled>
                                                Yes
                                            </label>
                                            <label>
                                                <input type = 'radio' id = 'isPurchaseSampleNo' name = 'isPurchaseSample' {ispcsample2} disabled>
                                                No
                                            </label>
                                        </td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>จำนวน : </td>
                                        <td>{String.Format("{0:#,##0.#0}", AddRMLineByID.QTY)}</td>
                                        <td style='text-align:right;'>หน่วย : </td>
                                        <td>{NewRequestByID.UNIT}</td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>เริ่มทดสอบ : </td>
                                        <td>
                                            <label>
                                                <input type = 'radio' id = 'isStartTestYes' name = 'isStartTest' {ischeck1} disabled>
                                                Yes
                                            </label>
                                            <label>
                                                <input type = 'radio' id = 'isStartTestNo' name = 'isStartTest' {ischeck2} disabled>
                                                No
                                            </label>

                                        </td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>สถานที่จัดเก็บ : </td>
                                        <td> {AddRMHeadByID.PLANT} </td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </table>
                                <br/>
                                <b>ชื่อผู้รับ : </b> {ALLApproveby}
                            ";

                            var sendEmail = _emailService.SendEmail(
                                  $"{NewRequestByID.REQUESTCODE} /เพื่อแจ้งให้หน่วยงาน store รับทราบ และจัดเก็บวัตถุดิบตัวอย่าง",
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
                        AddRMLineByID.APPROVESTATUS = RequestStatusModel.MoreDetail;
                       
                        /*if (AddRMHeadByID.ISADDMORE == 0)
                        {
                            AddRMHeadByID.PLANT = null;
                        }*/

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
                                                                        x.ApproveLevel == AddRMLineByID.CURRENTAPPROVESTEP &&
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
                            $"{NewRequestByID.REQUESTCODE} / More Detail / เพื่อแจ้งให้หน่วยงาน store รับทราบ และจัดเก็บวัตถุดิบตัวอย่าง ",
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
                        AddRMLineByID.CURRENTAPPROVESTEP += 1;
                        AddRMLineByID.APPROVESTATUS = RequestStatusModel.WaitingForApprove;

                        if (AddRMHeadByID.PLANT == null)
                        {
                            AddRMHeadByID.PLANT = Plant;
                        }

                        //GENERATE NONCE
                        var nonceKey = Guid.NewGuid().ToString();
                        await nonceRepo.InsertAsync(new S2EAddRawMaterialNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = DatetimeNow,
                            EXPIREDATE = DatetimeNow.AddDays(7),
                            ISUSED = 0
                        });

                        //NEXT APPROVE LEVEL
                        var nextALL = new GenericRepository<S2EAddRawMaterialApproveTrans_TB>(unitOfWork.Transaction);
                        var nextAllLevel = await nextALL.GetAllAsync();
                        var nextLevel = nextAllLevel.Where(x => x.ADDRMID == AddRMID &&
                                                            x.ADDRMLINEID == AddRMLine.ID &&
                                                            x.APPROVELEVEL == AddRMLineByID.CURRENTAPPROVESTEP &&
                                                            x.APPROVEMASTERID == approvemasterid &&
                                                            x.ISCURRENTAPPROVE == 1 &&
                                                            x.APPROVEGROUPID == ApproveGroupID);
                        foreach (var next in nextLevel)
                        {
                            var sendEmail = _emailService.SendEmail(
                                $"{NewRequestByID.REQUESTCODE} / เพื่อแจ้งให้หน่วยงาน Store รับทราบ และจัดเก็บวัตถุดิบตัวอย่าง",
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
                                            <td>{AddRMHeadByID.VENDORID}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ตัวแทนจำหน่าย : </td>
                                            <td colspan='3'>{AddRMHeadByID.SUPPLIERNAME}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ที่อยู่ของตัวแทนจำหน่าย : </td>
                                            <td colspan='3'>{NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>Item Code : </td>
                                            <td>{PCSampleByID.ITEMCODE}</td>
                                            <td>PONo. : </td>
                                            <td>{AddRMLineByID.PONO}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>Item Name : </td>
                                            <td colspan='3'>{PCSampleByID.ITEMNAME}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ราคา : </td>
                                            <td>{String.Format("{0:#,##0.#0}", AddRMLineByID.PRICE)}  {AddRMHeadByID.CURRENCYCODE}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ซื้อวัตถุดิบเพิ่มหรือไม่? : </td>
                                            <td> 
                                                <label>
                                                    <input type = 'radio' id = 'isPurchaseSampleYes' name = 'isPurchaseSample' {ispcsample1} disabled>
                                                    Yes
                                                </label>
                                                <label>
                                                    <input type = 'radio' id = 'isPurchaseSampleNo' name = 'isPurchaseSample' {ispcsample2} disabled>
                                                    No
                                                </label>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>จำนวน : </td>
                                            <td>{String.Format("{0:#,##0.#0}", AddRMLineByID.QTY)}</td>
                                            <td style='text-align:right;'>หน่วย : </td>
                                            <td>{NewRequestByID.UNIT}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>เริ่มทดสอบ : </td>
                                            <td>
                                                <label>
                                                    <input type = 'radio' id = 'isStartTestYes' name = 'isStartTest' {ischeck1} disabled>
                                                    Yes
                                                </label>
                                                <label>
                                                    <input type = 'radio' id = 'isStartTestNo' name = 'isStartTest' {ischeck2} disabled>
                                                    No
                                                </label>

                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>สถานที่จัดเก็บ : </td>
                                            <td> {AddRMHeadByID.PLANT} </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/AddRawMaterialTodolist?Email={next.EMAIL}'>  คลิกที่นี่ </a> <br/>
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

                            var approveTrans_next = await AddRMTransRepo.GetAsync(next.ID);
                            approveTrans_next.SENDEMAILDATE = DatetimeNow;
                            await AddRMTransRepo.UpdateAsync(approveTrans_next);

                        }

                    }

                    await AddRMHeadRepo.UpdateAsync(AddRMHeadByID);
                    await AddRMLineRepo.UpdateAsync(AddRMLineByID);
                    await nonceRepo.UpdateAsync(_nonce);

                    unitOfWork.Complete();
                    AlertSuccess = "Approve Success.";
                    return Redirect($"/S2E/Purchase/AddRawMaterialTodolist?Email={email}");
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
