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
    public class MaterialRequestApproveModel : PageModel
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
        public int AddRMID { get; set; }
        [BindProperty]
        public int RMReqID { get; set; }
        [BindProperty]
        public int RMReqLineID { get; set; }
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
        public MaterialRequestApproveModel(
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
        public async Task<IActionResult> OnGetAsync(int RMREQID, int RMREQLINEID, int TranID, string nonce, string email)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    RMReqLineID = RMREQLINEID;
                    await GetData(RMREQLINEID);

                    return Page();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task GetData(int RMREQLINEID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var MaterialReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                var MaterialReqLineByID = await MaterialReqLineRepo.GetAsync(RMREQLINEID);
                RMReqID = MaterialReqLineByID.RMREQID;
                var RMREQID = MaterialReqLineByID.RMREQID;

                var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMREQID);

                var ADDRMID = MaterialReqHeadByID.ADDRMID;

                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                var AddRMLineByID = AddRMLineALL.Where(x => x.ADDRMID == ADDRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                var MaterialReqLineALL = await MaterialReqLineRepo.GetAllAsync();
                var MaterialReqLineByReqID = MaterialReqLineALL.Where(x => x.RMREQID == RMREQID &&
                                                                           x.ADDRMLINEID == AddRMLineByID.ID &&
                                                                           x.ISACTIVE == 1 &&
                                                                           x.ID != RMREQLINEID &&
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

                var RequestID = AddRMHeadByID.REQUESTID;
                var LABID = AddRMHeadByID.LABID;
                var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;

                RequestStatus = MaterialReqHeadByID.REQUESTSTATUS;
                Plant = MaterialReqHeadByID.PLANT;
                ItemGroup = MaterialReqHeadByID.ITEMGROUP;
                ItemCode = MaterialReqHeadByID.ITEMCODE;
                ItemName = MaterialReqHeadByID.ITEMNAME;
                Unit = MaterialReqHeadByID.UNIT;

                No = MaterialReqLineByID.NO;
                Department = MaterialReqLineByID.DEPARTMENT;
                SupGroup = MaterialReqLineByID.SUPGROUP;
                Qty = MaterialReqLineByID.QTY;
                RequestDate = Convert.ToDateTime(MaterialReqLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss");

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostGridViewApproveAsync(int RMREQID, int RMREQLINEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<GetApproveLogsGridViewModel>($@"
                    SELECT EMAIL,
					       CONVERT(NVARCHAR,APPROVEDATE,103) + ' ' + CONVERT(NVARCHAR,APPROVEDATE,108) AS APPROVEDATE,
                           REMARK 
                    FROM TB_S2EMaterialRequestApproveTrans
                    WHERE RMREQID = {RMREQID} AND RMREQLINEID = {RMREQLINEID}
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
        public async Task<IActionResult> OnPostAsync(int RMREQID, int RMREQLINEID, int TranID, string nonce, string email)
        {
            try
            {
                if (ApproveResult == 0)
                {
                    AlertError = "กรุณาเลือกว่าจะอนุมัติ หรือ ไม่อนุมัติ !!";
                    return Redirect($"/S2E/Qtech/MaterialRequestApprove?RMREQID={RMREQID}&RMREQLINEID={RMREQLINEID}&TranID={TranID}&nonce={nonce}&email={email}");
                }
                if (ApproveResult == 2 && ApproveRemark == null)
                {
                    AlertError = "กรุณาใส่เหตุผลที่ต้องการ Reject !!";
                    return Redirect($"/S2E/Qtech/MaterialRequestApprove?RMREQID={RMREQID}&RMREQLINEID={RMREQLINEID}&TranID={TranID}&nonce={nonce}&email={email}");
                }

                if (!ModelState.IsValid)
                {
                    RMReqLineID = RMREQLINEID;
                    await GetData(RMREQLINEID);

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                    var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMREQID);

                    var MaterialReqLineRepo = new GenericRepository<S2EMaterialRequestLine_TB>(unitOfWork.Transaction);
                    var MaterialReqLineByID = await MaterialReqLineRepo.GetAsync(RMREQLINEID);

                    var ADDRMID = MaterialReqHeadByID.ADDRMID;

                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                    var RequestID = AddRMHeadByID.REQUESTID;
                    var LABID = AddRMHeadByID.LABID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                    var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                    var DatetimeNow = DateTime.Now;
                    int approvemasterid = MaterialReqLineByID.APPROVEMASTERID;

                    //UPDATE OLD DATA
                    var nonceRepo = new GenericRepository<S2EMaterialRequestNonce_TB>(unitOfWork.Transaction);
                    var _nonce = await unitOfWork.S2EControl.GetNonceMaterialRequestByKey(nonce);
                    if (_nonce.ISUSED == 1)
                    {
                        throw new Exception("Link Is Used.");
                    }
                    _nonce.ISUSED = 1;

                    //UPDATE Approve Trans
                    var MaterialReqTransRepo = new GenericRepository<S2EMaterialRequestApproveTrans_TB>(unitOfWork.Transaction);
                    var MaterialReqTransByID = await MaterialReqTransRepo.GetAsync(TranID);

                    var ApproveLevel = MaterialReqTransByID.APPROVELEVEL;
                    var ApproveGroupID = MaterialReqTransByID.APPROVEGROUPID;

                    var MaterialReqApproveTransRepo = new GenericRepository<S2EMaterialRequestApproveTrans_TB>(unitOfWork.Transaction);
                    var MaterialReqTransALL = await MaterialReqApproveTransRepo.GetAllAsync();
                    var MaterialReqTransLevel = MaterialReqTransALL.Where(x => x.RMREQID == RMREQID &&
                                                                    x.RMREQLINEID == RMREQLINEID &&
                                                                    x.APPROVEMASTERID == approvemasterid &&
                                                                    x.APPROVELEVEL == ApproveLevel &&
                                                                    x.ISCURRENTAPPROVE == 1 &&
                                                                    x.APPROVEGROUPID == ApproveGroupID);

                    foreach (var UpdateApproveTrans in MaterialReqTransLevel)
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
                        await MaterialReqApproveTransRepo.UpdateAsync(UpdateApproveTrans);
                    }

                    //GET REQUEST BY FULL NAME
                    var CreateBy = MaterialReqLineByID.CREATEBY;
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(CreateBy);

                    //GET APPROVE TRANS ALL LEVEL
                    var ApproveTransAll = await unitOfWork.S2EControl.GetApproveTransByMaterialReqIDAllLevel(RMREQID, RMREQLINEID, approvemasterid, ApproveGroupID);
                    var AllLevel = ApproveTransAll.ToList().Count;

                    var PlantBodyEmail = "";
                    if (AddRMHeadByID.PLANT == "DSL")
                    {
                        PlantBodyEmail = "บริษัท ดีสโตน จำกัด";
                    }
                    else if (AddRMHeadByID.PLANT == "DRB")
                    {
                        PlantBodyEmail = "บริษัท ดีรับเบอร์ จำกัด";
                    }
                    else if (AddRMHeadByID.PLANT == "DSI")
                    {
                        PlantBodyEmail = "บริษัท ดีสโตน อินเตอร์เนชั่นแนล จำกัด";
                    }
                    else if (AddRMHeadByID.PLANT == "DSR")
                    {
                        PlantBodyEmail = "บริษัท สวิซซ์-วัน คอร์ปอเรชั่น จำกัด";
                    }
                    else
                    {
                        PlantBodyEmail = "บริษัท สยามทรัค เรเดียล จำกัด";
                    }

                    var ischeck1 = "";
                    var ischeck2 = "";
                    if (MaterialReqHeadByID.ITEMGROUP == "RM")
                    {
                        ischeck1 = "checked";
                        ischeck2 = "";
                    }
                    else
                    {
                        ischeck1 = "";
                        ischeck2 = "checked";
                    }

                    var BodyEmail = "";

                    // CHECK IS FINAL APPROVE?
                    //isFinal
                    if (MaterialReqLineByID.CURRENTAPPROVESTEP == AllLevel && ApproveResult == 1)
                    {
                        if (MaterialReqLineByID.COMPLETEDATE == null)
                        {

                            MaterialReqLineByID.APPROVESTATUS = RequestStatusModel.Successfully;
                            MaterialReqLineByID.COMPLETEDATE = DatetimeNow;

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
                                <b> ใบเบิกวัตถุดิบ </b> <br/>
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
                                            {MaterialReqLineByID.NO}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             วันที่ : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {Convert.ToDateTime(MaterialReqLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             แผนก : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {MaterialReqLineByID.DEPARTMENT}
                                        </td>
                                    </tr>
                                    <tr>
                                        <tr>
                                        <td style='text-align:right;'>
                                             ฝ่าย : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {MaterialReqLineByID.SUPGROUP}
                                        </td>
                                    </tr>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             Item Group : 
                                        </td>
                                        <td style='text-align:left;'>
                                            <label>
                                                <input type = 'radio' id = 'RM' name = 'ItemGroup' {ischeck1} disabled>
                                                RM
                                            </label>
                                            <label>
                                                <input type = 'radio' id = 'FS' name = 'ItemGroup' {ischeck2} disabled>
                                                FS
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            Item Code :
                                        </td>
                                        <td style='text-align:left;'>
                                            {MaterialReqHeadByID.ITEMCODE}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            Item Name :
                                        </td>
                                        <td style='text-align:left;'>
                                            {MaterialReqHeadByID.ITEMNAME}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            จำนวน :
                                        </td>
                                        <td style='text-align:left;'>
                                            {String.Format("{0:#,##0.#0}", MaterialReqLineByID.QTY)}
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
                                  $"{NewRequestByID.REQUESTCODE} / แจ้งผลการอนุมัติการเบิกวัตถุดิบ /  No.: {MaterialReqLineByID.NO} / ผ่านการอนุมัติ",
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
                        MaterialReqLineByID.APPROVESTATUS = RequestStatusModel.Reject;

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
                                                                        x.ApproveLevel == MaterialReqLineByID.CURRENTAPPROVESTEP &&
                                                                        x.IsActive == 1);

                        var RejectByFirstName = approveFlowNameALL.Select(s => s.Name).FirstOrDefault();
                        var RejectByLastName = approveFlowNameALL.Select(s => s.LastName).FirstOrDefault();
                        var RejectBy = RejectByFirstName + " " + RejectByLastName;

                        BodyEmail = $@"
                            <b> {PlantBodyEmail} </b> <br/>
                            <b> ใบเบิกวัตถุดิบ </b> <br/>
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
                                        {MaterialReqLineByID.NO}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                         วันที่ : 
                                    </td>
                                    <td style='text-align:left;'>
                                        {Convert.ToDateTime(MaterialReqLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                         แผนก : 
                                    </td>
                                    <td style='text-align:left;'>
                                        {MaterialReqLineByID.DEPARTMENT}
                                    </td>
                                </tr>
                                <tr>
                                    <tr>
                                    <td style='text-align:right;'>
                                         ฝ่าย : 
                                    </td>
                                    <td style='text-align:left;'>
                                        {MaterialReqLineByID.SUPGROUP}
                                    </td>
                                </tr>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                         Item Group : 
                                    </td>
                                    <td style='text-align:left;'>
                                        <label>
                                            <input type = 'radio' id = 'RM' name = 'ItemGroup' {ischeck1} disabled>
                                            RM
                                        </label>
                                        <label>
                                            <input type = 'radio' id = 'FS' name = 'ItemGroup' {ischeck2} disabled>
                                            FS
                                        </label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                        Item Code :
                                    </td>
                                    <td style='text-align:left;'>
                                        {MaterialReqHeadByID.ITEMCODE}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                        Item Name :
                                    </td>
                                    <td style='text-align:left;'>
                                        {MaterialReqHeadByID.ITEMNAME}
                                    </td>
                                </tr>
                                <tr>
                                    <td style='text-align:right;'>
                                        จำนวน :
                                    </td>
                                    <td style='text-align:left;'>
                                        {String.Format("{0:#,##0.#0}", MaterialReqLineByID.QTY)}
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
                            $"{NewRequestByID.REQUESTCODE} / Reject ใบเบิกวัตถุดิบ /  No.: {MaterialReqLineByID.NO}",
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
                        MaterialReqLineByID.CURRENTAPPROVESTEP += 1;
                        MaterialReqLineByID.APPROVESTATUS = RequestStatusModel.WaitingForApprove;

                        //GENERATE NONCE
                        var nonceKey = Guid.NewGuid().ToString();
                        await nonceRepo.InsertAsync(new S2EMaterialRequestNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = DatetimeNow,
                            EXPIREDATE = DatetimeNow.AddDays(7),
                            ISUSED = 0
                        });

                        //NEXT APPROVE LEVEL
                        var nextALL = new GenericRepository<S2EMaterialRequestApproveTrans_TB>(unitOfWork.Transaction);
                        var nextAllLevel = await nextALL.GetAllAsync();
                        var nextLevel = nextAllLevel.Where(x => x.RMREQID == RMREQID &&
                                                            x.RMREQLINEID == RMREQLINEID &&
                                                            x.APPROVELEVEL == MaterialReqLineByID.CURRENTAPPROVESTEP &&
                                                            x.APPROVEMASTERID == approvemasterid &&
                                                            x.ISCURRENTAPPROVE == 1 &&
                                                            x.APPROVEGROUPID == ApproveGroupID);
                        foreach (var next in nextLevel)
                        {
                            BodyEmail = $@"
                                <b> {PlantBodyEmail} </b> <br/>
                                <b> ใบเบิกวัตถุดิบ </b> <br/>
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
                                            {MaterialReqLineByID.NO}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             วันที่ : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {Convert.ToDateTime(MaterialReqLineByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             แผนก : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {MaterialReqLineByID.DEPARTMENT}
                                        </td>
                                    </tr>
                                    <tr>
                                        <tr>
                                        <td style='text-align:right;'>
                                             ฝ่าย : 
                                        </td>
                                        <td style='text-align:left;'>
                                            {MaterialReqLineByID.SUPGROUP}
                                        </td>
                                    </tr>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                             Item Group : 
                                        </td>
                                        <td style='text-align:left;'>
                                            <label>
                                                <input type = 'radio' id = 'RM' name = 'ItemGroup' {ischeck1} disabled>
                                                RM
                                            </label>
                                            <label>
                                                <input type = 'radio' id = 'FS' name = 'ItemGroup' {ischeck2} disabled>
                                                FS
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            Item Code :
                                        </td>
                                        <td style='text-align:left;'>
                                            {MaterialReqHeadByID.ITEMCODE}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            Item Name :
                                        </td>
                                        <td style='text-align:left;'>
                                            {MaterialReqHeadByID.ITEMNAME}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='text-align:right;'>
                                            จำนวน :
                                        </td>
                                        <td style='text-align:left;'>
                                            {String.Format("{0:#,##0.#0}", MaterialReqLineByID.QTY)}
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
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/MaterialRequestTodolist?Email={next.EMAIL}'> คลิกที่นี่ </a>
                            ";

                            var sendEmail = _emailService.SendEmail(
                                  $"{NewRequestByID.REQUESTCODE} / เบิกวัตถุดิบ / No.: {MaterialReqLineByID.NO} ",
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

                            var approveTrans_next = await MaterialReqTransRepo.GetAsync(next.ID);
                            approveTrans_next.SENDEMAILDATE = DatetimeNow;
                            await MaterialReqTransRepo.UpdateAsync(approveTrans_next);

                        }

                    }

                    await MaterialReqLineRepo.UpdateAsync(MaterialReqLineByID);
                    await nonceRepo.UpdateAsync(_nonce);

                    unitOfWork.Complete();
                    AlertSuccess = "Approve Success.";
                    return Redirect($"/S2E/Qtech/MaterialRequestTodolist?Email={email}");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Qtech/MaterialRequestTodolist?Email={email}");
            }
        }
    }
}
