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
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.AllTransaction
{
    public class ViewListModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int RequestID { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public ViewListModel(
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
        public async Task<IActionResult> OnGetAsync(int requestid)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_ALLTRANSACTION));

                RequestID = requestid;

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(requestid);

                    RequestCode = NewRequestByID.REQUESTCODE;

                    unitOfWork.Complete();
                }

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E");
            }
        }
        public async Task<IActionResult> OnPostGridAsync(int requestid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
					var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EAllTransactionViewListGridModel>(@"
                            EXEC S2EAllTransactionViewList @RequestID
                        ",
						new
						{
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

        public async Task<IActionResult> OnPostGridApproveTransAsync(int requestid, int GroupID, int Id)
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
								    ,AT.ISDONE,AT.REMARK,AT.LOGFILEHEADID,AG.GROUPDESCRIPTION
                                FROM TB_S2ELogFileEditHead H  JOIN
                                TB_S2ELogFileEditApproveTrans AT ON H.ID = AT.LOGFILEHEADID JOIN
						        TB_S2EApproveGroup AG ON AT.APPROVEGROUPID = AG.ID
                                WHERE H.REQUESTID = {requestid} AND H.APPROVEGROUPID = {GroupID}
                            )T
                            GROUP BY T.ID,APPROVEMASTERID,T.EMAIL,T.APPROVELEVEL,T.SENDEMAILDATE,
                                T.APPROVEDATE,T.REJECTDATE,T.ISDONE,T.REMARK,T.GROUPDESCRIPTION,
                                T.APPROVEGROUPID,T.LOGFILEHEADID
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

        public async Task<IActionResult> OnGetResendEmailAsync(int TranID)
        {
            try
            {
                //await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_RAWMATERIALREQUEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var ApproveTranRepo = new GenericRepository<S2ELogFileEditApproveTrans_TB>(unitOfWork.Transaction);
                    var ApproveTranByID = await ApproveTranRepo.GetAsync(TranID);

                    var LogHeadID = ApproveTranByID.LOGFILEHEADID;
                    var ApproveLevel = ApproveTranByID.APPROVELEVEL;

                    var LogFileHeadRepo = new GenericRepository<S2ELogFileEditHead_TB>(unitOfWork.Transaction);
                    var LogFileHeadByID = await LogFileHeadRepo.GetAsync(LogHeadID);

                    RequestID = LogFileHeadByID.REQUESTID;
                    var RequestId = LogFileHeadByID.REQUESTID;
                    var ApproveMasterid = LogFileHeadByID.APPROVEMASTERID;
                    var CreateBy = LogFileHeadByID.CREATEBY;
                    var RequestDate = LogFileHeadByID.CREATEDATE;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestId);

                    var RequestCode = NewRequestByID.REQUESTCODE;

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
                                    <b>กด Link </b> <a href='{_configuration["Config:BaseUrl"]}/S2E/AllTransaction/EditFileTodolist?Email={ApproveTranByID.EMAIL}'> เพื่อดำเนินการ </a>
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
                                    <b>กด Link </b> <a href='{_configuration["Config:BaseUrl"]}/S2E/AllTransaction/EditFileTodolist?Email={ApproveTranByID.EMAIL}'> เพื่อดำเนินการ </a>
                                ";
                    }

                    var sendEmail = _emailService.SendEmail(
                           $"{NewRequestByID.REQUESTCODE}/แจ้งแก้ไขไฟล์เอกสารแนบ",
                           BodyEmail,
                           new List<string> { ApproveTranByID.EMAIL },
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
                    return Redirect($"/S2E/AllTransaction/{RequestID}/ViewList");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/AllTransaction/{RequestID}/ViewList");
            }
        }

    }
}
