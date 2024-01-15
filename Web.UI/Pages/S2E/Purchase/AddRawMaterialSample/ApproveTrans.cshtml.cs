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

namespace Web.UI.Pages.S2E.Purchase.AddRawMaterialSample
{
    public class ApproveTransModel : PageModel
    {
        public int AddRMSampleId { get; set; }
        public int AddRMLINEID { get; set; }
        public int ApproveGroupId { get; set; }
        public int Approvestatus { get; set; }
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
        public async Task<IActionResult> OnGetAsync(int AddRMSampleID, int ApproveGroupID, int ApproveStatus)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_ADDRAWMATERIALSAMPLE));
                AddRMSampleId = AddRMSampleID;
                ApproveGroupId = ApproveGroupID;
                Approvestatus = ApproveStatus;

                //if (Approvestatus == 2)
                //{
                //    PageBack = "MainCancel";
                //}
                //else
                //{
                //    PageBack = "Main";
                //}
                PageBack = "Main";
                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase");
            }
        }
        public async Task<IActionResult> OnPostGridAsync(int AddRMSampleID, int ApproveGroupID, int ApproveStatus)
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
                                FROM TB_S2EAddRawMaterialSampleApproveTrans AT JOIN
                                TB_S2EAddRawMaterialSample ARS ON AT.ADDRMSAMPLEID = ARS.ID JOIN
                                TB_S2EApproveGroup AG ON AT.APPROVEGROUPID = AG.ID 
                                WHERE AT.ADDRMSAMPLEID = {AddRMSampleID}
                                AND AT.APPROVEGROUPID = {ApproveGroupID}
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
        public async Task<IActionResult> OnGetResendEmailAsync(int AddRMSampleID, int ApproveGroupID, int ApproveStatus, int TranID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_ADDRAWMATERIALSAMPLE));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMSampleAppTransRepo = new GenericRepository<S2EAddRawMaterialSampleApproveTrans_TB>(unitOfWork.Transaction);
                    var AddRMSampleAppTransByID = await AddRMSampleAppTransRepo.GetAsync(TranID);

                    var ApproveLevel = AddRMSampleAppTransByID.APPROVELEVEL;

                    var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                    var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                    var RequestID = AddRMSampleByID.REQUESTID;
                    var AssessmentID = AddRMSampleByID.ASSESSMENTID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                    var CreateBy = AddRMSampleByID.CREATEBY;
                    var ApproveMasterid = AddRMSampleByID.APPROVEMASTERID;

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

                    var sendEmail = _emailService.SendEmail(
                                  $"{RequestCode} / เพื่อแจ้งให้หน่วยงาน Store รับทราบ และจัดเก็บวัตถุดิบตัวอย่าง (LAB Sample)",
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
                                            <td> {AddRMSampleByID.PLANT} </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/AddRawMaterialSampleTodolist?Email={AddRMSampleAppTransByID.EMAIL}'>  คลิกที่นี่ </a> <br/>
                                ",
                                  new List<string> { AddRMSampleAppTransByID.EMAIL },
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
                    return Redirect("/S2E/Purchase/AddRawMaterialSample/" + AddRMSampleID + "/" + ApproveGroupID + "/" + ApproveStatus + "/ApproveTrans");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/AddRawMaterialSample/Main");
            }
        }
    }
}
