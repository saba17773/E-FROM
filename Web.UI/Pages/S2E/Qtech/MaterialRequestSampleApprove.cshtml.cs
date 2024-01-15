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
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Qtech
{
    public class MaterialRequestSampleApproveModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public string No { get; set; }
        [BindProperty]
        public string Department { get; set; }
        [BindProperty]
        public string SupGroup { get; set; }
        [BindProperty]
        public string ItemGroup { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public decimal Qty { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public int AddRMSampleId { get; set; }
        [BindProperty]
        public int RMReqSamId { get; set; }
        [BindProperty]
        public int RMReqSamLineId { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public decimal QtyTotal { get; set; }
        [BindProperty]
        public int RequestStatus { get; set; }
        [BindProperty]
        public string RequestDate { get; set; }
        [BindProperty]
        public int ApproveResult { get; set; }
        [BindProperty]
        public string ApproveRemark { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public MaterialRequestSampleApproveModel(
         IDatabaseContext databaseContext,
         IDatatableService datatablesService,
         IAuthService authService,
         IEmailService emailService,
         IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync(int RMReqSamID, int RMReqSamLineID, int TranID, string nonce, string email)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    RMReqSamId = RMReqSamID;
                    RMReqSamLineId = RMReqSamLineID;
                    await GetData(RMReqSamID, RMReqSamLineID);

                    return Page();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task GetData(int RMReqSamID, int RMReqSamLineID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var RMReqSampleLineRepo = new GenericRepository<S2EMaterialRequestSampleLine_TB>(unitOfWork.Transaction);
                var RMReqSampleLineByID = await RMReqSampleLineRepo.GetAsync(RMReqSamLineID);

                var RMReqSampleHeadRepo = new GenericRepository<S2EMaterialRequestSampleHead_TB>(unitOfWork.Transaction);
                var RMReqSampleHeadByID = await RMReqSampleHeadRepo.GetAsync(RMReqSamID);

                var AddRMSampleID = RMReqSampleHeadByID.ADDRMSAMPLEID;

                var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                var RequestID = AddRMSampleByID.REQUESTID;
                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                var RMReqSampleLineALL = await RMReqSampleLineRepo.GetAllAsync();
                var RMReqSampleLineByReqID = RMReqSampleLineALL.Where(x => x.RMREQSAMID == RMReqSamID &&
                                                                           x.ISACTIVE == 1 &&
                                                                           x.ID != RMReqSamLineID &&
                                                                           x.APPROVESTATUS != 2);
                decimal QtyUse = 0;
                if (RMReqSampleLineByReqID != null)
                {
                    foreach (var MaterialReqLineQTY in RMReqSampleLineByReqID)
                    {
                        QtyUse += MaterialReqLineQTY.QTY;
                    }
                }

                QtyTotal = NewRequestByID.QTY - QtyUse;

                RequestCode = NewRequestByID.REQUESTCODE;

                RequestStatus = RMReqSampleHeadByID.REQUESTSTATUS;
                Plant = AddRMSampleByID.PLANT;
                ItemCode = NewRequestByID.ITEMCODE;
                ItemName = NewRequestByID.ITEMNAME;
                Unit = NewRequestByID.UNIT;

                No = RMReqSampleLineByID.NO;
                Department = RMReqSampleLineByID.DEPARTMENT;
                SupGroup = RMReqSampleLineByID.SUPGROUP;
                Qty = RMReqSampleLineByID.QTY;
                RequestDate = Convert.ToDateTime(RMReqSampleLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss");

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostGridViewApproveAsync(int RMReqSamID, int RMReqSamLineID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<GetApproveLogsGridViewModel>($@"
                    SELECT EMAIL,
					       CONVERT(NVARCHAR,APPROVEDATE,103) + ' ' + CONVERT(NVARCHAR,APPROVEDATE,108) AS APPROVEDATE,
                           REMARK 
                    FROM TB_S2EMaterialRequestSampleApproveTrans
                    WHERE RMREQSAMID = {RMReqSamID} AND RMREQSAMLINEID = {RMReqSamLineID}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEDATE IS NOT NULL
                    ", null, unitOfWork.Transaction);
                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnPostAsync(int RMReqSamID, int RMReqSamLineID, int TranID, string nonce, string email)
        {
            try
            {
                if (ApproveResult == 0)
                {
                    AlertError = "กรุณาเลือกว่าจะอนุมัติ หรือ ไม่อนุมัติ !!";
                    return Redirect($"/S2E/Qtech/MaterialRequestSampleApprove?RMReqSamID={RMReqSamID}&RMReqSamLineID={RMReqSamLineID}&TranID={TranID}&nonce={nonce}&email={email}");
                }
                if (ApproveResult == 2 && ApproveRemark == null)
                {
                    AlertError = "กรุณาใส่เหตุผลที่ต้องการ Reject !!";
                    return Redirect($"/S2E/Qtech/MaterialRequestSampleApprove?RMReqSamID={RMReqSamID}&RMReqSamLineID={RMReqSamLineID}&TranID={TranID}&nonce={nonce}&email={email}");
                }

                if (!ModelState.IsValid)
                {
                    RMReqSamId = RMReqSamID;
                    RMReqSamLineId = RMReqSamLineID;
                    await GetData(RMReqSamID, RMReqSamLineID);

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RMReqSampleHeadRepo = new GenericRepository<S2EMaterialRequestSampleHead_TB>(unitOfWork.Transaction);
                    var RMReqSampleHeadByID = await RMReqSampleHeadRepo.GetAsync(RMReqSamID);

                    var RMReqSampleLineRepo = new GenericRepository<S2EMaterialRequestSampleLine_TB>(unitOfWork.Transaction);
                    var RMReqSampleLineByID = await RMReqSampleLineRepo.GetAsync(RMReqSamLineID);

                    var AddRMSampleID = RMReqSampleHeadByID.ADDRMSAMPLEID;

                    var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                    var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                    var RequestID = AddRMSampleByID.REQUESTID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var DatetimeNow = DateTime.Now;
                    int approvemasterid = RMReqSampleLineByID.APPROVEMASTERID;

                    //UPDATE OLD DATA
                    var nonceRepo = new GenericRepository<S2EMaterialRequestSampleNonce_TB>(unitOfWork.Transaction);
                    var _nonce = await unitOfWork.S2EControl.GetNonceMaterialRequestSampleByKey(nonce);
                    if (_nonce.ISUSED == 1)
                    {
                        throw new Exception("Link Is Used.");
                    }
                    _nonce.ISUSED = 1;

                    //UPDATE Approve Trans
                    var RMReqSampleTransRepo = new GenericRepository<S2EMaterialRequestSampleApproveTrans_TB>(unitOfWork.Transaction);
                    var RMReqSampleTransByID = await RMReqSampleTransRepo.GetAsync(TranID);

                    var ApproveLevel = RMReqSampleTransByID.APPROVELEVEL;
                    var ApproveGroupID = RMReqSampleTransByID.APPROVEGROUPID;

                    var RMReqSampleApproveTransRepo = new GenericRepository<S2EMaterialRequestSampleApproveTrans_TB>(unitOfWork.Transaction);
                    var RMReqSampleApproveTransALL = await RMReqSampleApproveTransRepo.GetAllAsync();
                    var RMReqSampleTransLevel = RMReqSampleApproveTransALL.Where(x => x.RMREQSAMID == RMReqSamID &&
                                                                    x.RMREQSAMLINEID == RMReqSamLineID &&
                                                                    x.APPROVEMASTERID == approvemasterid &&
                                                                    x.APPROVELEVEL == ApproveLevel &&
                                                                    x.ISCURRENTAPPROVE == 1 &&
                                                                    x.APPROVEGROUPID == ApproveGroupID);

                    foreach (var UpdateApproveTrans in RMReqSampleTransLevel)
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
                        await RMReqSampleApproveTransRepo.UpdateAsync(UpdateApproveTrans);
                    }

                    //GET REQUEST BY FULL NAME
                    var CreateBy = RMReqSampleLineByID.CREATEBY;
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(CreateBy);

                    //GET APPROVE TRANS ALL LEVEL
                    var ApproveTransAll = await unitOfWork.S2EControl.GetApproveTransByRMReqSampleIDAllLevel(RMReqSamID, RMReqSamLineID, approvemasterid, ApproveGroupID);
                    var AllLevel = ApproveTransAll.ToList().Count;

                    var PlantBodyEmail = "";
                    if (AddRMSampleByID.PLANT == "DSL")
                    {
                        PlantBodyEmail = "บริษัท ดีสโตน จำกัด";
                    }
                    else if (AddRMSampleByID.PLANT == "DRB")
                    {
                        PlantBodyEmail = "บริษัท ดีรับเบอร์ จำกัด";
                    }
                    else if (AddRMSampleByID.PLANT == "DSI")
                    {
                        PlantBodyEmail = "บริษัท ดีสโตน อินเตอร์เนชั่นแนล จำกัด";
                    }
                    else if (AddRMSampleByID.PLANT == "DSR")
                    {
                        PlantBodyEmail = "บริษัท สวิซซ์-วัน คอร์ปอเรชั่น จำกัด";
                    }
                    else
                    {
                        PlantBodyEmail = "บริษัท สยามทรัค เรเดียล จำกัด";
                    }

                    var BodyEmail = "";

                    // CHECK IS FINAL APPROVE?
                    //isFinal
                    if (RMReqSampleLineByID.CURRENTAPPROVESTEP == AllLevel && ApproveResult == 1)
                    {
                        if (RMReqSampleLineByID.COMPLETEDATE == null)
                        {

                            RMReqSampleLineByID.APPROVESTATUS = RequestStatusModel.Successfully;
                            RMReqSampleLineByID.COMPLETEDATE = DatetimeNow;

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

                            BodyEmail = $@"
                                <b> {PlantBodyEmail} </b> <br/>
                                <b> ใบเบิกวัตถุดิบ (LAB Sample) </b> <br/>
                                <table>
                                    <tr>
                                        <td style='text-align:right;'>
                                             Request Code : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {NewRequestByID.REQUESTCODE}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             เลขที่ : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {RMReqSampleLineByID.NO}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             วันที่ : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {Convert.ToDateTime(RMReqSampleLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             แผนก : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {RMReqSampleLineByID.DEPARTMENT}
                                        </td>
                                    </tr>
                                    <tr>
                                        <tr>
                                        <td style='text-align:right;'>
                                             ฝ่าย : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {RMReqSampleLineByID.SUPGROUP}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            Item Code :
                                        </td>
                                        <td style='text-align:left;'>
                                            {NewRequestByID.ITEMCODE}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            Item Name :
                                        </td>
                                        <td style='text-align:left;'>
                                            {NewRequestByID.ITEMNAME}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            จำนวน :
                                        </td>
                                        <td style='text-align:left;'>
                                            {String.Format("{0:#,##0.#0}", RMReqSampleLineByID.QTY)}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            หน่วย :
                                        </td>
                                        <td style='text-align:left;'>
                                            {NewRequestByID.UNIT}
                                        </td>
                                    </tr>
                                </table>
                                <br/><br/>
                                <b>ผลการอนุมัติ : </b> <b style='color:green'>ผ่านการอนุมัติ</b>
                            ";

                            var sendEmail = _emailService.SendEmail(
                                  $"{NewRequestByID.REQUESTCODE} / แจ้งผลการอนุมัติการเบิกวัตถุดิบ (LAB Sample) /  No.: {RMReqSampleLineByID.NO} / ผ่านการอนุมัติ",
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
                    ////isReject or More Detail
                    else if ((ApproveResult == 2 && ApproveRemark != null))
                    {
                        //UPDATE PCREQUEST_TB (HEAD TABLE)
                        RMReqSampleLineByID.APPROVESTATUS = RequestStatusModel.Reject;

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
                                                                        x.ApproveLevel == RMReqSampleLineByID.CURRENTAPPROVESTEP &&
                                                                        x.IsActive == 1);

                        var RejectByFirstName = approveFlowNameALL.Select(s => s.Name).FirstOrDefault();
                        var RejectByLastName = approveFlowNameALL.Select(s => s.LastName).FirstOrDefault();
                        var RejectBy = RejectByFirstName + " " + RejectByLastName;

                        BodyEmail = $@"
                            <b> {PlantBodyEmail} </b> <br/>
                            <b> ใบเบิกวัตถุดิบ (LAB Sample) </b> <br/>
                            <table>
                                <tr>
                                    <td style='text-align:right;'>
                                         Request Code : 
                                    </td>
                                    <td style='text-align:left;'>
                                        {NewRequestByID.REQUESTCODE}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                         เลขที่ : 
                                    </td>
                                    <td style='text-align:left;'>
                                        {RMReqSampleLineByID.NO}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                         วันที่ : 
                                    </td>
                                    <td style='text-align:left;'>
                                        {Convert.ToDateTime(RMReqSampleLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                         แผนก : 
                                    </td>
                                    <td style='text-align:left;'>
                                        {RMReqSampleLineByID.DEPARTMENT}
                                    </td>
                                </tr>
                                <tr>
                                    <tr>
                                    <td style='text-align:right;'>
                                         ฝ่าย : 
                                    </td>
                                    <td style='text-align:left;'>
                                        {RMReqSampleLineByID.SUPGROUP}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                        Item Code :
                                    </td>
                                    <td style='text-align:left;'>
                                        {NewRequestByID.ITEMCODE}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                        Item Name :
                                    </td>
                                    <td style='text-align:left;'>
                                        {NewRequestByID.ITEMNAME}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                        จำนวน :
                                    </td>
                                    <td style='text-align:left;'>
                                        {String.Format("{0:#,##0.#0}", RMReqSampleLineByID.QTY)}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                        หน่วย :
                                    </td>
                                    <td style='text-align:left;'>
                                        {NewRequestByID.UNIT}
                                    </td>
                                </tr>
                            </table>
                            <br/><br/>
                            <b>ผลการอนุมัติ : </b> <b style='color:red'>ไม่ผ่านการอนุมัติ</b>
                        ";

                        var sendEmail = _emailService.SendEmail(
                            $"{NewRequestByID.REQUESTCODE} / Reject ใบเบิกวัตถุดิบ (LAB Sample) /  No.: {RMReqSampleLineByID.NO}",
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
                        RMReqSampleLineByID.CURRENTAPPROVESTEP += 1;
                        RMReqSampleLineByID.APPROVESTATUS = RequestStatusModel.WaitingForApprove;

                        //GENERATE NONCE
                        var nonceKey = Guid.NewGuid().ToString();
                        await nonceRepo.InsertAsync(new S2EMaterialRequestSampleNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = DatetimeNow,
                            EXPIREDATE = DatetimeNow.AddDays(7),
                            ISUSED = 0
                        });

                        //NEXT APPROVE LEVEL
                        var nextALL = new GenericRepository<S2EMaterialRequestSampleApproveTrans_TB>(unitOfWork.Transaction);
                        var nextAllLevel = await nextALL.GetAllAsync();
                        var nextLevel = nextAllLevel.Where(x => x.RMREQSAMID == RMReqSamID &&
                                                            x.RMREQSAMLINEID == RMReqSamLineID &&
                                                            x.APPROVELEVEL == RMReqSampleLineByID.CURRENTAPPROVESTEP &&
                                                            x.APPROVEMASTERID == approvemasterid &&
                                                            x.ISCURRENTAPPROVE == 1 &&
                                                            x.APPROVEGROUPID == ApproveGroupID);
                        foreach (var next in nextLevel)
                        {
                            BodyEmail = $@"
                                <b> {PlantBodyEmail} </b> <br/>
                                <b> ใบเบิกวัตถุดิบ (LAB Sample) </b> <br/>
                                <table>
                                    <tr>
                                        <td style='text-align:right;'>
                                             Request Code : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {NewRequestByID.REQUESTCODE}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             เลขที่ : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {RMReqSampleLineByID.NO}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             วันที่ : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {Convert.ToDateTime(RMReqSampleLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             แผนก : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {RMReqSampleLineByID.DEPARTMENT}
                                        </td>
                                    </tr>
                                    <tr>
                                        <tr>
                                        <td style='text-align:right;'>
                                             ฝ่าย : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {RMReqSampleLineByID.SUPGROUP}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            Item Code :
                                        </td>
                                        <td style='text-align:left;'>
                                            {NewRequestByID.ITEMCODE}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            Item Name :
                                        </td>
                                        <td style='text-align:left;'>
                                            {NewRequestByID.ITEMNAME}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            จำนวน :
                                        </td>
                                        <td style='text-align:left;'>
                                            {String.Format("{0:#,##0.#0}", RMReqSampleLineByID.QTY)}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            หน่วย :
                                        </td>
                                        <td style='text-align:left;'>
                                            {NewRequestByID.UNIT}
                                        </td>
                                    </tr>
                                </table>
                                <br/>
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/MaterialRequestSampleTodolist?Email={next.EMAIL}'> คลิกที่นี่ </a>
                            ";

                            var sendEmail = _emailService.SendEmail(
                                  $"{NewRequestByID.REQUESTCODE} / เบิกวัตถุดิบ (LAB Sample) / No.: {RMReqSampleLineByID.NO} ",
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

                            var approveTrans_next = await RMReqSampleTransRepo.GetAsync(next.ID);
                            approveTrans_next.SENDEMAILDATE = DatetimeNow;
                            await RMReqSampleTransRepo.UpdateAsync(approveTrans_next);

                        }

                    }

                    await RMReqSampleLineRepo.UpdateAsync(RMReqSampleLineByID);
                    await nonceRepo.UpdateAsync(_nonce);

                    unitOfWork.Complete();
                    AlertSuccess = "Approve Success.";
                    return Redirect($"/S2E/Qtech/MaterialRequestSampleTodolist?Email={email}");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Qtech/MaterialRequestSampleTodolist?Email={email}");
            }
        }
    }
}
