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

namespace Web.UI.Pages.S2E.Purchase.AddRawMaterial
{
    public class ApproveTransModel : PageModel
    {
        public int AddRMID { get; set; }
        public int AddRMLINEID { get; set; }
        public int ApproveGroupID { get; set; }
        public int ApproveStatus { get; set; }
        public string PageBack { get; set; }
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }

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
        public async Task<IActionResult> OnGetAsync(int ADDRMID, int approvegroupid, int approvestatus,int ADDRMLINEID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_ADDRAWMATERIAL));
                AddRMID = ADDRMID;
                AddRMLINEID = ADDRMLINEID;
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
        public async Task<IActionResult> OnPostGridAsync(int ADDRMID, int approvegroupid, int approvestatus, int ADDRMLINEID)
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
                                FROM TB_S2EAddRawMaterialApproveTrans AT JOIN
                                TB_S2EAddRawMaterialLine AL ON AT.ADDRMLINEID = AL.ID JOIN
                                TB_S2EApproveGroup AG ON AL.APPROVEGROUPID = AG.ID 
                                WHERE AT.ADDRMID = {ADDRMID}
                                AND AT.ADDRMLINEID = {ADDRMLINEID}
                                AND AT.APPROVEGROUPID = {approvegroupid}
                            )T
                            GROUP BY T.ID,T.APPROVEGROUPID,T.APPROVEMASTERID,T.EMAIL,T.APPROVELEVEL,T.SENDEMAILDATE,
                                T.APPROVEDATE,T.REJECTDATE,T.ISDONE,T.REMARK,T.GROUPDESCRIPTION
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
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_ADDRAWMATERIAL));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMAppTransRepo = new GenericRepository<S2EAddRawMaterialApproveTrans_TB>(unitOfWork.Transaction);
                    var AddRMAppTransByID = await AddRMAppTransRepo.GetAsync(TranID);

                    var AddRMID = AddRMAppTransByID.ADDRMID;
                    var ApproveLevel = AddRMAppTransByID.APPROVELEVEL;

                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                    var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                    var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                    var AddRMLine = AddRMLineALL.Where(x=>x.ADDRMID == AddRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();
                    var AddRMLineByID = await AddRMLineRepo.GetAsync(AddRMLine.ID);

                    var RequestID = AddRMHeadByID.REQUESTID;
                    var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                    var LABID = AddRMHeadByID.LABID;
                    var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;
                    
                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                    var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                    var CreateBy = AddRMLineByID.CREATEBY;
                    var ApproveMasterid = AddRMLineByID.APPROVEMASTERID;

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

                    var sendEmail = _emailService.SendEmail(
                                  $"{RequestCode} / เพื่อแจ้งให้หน่วยงาน store รับทราบ และจัดเก็บวัตถุดิบตัวอย่าง",
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
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/AddRawMaterialTodolist?Email={AddRMAppTransByID.EMAIL}'>  คลิกที่นี่ </a> <br/>
                                ",
                                  new List<string> { AddRMAppTransByID.EMAIL },
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
                    return Redirect("/S2E/Purchase/AddRawMaterial/" + AddRMID + "/" + approvegroupid + "/" + approvestatus + "/" + AddRMLine.ID + "/ApproveTrans");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/AddRawMaterial/Main");
            }
        }
    }
}
