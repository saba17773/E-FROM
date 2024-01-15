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
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Vender
{
    public class ApproveTransModel : PageModel
    {
        public int RequestID { get; set; }

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
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                await _authService.CanAccess(nameof(VenderPermissionModel.VIEW_VENDER));
                RequestID = id;

                return Page();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> OnPostGridAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<VenderApproveTransactionGridViewModel>($@"
                           SELECT *
                            FROM
                            (
                                SELECT VT.ID,VT.REQUESTID,VT.APPROVEMASTERID,VT.EMAIL,VT.APPROVELEVEL
                                ,CONVERT(VARCHAR,VT.SENDEMAILDATE,103)+' '+CONVERT(VARCHAR,VT.SENDEMAILDATE,108)SENDEMAILDATE
                                ,CONVERT(VARCHAR,VT.APPROVEDATE,103)+' '+CONVERT(VARCHAR,VT.APPROVEDATE,108)APPROVEDATE
                                ,CONVERT(VARCHAR,VT.REJECTDATE,103)+' '+CONVERT(VARCHAR,VT.REJECTDATE,108)REJECTDATE
                                ,VT.ISDONE,VT.REMARK
                                ,CASE WHEN VT.PROCESS = '' OR VT.PROCESS IS NULL THEN P.DESCRIPTION
                                ELSE PROCESS END AS DESCRIPTION
                                FROM TB_VenderApproveTrans VT JOIN
                                TB_VenderTable V ON VT.REQUESTID = V.ID JOIN
                                TB_VenderProcess P ON V.VENDPROCESSID = P.ID
                                WHERE VT.REQUESTID = {id} AND VT.ISCURRENTAPPROVE = 1
                            )T
                             GROUP BY T.ID,T.REQUESTID,T.APPROVEMASTERID,T.EMAIL,T.APPROVELEVEL,
                             T.SENDEMAILDATE,T.APPROVEDATE,T.REJECTDATE,T.ISDONE,T.REMARK,
                             T.DESCRIPTION
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
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    
                    var vendTranRepo = new GenericRepository<VenderApproveTrans_TB>(unitOfWork.Transaction);
                    var vendTran = await vendTranRepo.GetAsync(TranID);

                    var vendNonRepo = new GenericRepository<VenderNonce_TB>(unitOfWork.Transaction);
                    var vendNonALL = await vendNonRepo.GetAllAsync();
                    var vendNon = vendNonALL.Where(x => x.CreateDate == vendTran.SENDEMAILDATE
                    && x.IsUsed == 0).Select(s => s.NonceKey).SingleOrDefault();

                    var NonceKey = vendNon.ToString();

                    var Requestid = vendTran.REQUESTID;
                    var ApproveLevel = vendTran.APPROVELEVEL;
                    //var EmailTo = vendTran.EMAIL;
                    var Process = vendTran.PROCESS;

                    var vendTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var vendTable = await vendTableRepo.GetAsync(Requestid);
                    var CreateBy = vendTable.CREATEBY;
                    var ApproveMasterid = vendTable.APPROVEMASTERID;
                    var VendCode = "";
                    if(vendTable.VENDCODE_AX != null)
                    {
                        VendCode = vendTable.VENDCODE_AX;
                    }
                    else
                    {
                        VendCode = vendTable.VENDCODE;
                    }
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(CreateBy);
                    var EmployeeId = UserALL.EmployeeId;
                    var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var EmpALL = await EmpRepo.GetAllAsync();
                    var EmpFName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.Name).FirstOrDefault();
                    var EmpLName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.LastName).FirstOrDefault();
                    var EmpFullName = EmpFName + " " + EmpLName;

                    var appoveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                    var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                    var approveFlowName = appoveFlowALL.Where(x => x.ApproveMasterId == (int)ApproveMasterid && x.ApproveLevel == vendTran.APPROVELEVEL && x.IsActive == 1).Select(s => s.Name).FirstOrDefault();
                    var ApproveBy = approveFlowName;

                    //Sample Vendor Release Vendor Normal Vendor
                    if (vendTran.PROCESS == null || vendTran.PROCESS == "")
                    {
                        var vendMapRepo = new GenericRepository<VenderApproveMapping_TB>(unitOfWork.Transaction);
                        var vendMapALL = await vendMapRepo.GetAllAsync();
                        var vendMap = vendMapALL.Where(x => x.APPROVEMASTERID == ApproveMasterid && x.CreateBy == CreateBy).Select(s => s.APPROVEGROUPID).FirstOrDefault();
                        var vendGroupRepo = new GenericRepository<VenderApproveGroupMaster_TB>(unitOfWork.Transaction);
                        var vendGroup = await vendGroupRepo.GetAsync(vendMap);

                        //var fileok = new List<string>();
                        var filePath = $"wwwroot/files/VenderFiles/" + vendTable.REQUESTCODE.Substring(0, 4) + "_" + vendTable.REQUESTCODE.Substring(5, 2) + "_" + vendTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();

                        var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                        var vendLogFileALL = await vendLogFileRepo.GetAllAsync();

                        foreach (var filelog in vendLogFileALL.Where(x => x.REQUESTID == vendTable.ID && x.ISACTIVE == 1))
                        {
                            string fileName = filelog.FILENAME;
                           // fileok.Add($"{filePath}/{fileName}");
                        }

                        var subject = "";
                        // var urlpath = "";
                        if (vendGroup.ID == 1 || vendGroup.ID == 3)
                        {
                            subject = "Create " + vendGroup.DESCRIPTION;
                            //   urlpath = "ApproveVender";
                        }
                        else
                        {
                            subject = vendGroup.DESCRIPTION;
                            //   urlpath = "ApproveVender_Release";
                        }

                        var sendEmail = _emailService.SendEmail(
                              $"Subject : {vendTable.REQUESTCODE} / {subject} / ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ / {VendCode}",
                               $@"
                                    <b> ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ {VendCode} </b><br/>
                                    <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                    <b> รหัสผู้ขาย </b> {VendCode}<br/>
                                    <b> ชื่อผู้ขาย  </b> {vendTable.VENDNAME}<br/>
                                    <b> ที่อยู่     </b> {vendTable.ADDRESS}<br/>
                                    <b> ชื่อผู้ติดต่อ </b> {vendTable.CONTACTNAME}<br/>
                                    <b> โทรศัพท์  </b> {vendTable.TEL}<br/>
                                    <br/><br/><br/>
                                    <b> ชื่อผู้ทำการอนุมัติ </b> {ApproveBy}<br/></br/>
                                    <b> Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={vendTran.EMAIL}'>คลิกที่นี่</a> <br/>
                                ",
                              //<b>Link ���ʹ��Թ���:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/{urlpath}?VendID={vendTable.ID}&TranID={TranID}&nonce={NonceKey}'>��ԡ�����</a> <br/>
                              new List<string> { vendTran.EMAIL },
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
                    //ReviseSUCCESS INACTIVE
                    else
                    {
                        if (vendTable.ISREVISE == 1)
                        {
                            var vendTbTempRepo = new GenericRepository<VenderTable_TempTB>(unitOfWork.Transaction);
                            var vendTbTempALL = await vendTbTempRepo.GetAllAsync();
                            var vendTbTemp = vendTbTempALL.Where(x => x.REQUESTID == vendTable.ID && x.ISCOMPLETE == 0).FirstOrDefault();

                           // var fileok = new List<string>();
                            var filePath = $"wwwroot/files/VenderFiles/" + vendTable.REQUESTCODE.Substring(0, 4) + "_" + vendTable.REQUESTCODE.Substring(5, 2) + "_" + vendTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();

                            var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                            var vendLogFileALL = await vendLogFileRepo.GetAllAsync();

                            foreach (var filelog in vendLogFileALL.Where(x => (x.REQUESTID == vendTable.ID && x.ISACTIVE == 1) || (x.REQUESTID == vendTable.ID && x.ISTEMP == 1)))
                            {
                                string fileName = filelog.FILENAME;
                                // fileok.Add($"{filePath}/{fileName}");
                            }

                            var sendNextEmail = _emailService.SendEmail(
                               $"Subject : {vendTable.REQUESTCODE} / Revise / ร้องขอเพื่อแก้ไขข้อมูลผู้ขาย / {vendTable.VENDCODE_AX}",
                               $@"
                                    <b> ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ {vendTable.VENDCODE_AX} </b><br/>
                                    <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                    <b> รหัสผู้ขาย </b> {vendTable.VENDCODE_AX}<br/>
                                    <b> ชื่อผู้ขาย  </b> {vendTable.VENDNAME}<br/>
                                    <b> ที่อยู่     </b> {vendTbTemp.ADDRESS_TEMP}<br/>
                                    <b> ชื่อผู้ติดต่อ </b> {vendTbTemp.CONTACTNAME_TEMP}<br/>
                                    <b> โทรศัพท์  </b> {vendTbTemp.TEL_TEMP}<br/>
                                    <br/><br/><br/>
                                    <b style='color:red'> สาเหตุการร้องขอ Revise </b> {vendTbTemp.REMARK_TEMP}
                                    <br/><br/><br/>
                                    <b style='color:black'> ชื่อผู้ทำการอนุมัติ </b> {ApproveBy}<br/></br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={vendTran.EMAIL}'>คลิกที่นี่</a> <br/>
                                    
                                ",
                                //<b style='color:black'>Link ���ʹ��Թ���:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/ApproveVender_Revise?VendID={vendTable.ID}&TranID={TranID}&nonce={NonceKey}'>��ԡ�����</a> <br/>
                                new List<string> { vendTran.EMAIL },
                                new List<string> { },
                                "",
                                "",
                                new List<string> { }
                            );

                            if (sendNextEmail.Result == false)
                            {
                                throw new Exception(sendNextEmail.Message);
                            }

                        }
                        else
                        {
                            var subject = "";
                            var body = "";
                            if (vendTable.ISACTIVE == 0)
                            {
                                subject += "/ Inactive / ร้องขอเพื่อระงับผู้จำหน่ายในระบบ / ";
                                body += "ร้องขอเพื่อระงับผู้จำหน่ายในระบบ";
                            }
                            else
                            {
                                subject += "/ Active / ร้องขอเพื่อยกเลิกการระงับผู้จำหน่าย / ";
                                body += "ร้องขอเพื่อยกเลิกการระงับผู้จำหน่าย";
                            }

                            var vendTbTempRepo = new GenericRepository<VenderTable_TempTB>(unitOfWork.Transaction);
                            var vendTbTempALL = await vendTbTempRepo.GetAllAsync();
                            var vendTbTemp = vendTbTempALL.Where(x => x.REQUESTID == vendTable.ID && x.ISCOMPLETE == 0).FirstOrDefault();

                           // var fileok = new List<string>();
                            var filePath = $"wwwroot/files/VenderFiles/" + vendTable.REQUESTCODE.Substring(0, 4) + "_" + vendTable.REQUESTCODE.Substring(5, 2) + "_" + vendTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();

                            var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                            var vendLogFileALL = await vendLogFileRepo.GetAllAsync();

                            foreach (var filelog in vendLogFileALL.Where(x => (x.REQUESTID == vendTable.ID && x.ISACTIVE == 1) || (x.REQUESTID == vendTable.ID && x.ISTEMP == 1)))
                            {
                                string fileName = filelog.FILENAME;
                               // fileok.Add($"{filePath}/{fileName}");
                            }

                            if (vendTbTemp != null)
                            {

                                var sendNextEmail = _emailService.SendEmail(
                                   $"Subject : {vendTable.REQUESTCODE} {subject} {vendTable.VENDCODE_AX} ",
                                    $@"
                                        <b>{body} {vendTable.VENDCODE_AX} </b> <br/>
                                        <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                        <b> รหัสผู้ขาย </b> {vendTable.VENDCODE_AX}<br/>
                                        <b> ชื่อผู้ขาย  </b> {vendTable.VENDNAME}<br/>
                                        <b> ที่อยู่     </b> {vendTbTemp.ADDRESS_TEMP}<br/>
                                        <b> ชื่อผู้ติดต่อ </b> {vendTbTemp.CONTACTNAME_TEMP}<br/>
                                        <b> โทรศัพท์  </b> {vendTbTemp.TEL_TEMP}<br/>
                                        <br/><br/><br/>
                                        <b style='color:red'>สาเหตุที่{body}  </b> {vendTbTemp.REMARK_TEMP}<br/>
                                        <br/><br/><br/>
                                        <b> ชื่อผู้ทำการอนุมัติ </b> {ApproveBy}<br/></br/>
                                        <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={vendTran.EMAIL}'>คลิกที่นี่</a> <br/>
                                       
                                        ",
                                    // <b>Link ���ʹ��Թ���:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/ApproveVender_isActive?VendID={vendTable.ID}&TranID={TranID}&nonce={NonceKey}'>��ԡ�����</a> <br/>
                                    new List<string> { vendTran.EMAIL },
                                    new List<string> { },
                                    "",
                                    "",
                                    new List<string> { }
                                );
                                if (sendNextEmail.Result == false)
                                {
                                    throw new Exception(sendNextEmail.Message);
                                }
                            }
                            else
                            {
                                var sendNextEmail = _emailService.SendEmail(
                                    $"Subject : {vendTable.REQUESTCODE} {subject} {vendTable.VENDCODE_AX} ",
                                    $@"
                                        <b>{body} {vendTable.VENDCODE_AX} </b> <br/>
                                        <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                        <b> รหัสผู้ขาย </b> {vendTable.VENDCODE_AX}<br/>
                                        <b> ชื่อผู้ขาย  </b> {vendTable.VENDNAME}<br/>
                                        <b> ที่อยู่     </b> {vendTable.ADDRESS}<br/>
                                        <b> ชื่อผู้ติดต่อ </b> {vendTable.CONTACTNAME}<br/>
                                        <b> โทรศัพท์  </b> {vendTable.TEL}<br/>
                                        <br/><br/><br/>
                                        <b style='color:red'>สาเหตุที่{body}  </b> {vendTable.REMARK}<br/>
                                        <br/><br/><br/>
                                        <b> ชื่อผู้ทำการอนุมัติ </b> {ApproveBy}<br/></br/>
                                        <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={vendTran.EMAIL}'>คลิกที่นี่</a> <br/>
                                     ",
                                    //<b>Link ���ʹ��Թ���:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/ApproveVender_isActive?VendID={vendTable.ID}&TranID={TranID}&nonce={NonceKey}'>��ԡ�����</a> <br/>
                                    new List<string> { vendTran.EMAIL },
                                    new List<string> { },
                                    "",
                                    "",
                                    new List<string> { }
                                );
                                if (sendNextEmail.Result == false)
                                {
                                    throw new Exception(sendNextEmail.Message);
                                }
                            }
                        }
                    }

                        
                        unitOfWork.Complete();

                    AlertSuccess = "Resend Email successfully"; 
                    return Redirect("/Vender/" + vendTran.REQUESTID + "/ApproveTrans");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
