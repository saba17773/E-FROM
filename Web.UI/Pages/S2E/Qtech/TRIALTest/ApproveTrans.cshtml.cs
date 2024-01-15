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

namespace Web.UI.Pages.S2E.Qtech.TRIALTest
{
    public class ApproveTransModel : PageModel
    {
        public int TrialID { get; set; }
        public int TrialLineID { get; set; }
        public int ApproveGroupID { get; set; }
        public int ApproveStatus { get; set; }
        public string PageBack { get; set; }
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int isPurchase { get; set; }
        public string Pagetext { get; set; }
        public string PageMain { get; set; }

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
        public async Task<IActionResult> OnGetAsync(int trialid, int triallineid, int approvegroupid, int approvestatus)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_TRIALTEST));
                TrialID = trialid;
                TrialLineID = triallineid;
                ApproveGroupID = approvegroupid;
                ApproveStatus = approvestatus;

                if (await _authService.CanDisplay(nameof(S2EPermissionModel.VIEW_PURCHASE)))
                {
                    isPurchase = 1;
                }
                else
                {
                    isPurchase = 0;
                }

                if (isPurchase == 0)
                {
                    if (approvestatus == 2)
                    {
                        PageBack = "/S2E/Qtech/TRIALTest/MainCancel";
                    }
                    else
                    {
                        PageBack = "/S2E/Qtech/TRIALTest/Main";
                    }
                    PageMain = "/S2E/Qtech";
                    Pagetext = "Qtech";
                }
                else
                {
                    PageBack = "/S2E/Qtech/TRIALTest/MainSuccess";
                    PageMain = "/S2E/Purchase";
                    Pagetext = "Purchase";
                }
                
                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech");
            }
        }
        public async Task<IActionResult> OnPostGridAsync(int trialid,int triallineid, int approvegroupid)
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
	                            ,AT.ISDONE,AT.REMARK,AG.GROUPDESCRIPTION
	                            FROM TB_S2ETrialTestApproveTrans AT JOIN 
	                            TB_S2EApproveGroup AG ON AT.APPROVEGROUPID = AG.ID JOIN
	                            TB_S2ETrialTestHead TH ON AT.TRIALID = TH.ID JOIN
	                            TB_S2ETrialTestLine TL ON AT.TRIALLINEID = TL.ID
	                            WHERE AT.TRIALID = {trialid} AND AT.TRIALLINEID = {triallineid} AND AT.APPROVEGROUPID = {approvegroupid}
                            )T
                            GROUP BY T.ID,APPROVEMASTERID,T.EMAIL,T.APPROVELEVEL,T.SENDEMAILDATE,
                            T.APPROVEDATE,T.REJECTDATE,T.ISDONE,T.REMARK,T.GROUPDESCRIPTION,
                            T.APPROVEGROUPID
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
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_TRIALTEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var TrialTestAppTransRepo = new GenericRepository<S2ETrialTestApproveTrans_TB>(unitOfWork.Transaction);
                    var TrialTestAppTransByTransID = await TrialTestAppTransRepo.GetAsync(TranID);
                    var TRIALID = TrialTestAppTransByTransID.TRIALID;
                    var TRIALLINEID = TrialTestAppTransByTransID.TRIALLINEID;
                    var ApproveLevel = TrialTestAppTransByTransID.APPROVELEVEL;

                    var TrialTestHeadRepo = new GenericRepository<S2ETrialTestHead_TB>(unitOfWork.Transaction);
                    var TrialTestHeadByID = await TrialTestHeadRepo.GetAsync(TRIALID);

                    var TrialTestLineRepo = new GenericRepository<S2ETrialTestLine_TB>(unitOfWork.Transaction);
                    var TrialTestLineByID = await TrialTestLineRepo.GetAsync(TRIALLINEID);

                    var RMReqID = TrialTestHeadByID.RMREQID;
                    var RequestDate = TrialTestHeadByID.REQUESTDATE;
                    
                    var CreateBy = TrialTestLineByID.CREATEBY;
                    var ApproveMasterid = TrialTestLineByID.APPROVEMASTERID;
                    

                    var MaterialReqHeadRepo = new GenericRepository<S2EMaterialRequestHead_TB>(unitOfWork.Transaction);
                    var MaterialReqHeadByID = await MaterialReqHeadRepo.GetAsync(RMReqID);

                    var AddRMID = MaterialReqHeadByID.ADDRMID;

                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                    var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                    var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                    var AddRMLine = AddRMLineALL.Where(x => x.ADDRMID == AddRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();
                    var AddRMLineByID = await AddRMLineRepo.GetAsync(AddRMLine.ID);

                    var RequestID = AddRMHeadByID.REQUESTID;
                    var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(AddRMHeadByID.LABID);
                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                    var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == AddRMHeadByID.LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                    var RequestCode = NewRequestByID.REQUESTCODE;
                    var ProjectRefNo = LABTestLineByID.PROJECTREFNO;

                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

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
                                                              x.ApproveLevel == ApproveLevel &&
                                                              x.IsActive == 1).Select(s => s.Name).FirstOrDefault();

                    var Subject = "";
                    var Body = "";
                    if (TrialTestLineByID.TESTRESULT == 1)
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
                             <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/TRIALTestTodolist?Email={TrialTestAppTransByTransID.EMAIL}'> คลิกที่นี่ </a>
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
                            <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Qtech/TRIALTestTodolist?Email={TrialTestAppTransByTransID.EMAIL}'> คลิกที่นี่ </a>
                            <br/>
                        ";
                    }

                    var sendEmail = _emailService.SendEmail(
                        $"{NewRequestByID.REQUESTCODE} / แจ้งผลการทดสอบวัตถุดิบ Trial / {Subject}",
                        BodyEmail,
                        new List<string> { TrialTestAppTransByTransID.EMAIL },
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
                    return Redirect("/S2E/Qtech/TRIALTest/" + TRIALID + "/" + TRIALLINEID + "/" + approvegroupid + "/" + approvestatus + "/ApproveTrans");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech/TRIALTest/Main");
            }
        }
    }
}
