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

namespace Web.UI.Pages.S2E.Purchase.RMAssessment
{
    public class ApproveTransModel : PageModel
    {
        public int AssessmentId { get; set; }
        public int ApproveGroupID { get; set; }
        public int ApproveStatus { get; set; }
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        public string PageBack { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public ApproveTransModel(
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
        public async Task<IActionResult> OnGetAsync(int AssessmentID, int approvegroupid, int approvestatus)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_RMASSESSMENT));
                AssessmentId = AssessmentID;
                ApproveGroupID = approvegroupid;
                ApproveStatus = approvestatus;
                if (approvestatus == 2)
                {
                    PageBack = "MainCancel";
                }
                else
                {
                    PageBack = "Main";
                }

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase");
            }
        }
        public async Task<IActionResult> OnPostGridAsync(int AssessmentID, int approvegroupid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EApproveTransGridModel>($@"
                           SELECT *
                            FROM
                            (
                                SELECT AT.ID,AT.APPROVEGROUPID,AT.APPROVEMASTERID,AT.EMAIL,AT.APPROVELEVEL
                                ,CONVERT(VARCHAR,AT.SENDEMAILDATE,103)+' '+CONVERT(VARCHAR,AT.SENDEMAILDATE,108)SENDEMAILDATE
                                ,CONVERT(VARCHAR,AT.APPROVEDATE,103)+' '+CONVERT(VARCHAR,AT.APPROVEDATE,108)APPROVEDATE
                                ,CONVERT(VARCHAR,AT.REJECTDATE,103)+' '+CONVERT(VARCHAR,AT.REJECTDATE,108)REJECTDATE
                                ,AT.ISDONE,AT.REMARK
                                FROM TB_S2ERMAssessmentApproveTrans AT JOIN
                                TB_S2ERMAssessment R ON AT.ASSESSMENTID = R.ID
                                WHERE AT.ASSESSMENTID = {AssessmentID}
                                AND AT.APPROVEGROUPID = {approvegroupid}
                            )T
                            GROUP BY T.ID,T.APPROVEGROUPID,T.APPROVEMASTERID,T.EMAIL,T.APPROVELEVEL,T.SENDEMAILDATE,
                                T.APPROVEDATE,T.REJECTDATE,T.ISDONE,T.REMARK
                            ORDER BY T.ID ASC
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
        public async Task<IActionResult> OnGetResendEmailAsync(int TranID, int approvegroupid, int approvestatus)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_RMASSESSMENT));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var RMAssessmentAppTransRepo = new GenericRepository<S2ERMAssessmentApproveTrans_TB>(unitOfWork.Transaction);
                    var RMAssessmentAppTransByID = await RMAssessmentAppTransRepo.GetAsync(TranID);
                    var AssessmentID = RMAssessmentAppTransByID.ASSESSMENTID;
                    var ApproveLevel = RMAssessmentAppTransByID.APPROVELEVEL;

                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                    var CreateBy = RMAssessmentByID.CREATEBY;
                    var ApproveMasterid = RMAssessmentByID.APPROVEMASTERID;
                    var RequestID = RMAssessmentByID.REQUESTID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);
                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(CreateBy);
                    var EmployeeId = UserALL.EmployeeId;
                    var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var EmpALL = await EmpRepo.GetAllAsync();
                    var EmpFName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.Name).FirstOrDefault();
                    var EmpLName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.LastName).FirstOrDefault();
                    var EmpFullName = EmpFName + " " + EmpLName;

                    var appoveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                    var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                    var ApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == ApproveMasterid &&
                                                              x.ApproveLevel == RMAssessmentAppTransByID.APPROVELEVEL &&
                                                              x.IsActive == 1).Select(s => s.Name).FirstOrDefault();
                    //PREPARE FILE UPLOAD
                    //var fileok = new List<string>();
                    var RequestCodefilePath = "S2E_" + RequestCode.Substring(4, 3) + "_" +
                        RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2);

               
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
                            <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/RMAssessmentTodolist?Email={RMAssessmentAppTransByID.EMAIL}'> คลิกที่นี่ </a>
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
                            <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/RMAssessmentTodolist?Email={RMAssessmentAppTransByID.EMAIL}'> คลิกที่นี่ </a>
                            <br/>
                        ";
                    }

                    var sendEmail = _emailService.SendEmail(
                        $"{RequestCode} / Request / ร้องขอเพื่ออนุมัติเอกสารขอประเมินวัตถุดิบ",
                        BodyEmail,
                        new List<string> { RMAssessmentAppTransByID.EMAIL },
                        new List<string> { },
                        "",
                        "",
                        new List<string> { }
                    );

                    if (sendEmail.Result == false)
                    {
                        throw new Exception(sendEmail.Message);
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Resend Email Successfully";
                    return Redirect("/S2E/Purchase/RMAssessment/" + AssessmentID + "/" + approvegroupid + "/" + approvestatus + "/ApproveTrans");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/RMAssessment/Main");
            }
        }
    }
}
