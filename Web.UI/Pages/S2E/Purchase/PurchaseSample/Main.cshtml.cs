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

namespace Web.UI.Pages.S2E.Purchase.PurchaseSample
{
    public class MainModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public MainModel(
         IDatabaseContext databaseContext,
         IDatatableService datatablesService,
         IEmailService emailService,
         IAuthService authService,
         IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _emailService = emailService;
            _authService = authService;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_PURCHASESAMPLE));

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase");
            }
        }
        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        REQUESTCODE = "REQUESTCODE",
                        REQUESTNO = "REQUESTNO",
                        SUPPLIERNAME = "SUPPLIERNAME",
                        VENDORID = "VENDORID",
                        REQUESTBY = "REQUESTBY"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EPurchaseSampleGridModel>(@"
                            SELECT T.* 
                            FROM (
                                SELECT LH.ID LABID
	                                ,LH.REQUESTID
	                                ,LH.ASSESSMENTID
	                                ,LL.PROJECTREFNO REQUESTNO
                                    ,LL.ID LABLINEID
	                                ,R.REQUESTCODE
	                                ,CASE WHEN PS.APPROVESTATUS IS NULL THEN R.VENDORID
	                                ELSE PS.VENDORID END AS VENDORID
	                                ,CASE WHEN PS.APPROVESTATUS IS NULL THEN LEFT(R.DEALER, 30) + '|' + LEFT(R.SUPPLIERNAME, 30)
	                                ELSE LEFT(PS.SUPPLIERNAME, 30)  + '<br/>' + LEFT(R.SUPPLIERNAME, 30)  END AS SUPPLIERNAME
	                                ,U.Email AS REQUESTBY
	                                ,LL.APPROVESTATUS LABSTATUS
	                                ,PS.ID PCSAMPLEID
	                                ,PS.APPROVESTATUS PCSAMPLESTATUS
                                    ,LL.ISPURCHASESAMPLE
	                                ,AH.ID ADDRMID
                                FROM TB_S2ELABTestHead LH JOIN
                                TB_S2ELABTestLine LL ON LH.ID = LL.LABID AND LL.ISCURRENTLOGS = 1 JOIN
                                TB_S2ENewRequest R ON LH.REQUESTID = R.ID JOIN
                                TB_User U ON LL.CREATEBY = U.Id LEFT JOIN
                                TB_S2EPurchaseSample PS ON LH.ID = PS.LABID LEFT JOIN
                                TB_S2EAddRawMaterialHead AH ON PS.ID = AH.PCSAMPLEID
                                WHERE LL.APPROVESTATUS = 5 OR 
                                (LL.APPROVESTATUS = 7 AND PS.APPROVESTATUS = 5) OR 
                                (LL.APPROVESTATUS = 7 AND PS.APPROVESTATUS = 7 )
                            )T
                        WHERE " + filter + @" ORDER BY T.LABSTATUS,T.PCSAMPLESTATUS
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnGetResendEmailAsync(int PCSamID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_PURCHASESAMPLE));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var CreateDate = DateTime.Now;

                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSamID);

                    var LABID = PCSampleByID.LABID;

                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                    var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                    if (LABTestLineByID.ISPURCHASESAMPLE == 0)
                    {
                        AlertError = "รายการนี้ไม่ได้สั่งซื้อเพิ่ม !!";
                        return Redirect("/S2E/Purchase/PurchaseSample/Main");
                    }

                    var RequestID = PCSampleByID.REQUESTID;
                    var approvemasterid = PCSampleByID.APPROVEMASTERID;
                    var CreateBy = PCSampleByID.CREATEBY;

                    var ApproveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var ApproveMapALL = await ApproveMapRepo.GetAllAsync();
                    var ApproveGroup = ApproveMapALL.Where(x => x.APPROVEMASTERID == approvemasterid).Select(s => s.APPROVEGROUPID).FirstOrDefault();

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    //UPDATE OLD APPROVE TRANS
                    var LogsSendEmailOLDRepo = new GenericRepository<S2EPurchaseSampleLogsSendEmail_TB>(unitOfWork.Transaction);
                    var LogsSendEmailOLDALL = LogsSendEmailOLDRepo.GetAll();
                    var LogsSendEmailOLD = LogsSendEmailOLDALL.Where(x => x.PCSAMPLEID == PCSamID && x.APPROVEGROUPID == ApproveGroup);
                    if (LogsSendEmailOLD.ToList().Count != 0)
                    {
                        foreach (var Logs in LogsSendEmailOLD)
                        {
                            var SendEmailOldUpdate = await LogsSendEmailOLDRepo.GetAsync(Logs.ID);
                            SendEmailOldUpdate.ISLASTSENDEMAIL = 0;
                            await LogsSendEmailOLDRepo.UpdateAsync(SendEmailOldUpdate);
                        }
                    }

                    //GET ApproveFlow
                    var AppFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                    var AppFlowALL = await AppFlowRepo.GetAllAsync();
                    var AppFlowByAppMasterID = AppFlowALL.Where(x => x.ApproveMasterId == approvemasterid & x.IsActive == 1);

                    // INSERT PC APPROVE TRANSECTION
                    var PCSampleLogsSendmailRepo = new GenericRepository<S2EPurchaseSampleLogsSendEmail_TB>(unitOfWork.Transaction);
                    foreach (var AppFlow in AppFlowByAppMasterID)
                    {
                        await PCSampleLogsSendmailRepo.InsertAsync(new S2EPurchaseSampleLogsSendEmail_TB
                        {
                            PCSAMPLEID = PCSamID,
                            EMAIL = AppFlow.Email,
                            SENDEMAILBY = CreateBy,
                            SENDEMAILDATE = CreateDate,
                            APPROVEGROUPID = ApproveGroup,
                            APPROVEMASTERID = approvemasterid,
                            ISLASTSENDEMAIL = 1
                        });
                    }

                    //GET Fullname CreateBy
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(CreateBy);
                    var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var EmpALL = await EmpRepo.GetAllAsync();
                    var EmpByEmpID = EmpALL.Where(x => x.EmployeeId == UserALL.EmployeeId).FirstOrDefault();
                    var FName = EmpByEmpID.Name;
                    var LName = EmpByEmpID.LastName;
                    var CreateByFullName = FName + " " + LName;

                    //GET Email in ApproveTrans
                    var AppTransByRequestID = await unitOfWork.S2EControl.GetLogsSendEmailByPCSampleID(PCSamID, approvemasterid, ApproveGroup);
                    var AppTransALL = AppTransByRequestID.Where(x => x.ISLASTSENDEMAIL == 1);
                    foreach (var AppTrans in AppTransALL)
                    {
                        var BodyEmail = "";
                        BodyEmail = $@"
                                <b> ข้อมูลที่ต้องการให้เปิด PR  </b><br/>
                                <table width='70%'>
                                    <tr style='vertical-align: top;'>
                                        <td colspan='2'>
                                            <b> รหัสผู้ขาย/ผู้ผลิต :</b> {PCSampleByID.VENDORID}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td colspan='2'>
                                            <b> ตัวแทนจำหน่าย :</b> {PCSampleByID.SUPPLIERNAME}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td colspan='2'>
                                            <b> Item Code :</b> {PCSampleByID.ITEMCODE}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td colspan='2'>
                                            <b> Item Name :</b> {PCSampleByID.ITEMNAME}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td colspan='2'>
                                            <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)}  {NewRequestByID.CURRENCYCODE}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td colspan='2'>
                                            <b> จำนวนที่ต้องการซื้อเพิ่ม :</b> {String.Format("{0:#,##0.#0}", LABTestLineByID.QTY)}  {NewRequestByID.UNIT}
                                        </td>
                                    </tr>
                                </table>
                                <br/><br/>
                                <a href='{_configuration["Config:BaseUrl"]}/S2E/PurchaseSampleViewInfo/?PCSampleID={PCSamID}&EMAILAPPROVE={AppTrans.EMAIL}'> คลิกที่นี่ เพื่อดูรายละเอียด </a>
                            ";

                        var sendEmail = _emailService.SendEmail(
                              $"{NewRequestByID.REQUESTCODE} / ทำการเปิด PR ได้",
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

                    AlertSuccess = "Resend Email successfully";
                    return Redirect("/S2E/Purchase/PurchaseSample/Main");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/PurchaseSample/Main");
            }
        }

    }
}
