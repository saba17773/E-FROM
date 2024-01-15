using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Ocsp;
using Renci.SshNet.Messages;
using Web.UI.Contexts;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;
using Web.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using System.Net.Http;
using System.Collections;

namespace Web.UI.Pages.Vender
{
    public class AutoSendMailModel : PageModel
    {
        private IDatabaseContext _databaseContext;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public AutoSendMailModel(
          IDatabaseContext databaseContext,
          IEmailService emailService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _emailService = emailService;
            _configuration = configuration;
        }  

        public async Task<IActionResult> OnGetCompleteVenderAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var VendTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var VendTableALL = await VendTableRepo.GetAllAsync();

                    foreach (var VendTB in VendTableALL.Where(x => x.APPROVESTATUS == 6))
                    {
                        var approvemasterid = VendTB.APPROVEMASTERID;
                       // var VendTB_Update = await VendTableRepo.GetAsync((int)VendTB.ID);

                        var emailsuccess = new List<string>();
                        var ApproveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                        var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();

                        foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenComplete == 1))
                        {
                            emailsuccess.Add(emaillog.Email);
                        }

                        
                        var sendEmail = _emailService.SendEmail(
                             $"Subject : {VendTB.REQUESTCODE} / {VendTB.VENDCODE_AX} / สร้างVenderจริงเรียบร้อย /รอการอนุมัติ",
                              $@"
                                <b>Request Code : {VendTB.REQUESTCODE} </b> <br/>
                                <b>รหัสผู้ขาย : {VendTB.VENDCODE_AX} </b> <br/> 
                            ",
                             emailsuccess,
                             new List<string> { }
                       );

                        if (sendEmail.Result == false)
                        {
                            throw new Exception(sendEmail.Message);
                        }

                    }

                    unitOfWork.Complete();
                    return new JsonResult("Successfully");

                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        public async Task<IActionResult> OnGetSuccessVenderAsync()
        {
            try
            {

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var VendTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var VendTableALL = await VendTableRepo.GetAllAsync();
                    //1,6
                    foreach (var VendTB in VendTableALL.Where(x => x.APPROVESTATUS == 6 && x.SENDMAILSUCCESS == 0))
                    {
                        var approvemasterid = VendTB.APPROVEMASTERID;
                        var VendTB_Update = await VendTableRepo.GetAsync((int)VendTB.ID);
                        VendTB_Update.SENDMAILSUCCESS = 1;
                        await VendTableRepo.UpdateAsync(VendTB_Update);
                        var emailsuccess = new List<string>();
                        var ApproveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                        var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();

                        /*foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenComplete == 1))
                        {
                            emailsuccess.Add(emaillog.Email);
                        }*/
                        var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                        var UserALL = await UserRepo.GetAsync(VendTB.CREATEBY);
                        emailsuccess.Add(UserALL.Email);


                        var sendEmail = _emailService.SendEmail(
                             $"Subject : {VendTB.REQUESTCODE} / {VendTB.VENDCODE_AX} /ดำเนินการ Create Sample Vendor เรียบร้อยแล้ว",
                              $@"
                                <b>ดำเนินการ Create Sample Vendor เรียบร้อยแล้ว</b>
                                <b>เลขที่คำขอ : {VendTB.REQUESTCODE} </b> <br/>
                                <b>รหัสผู้ขาย : {VendTB.VENDCODE_AX} </b> <br/> 
                                <b>ชื่อผู้ขาย : {VendTB.VENDNAME} </b> <br/> 
                            ",
                             emailsuccess,
                             new List<string> { }
                       );

                        if (sendEmail.Result == false)
                        {
                            throw new Exception(sendEmail.Message);
                        }
                    }
                    //3,5
                    foreach (var VendTB in VendTableALL.Where(x => x.APPROVESTATUS == 7 && x.SENDMAILSUCCESS == 0))
                    {
                        var approvemasterid = VendTB.APPROVEMASTERID;
                        var VendTB_Update = await VendTableRepo.GetAsync((int)VendTB.ID);

                        VendTB_Update.SENDMAILSUCCESS = 1;
                        VendTB_Update.COMPLETEDATE = DateTime.Now;

                        await VendTableRepo.UpdateAsync(VendTB_Update);

                        var emailsuccess = new List<string>();
                        var ApproveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                        var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();

                        /*foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenComplete == 1))
                        {
                            emailsuccess.Add(emaillog.Email);
                        }*/

                        var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                        var UserALL = await UserRepo.GetAsync(VendTB.CREATEBY);
                        emailsuccess.Add(UserALL.Email);

                        var subject = "";

                        if (VendTB.VENDPROCESSID == 2)
                        {
                            subject = "Release";
                        }
                        else if (VendTB.VENDPROCESSID == 3)
                        {
                            subject = "Create Normal Vendor";
                        }

                        var sendEmail = _emailService.SendEmail(
                             $"Subject : {VendTB.REQUESTCODE} / {VendTB.VENDCODE_AX} /ดำเนินการ {subject} เรียบร้อยแล้ว",
                              $@"
                                <b>ดำเนินการ {subject} เรียบร้อยแล้ว</b>
                                <b>เลขที่คำขอ : {VendTB.REQUESTCODE} </b> <br/>
                                <b>รหัสผู้ขาย : {VendTB.VENDCODE_AX} </b> <br/> 
                                <b>ชื่อผู้ขาย : {VendTB.VENDNAME} </b> <br/> 
                            ",
                             emailsuccess,
                             new List<string> { }
                       );

                        if (sendEmail.Result == false)
                        {
                            throw new Exception(sendEmail.Message);
                        }

                    }

                    unitOfWork.Complete();
                    return new JsonResult("Successfully");

                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        public async Task<IActionResult> OnGetAlertMailAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var datamail = await unitOfWork.Transaction.Connection.QueryAsync<VenderApproveTrans_TB>($@"
                            SELECT T.EMAIL
                            FROM
                            (
	                            SELECT VT.*
	                            ,CASE WHEN VT.ISALERT = 0 THEN VT.SENDEMAILDATE
	                            ELSE (SELECT SENDEMAILDATE FROM TB_VenderLogSendEmailAlert L 
                                        WHERE L.REQUESTID = VT.REQUESTID AND L.ISLASTLOG = 1
                                        AND L.EMAIL = VT.EMAIL)
	                            END AS LASTSENDEMAILDATE
	                            FROM TB_VenderApproveTrans VT JOIN
	                            TB_VenderNonce N ON VT.SENDEMAILDATE = N.CreateDate 
	                            WHERE SENDEMAILDATE IS NOT NULL AND APPROVEDATE IS NULL AND REJECTDATE IS NULL
	                            AND VT.ISDONE = 0 AND N.IsUsed = 0  
                                AND (VT.ISALERT = 0 OR (VT.ISALERT = 1 AND VT.ISSKIPALERT = 0)) 
                            )T
                            WHERE DATEADD(DAY,2,T.LASTSENDEMAILDATE) < GETDATE()
                            GROUP BY T.EMAIL", 
                            null, 
                            unitOfWork.Transaction
                    );

                    foreach (var mail in datamail)
                    {
                        
                        using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection()))
                        {
                            var dataalert1 = await unitOfWork2.Transaction.Connection.QueryAsync<VenderApproveTrans_TB>($@"
                                SELECT T.*
                                FROM
                                (
	                                SELECT VT.*
	                                ,CASE WHEN VT.ISALERT = 0 THEN VT.SENDEMAILDATE
	                                ELSE (SELECT SENDEMAILDATE FROM TB_VenderLogSendEmailAlert L 
                                            WHERE L.REQUESTID = VT.REQUESTID AND L.ISLASTLOG = 1
                                            AND L.EMAIL = VT.EMAIL)
	                                END AS LASTSENDEMAILDATE
	                                FROM TB_VenderApproveTrans VT JOIN
	                                TB_VenderNonce N ON VT.SENDEMAILDATE = N.CreateDate 
	                                WHERE SENDEMAILDATE IS NOT NULL AND APPROVEDATE IS NULL AND REJECTDATE IS NULL
	                                AND VT.ISDONE = 0 AND N.IsUsed = 0  
                                    AND (VT.ISALERT = 0 OR (VT.ISALERT = 1 AND VT.ISSKIPALERT = 0))
                                )T
                                WHERE DATEADD(DAY,2,T.LASTSENDEMAILDATE) < GETDATE()
                                AND T.EMAIL = @mail ",
                               new { @mail = mail.EMAIL },
                               unitOfWork2.Transaction
                            );

                            foreach (var data in dataalert1)
                            {
                                var DatetimeNow = DateTime.Now;
                                var VenSendMailLogRepo = new GenericRepository<VenderLogSendEmailAlert_TB>(unitOfWork2.Transaction);
                                var VenSendMailLog = await VenSendMailLogRepo.GetAllAsync();
                                var logsendid = VenSendMailLog.Where(x => x.REQUESTID == data.REQUESTID && 
                                                                    x.ISLASTLOG == 1 && x.EMAIL== data.EMAIL).Select(s => s.ID).FirstOrDefault();

                               //throw new Exception(logsendid.ToString());
                                
                                if(logsendid != 0)
                                {
                                    var VenSendMailLog_update = await VenSendMailLogRepo.GetAsync(logsendid);
                                    VenSendMailLog_update.ISLASTLOG = 0;
                                    await VenSendMailLogRepo.UpdateAsync(VenSendMailLog_update);
                                    var VendLogSendRepo = new GenericRepository<VenderLogSendEmailAlert_TB>(unitOfWork2.Transaction);
                                    await VendLogSendRepo.InsertAsync(new VenderLogSendEmailAlert_TB
                                    {
                                        REQUESTID = data.REQUESTID,
                                        EMAIL = data.EMAIL,
                                        SENDEMAILDATE = DatetimeNow,
                                        ISLASTLOG = 1
                                    });
                                }
                                else
                                {
                                    var VendLogSendRepo = new GenericRepository<VenderLogSendEmailAlert_TB>(unitOfWork2.Transaction);
                                    await VendLogSendRepo.InsertAsync(new VenderLogSendEmailAlert_TB
                                    {
                                        REQUESTID = data.REQUESTID,
                                        EMAIL = data.EMAIL,
                                        SENDEMAILDATE = DatetimeNow,
                                        ISLASTLOG = 1
                                    });
                                }

                                if (data.ISALERT == 0)
                                {
                                    var alert2 = await unitOfWork2.Transaction.Connection.QueryAsync<VenderApproveTrans_TB>($@"
                                        SELECT T.*
                                        FROM
                                        (
	                                        SELECT VT.*
	                                        ,CASE WHEN VT.ISALERT = 0 THEN VT.SENDEMAILDATE
	                                        ELSE (SELECT SENDEMAILDATE FROM TB_VenderLogSendEmailAlert L 
                                                    WHERE L.REQUESTID = VT.REQUESTID AND L.ISLASTLOG = 1
                                                    AND L.EMAIL = VT.EMAIL)
	                                        END AS LASTSENDEMAILDATE
	                                        FROM TB_VenderApproveTrans VT JOIN
	                                        TB_VenderNonce N ON VT.SENDEMAILDATE = N.CreateDate 
	                                        WHERE SENDEMAILDATE IS NOT NULL AND APPROVEDATE IS NULL AND REJECTDATE IS NULL
	                                        AND VT.ISDONE = 0 AND N.IsUsed = 0  
                                            AND VT.ISALERT = 0
                                        )T
                                        WHERE DATEADD(DAY,2,T.LASTSENDEMAILDATE) < GETDATE()
                                        AND T.EMAIL = @mail",
                                      new { @mail = data.EMAIL },
                                      unitOfWork2.Transaction
                                   );

                                    foreach (var data2 in alert2)
                                    {
                                        var VendTranRepo2 = new GenericRepository<VenderApproveTrans_TB>(unitOfWork2.Transaction);
                                        var VendTran = await VendTranRepo2.GetAsync((int)data2.ID);
                                        VendTran.ISALERT = 1;
                                        await VendTranRepo2.UpdateAsync(VendTran);

                                    }
                                }
                            }

                            unitOfWork2.Complete();
                        }

                        var sendEmail = _emailService.SendEmail(
                            $"Subject : Request wating for approve vendor. ",
                            $@" <b>Request wating for approve vendor.</b>
                                <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={mail.EMAIL}'>คลิกที่นี่</a> <br/>
                            ",
                            new List<string> { mail.EMAIL },
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
                    return new JsonResult("Successfully");
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        public async Task<IActionResult> OnGetSkipApproveAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var dataWaitingSkip = await unitOfWork.Transaction.Connection.QueryAsync<VenderApproveTrans_TB>($@"
                            SELECT T.REQUESTID,T.ISCURRENTAPPROVE,T.APPROVELEVEL,T.SENDEMAILDATE
                            FROM
                            (
	                            SELECT VT.*
	                            ,CASE WHEN VT.ISALERT = 0 THEN VT.SENDEMAILDATE
	                            ELSE (SELECT SENDEMAILDATE FROM TB_VenderLogSendEmailAlert L 
                                        WHERE L.REQUESTID = VT.REQUESTID AND L.ISLASTLOG = 1
                                        AND L.EMAIL = VT.EMAIL)
	                            END AS LASTSENDEMAILDATE
	                            FROM TB_VenderApproveTrans VT JOIN
	                            TB_VenderNonce N ON VT.SENDEMAILDATE = N.CreateDate 
	                            WHERE SENDEMAILDATE IS NOT NULL AND APPROVEDATE IS NULL AND REJECTDATE IS NULL
	                            AND VT.ISDONE = 0 AND N.IsUsed = 0  
                                AND VT.ISALERT = 1 AND VT.ISSKIPALERT = 1
                                AND VT.ISCURRENTAPPROVE = 1 
                            )T
                            WHERE DATEADD(DAY,0,T.LASTSENDEMAILDATE) < GETDATE()
                            GROUP BY T.REQUESTID,T.ISCURRENTAPPROVE,T.APPROVELEVEL,T.SENDEMAILDATE
                        ",
                        null,
                        unitOfWork.Transaction
                    );

                    var test = "";
                    foreach (var datawait in dataWaitingSkip)
                    {
                        //update vendor table
                        var DatetimeNow = DateTime.Now;
                        var VendTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                        var VendTable = await VendTableRepo.GetAsync((int)datawait.REQUESTID);

                        var NextStep = VendTable.CURRENTAPPROVESTEP + 1;
                        VendTable.CURRENTAPPROVESTEP += 1;

                        //update old nonce
                        var VendNonceRepo = new GenericRepository<VenderNonce_TB>(unitOfWork.Transaction);
                        var VendNonce = await VendNonceRepo.GetAllAsync();
                        var nonce = VendNonce.Where(x => x.CreateDate == datawait.SENDEMAILDATE).Select(s => s.NonceKey).FirstOrDefault();
                        var _nonce = await unitOfWork.VenderNonce.GetNonceByKey(nonce);
                        _nonce.IsUsed = 1;
                        await VendNonceRepo.UpdateAsync(_nonce);

                        //insert log approve by system
                        var newLogsApproveRepo = new GenericRepository<VenderLogsApprove_TB>(unitOfWork.Transaction);
                        var newLogsApprove = new VenderLogsApprove_TB
                        {
                            REQUESTID = datawait.REQUESTID,
                            EMAIL = "System skip approve",
                            CREATEDATE = DatetimeNow,
                            ISAPPROVE = 1,
                            ISREJECT = 0
                        };
                        var LogsApprove = await newLogsApproveRepo.InsertAsync(newLogsApprove);

                        //get request by name
                        var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                        var UserALL = await UserRepo.GetAsync(VendTable.CREATEBY);
                        var EmployeeId = UserALL.EmployeeId;
                        var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                        var EmpALL = await EmpRepo.GetAllAsync();
                        var EmpFName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.Name).FirstOrDefault();
                        var EmpLName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.LastName).FirstOrDefault();
                        var EmpFullName = EmpFName + " " + EmpLName;

                        //get file request id
                        //var fileok = new List<string>();
                        var filePath = $"wwwroot/files/VenderFiles/" + VendTable.REQUESTCODE.Substring(0, 4) + "_" + VendTable.REQUESTCODE.Substring(5, 2) + "_" + VendTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();
                        var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                        var vendLogFileALL = await vendLogFileRepo.GetAllAsync();

                        foreach (var filelog in vendLogFileALL.Where(x => (x.REQUESTID == VendTable.ID && x.ISACTIVE == 1) || (x.REQUESTID == VendTable.ID && x.ISTEMP == 1)))
                        {
                            string fileName = filelog.FILENAME;
                            //fileok.Add($"{filePath}/{fileName}");
                        }

                        var Subject = "";
                        var Body = "";

                        //get group request id
                        var AppMappingRepo = new GenericRepository<VenderApproveMapping_TB>(unitOfWork.Transaction);
                        var AppMapping = await AppMappingRepo.GetAllAsync();
                        var GroupID = AppMapping.Where(x => x.APPROVEMASTERID == VendTable.APPROVEMASTERID && x.CreateBy == VendTable.CREATEBY).Select(s => s.APPROVEGROUPID).FirstOrDefault();
                        var GroupVendRepo = new GenericRepository<VenderApproveGroupMaster_TB>(unitOfWork.Transaction);
                        var GroupVend = await GroupVendRepo.GetAsync((int)GroupID);

                        using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection()))
                        {
                            var dataskip = await unitOfWork2.Transaction.Connection.QueryAsync<VenderApproveTrans_TB>($@"
                                 SELECT T.*
                                 FROM
                                 (
                                     SELECT VT.*
                                     ,CASE WHEN VT.ISALERT = 0 THEN VT.SENDEMAILDATE
                                     ELSE (SELECT SENDEMAILDATE FROM TB_VenderLogSendEmailAlert L 
                                         WHERE L.REQUESTID = VT.REQUESTID AND L.ISLASTLOG = 1
                                         AND L.EMAIL = VT.EMAIL)
                                     END AS LASTSENDEMAILDATE
                                     FROM TB_VenderApproveTrans VT JOIN
                                     TB_VenderNonce N ON VT.SENDEMAILDATE = N.CreateDate 
                                     WHERE SENDEMAILDATE IS NOT NULL AND APPROVEDATE IS NULL AND REJECTDATE IS NULL
                                     AND VT.ISDONE = 0 AND N.IsUsed = 0  
                                     AND VT.ISALERT = 1 AND VT.ISSKIPALERT = 1 
                                     AND VT.ISCURRENTAPPROVE = 1 
                                 )T
                                 WHERE DATEADD(DAY,0,T.LASTSENDEMAILDATE) < GETDATE()
                                 AND REQUESTID = @RequestID ",
                                new
                                {
                                     @RequestID = datawait.REQUESTID
                                },
                                unitOfWork2.Transaction
                            );

                            foreach (var skip in dataskip)
                            {
                                //update old LogSendmail
                                var VenSendMailLogRepo = new GenericRepository<VenderLogSendEmailAlert_TB>(unitOfWork2.Transaction);
                                var VenSendMailLog = await VenSendMailLogRepo.GetAllAsync();
                                var logsendid = VenSendMailLog.Where(x => x.REQUESTID == skip.REQUESTID &&
                                                                    x.ISLASTLOG == 1 &&
                                                                    x.EMAIL == skip.EMAIL).Select(s => s.ID).FirstOrDefault();

                                var VenSendMailLog_update = await VenSendMailLogRepo.GetAsync(logsendid);
                                VenSendMailLog_update.ISLASTLOG = 0;
                                await VenSendMailLogRepo.UpdateAsync(VenSendMailLog_update);

                                //update old Approve Trans
                                var VendTransRepo = new GenericRepository<VenderApproveTrans_TB>(unitOfWork2.Transaction);
                                var VendTrans = await VendTransRepo.GetAsync(skip.ID);
                                VendTrans.ISDONE = 1;
                                await VendTransRepo.UpdateAsync(VendTrans);

                                //generate nonce next approve
                                var nonceRepo = new GenericRepository<VenderNonce_TB>(unitOfWork2.Transaction);
                                var nonceKey = Guid.NewGuid().ToString();
                                await nonceRepo.InsertAsync(new VenderNonce_TB
                                {
                                    NonceKey = nonceKey,
                                    CreateDate = DatetimeNow,
                                    ExpireDate = DatetimeNow.AddDays(7),
                                    IsUsed = 0
                                });

                                // next approve trans
                                var approveTransByRequestID = await unitOfWork.VenderControl.GetApproveTransByRequestID(skip.REQUESTID, skip.APPROVEMASTERID);
                                var approveTransNextLevel = approveTransByRequestID.Where(x => x.APPROVELEVEL == NextStep);

                                foreach (var next in approveTransNextLevel)
                                {
                                    //next approve name
                                    var appoveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork2.Transaction);
                                    var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                                    var approveFlowName = appoveFlowALL.Where(x => x.ApproveMasterId == skip.APPROVEMASTERID &&
                                                                      x.ApproveLevel == NextStep && x.IsActive == 1 &&
                                                                      x.Email == next.EMAIL).Select(s => s.Name).FirstOrDefault();

                                    var ApproveBy = approveFlowName;

                                    if (VendTrans.PROCESS == null && VendTable.COMPLETEDATE == null)
                                    {
                                        var Vendcode = "";
                                        if (VendTable.VENDCODE_AX == null)
                                        {
                                            Vendcode = VendTable.VENDCODE;
                                        }
                                        else
                                        {
                                            Vendcode = VendTable.VENDCODE_AX;
                                        }

                                        Subject = $"Subject : {VendTable.REQUESTCODE} / Create {GroupVend.DESCRIPTION} / ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ / {Vendcode}";

                                        Body = $@"
                                             <b> ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ {Vendcode} </b><br/>
                                             <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                             <b> รหัสผู้ขาย </b> {Vendcode}<br/>
                                             <b> ชื่อผู้ขาย  </b> {VendTable.VENDNAME}<br/>
                                             <b> ที่อยู่     </b> {VendTable.ADDRESS}<br/>
                                             <b> ชื่อผู้ติดต่อ </b> {VendTable.CONTACTNAME}<br/>
                                             <b> โทรศัพท์  </b> {VendTable.TEL}<br/>
                                             <br/><br/><br/>
                                             <b> ชื่อผู้ทำการอนุมัติ </b> {ApproveBy}<br/></br/>
                                             <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={next.EMAIL}'>คลิกที่นี่</a> <br/>
                                         ";

                                    }
                                    else
                                    {
                                        var sub = "";
                                        var bod = "";
                                        if (VendTrans.PROCESS == "InActive")
                                        {
                                            sub = "Inactive / ร้องขอเพื่อระงับผู้จำหน่ายในระบบ";
                                            bod = "ร้องขอเพื่อระงับผู้จำหน่ายในระบบ";
                                        }
                                        else if (VendTrans.PROCESS == "Active")
                                        {
                                            sub = "Active / ร้องขอเพื่อยกเลิกการระงับผู้จำหน่าย";
                                            bod = "ร้องขอเพื่อยกเลิกการระงับผู้จำหน่าย";
                                        }
                                        else if (VendTrans.PROCESS == "Revise")
                                        {
                                            sub = "Revise / ร้องขอเพื่อแก้ไขข้อมูลผู้ขาย";
                                            bod = "ร้องขอเพื่อแก้ไขข้อมูลผู้ขาย";
                                        }

                                        Subject = $"Subject : {VendTable.REQUESTCODE} / {sub} / {VendTable.VENDCODE_AX}";

                                        Body = $@"
                                             <b> {bod} {VendTable.VENDCODE_AX} </b><br/>
                                             <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                             <b> รหัสผู้ขาย </b> {VendTable.VENDCODE_AX}<br/>
                                             <b> ชื่อผู้ขาย  </b> {VendTable.VENDNAME}<br/>
                                             <b> ที่อยู่     </b> {VendTable.ADDRESS}<br/>
                                             <b> ชื่อผู้ติดต่อ </b> {VendTable.CONTACTNAME}<br/>
                                             <b> โทรศัพท์  </b> {VendTable.TEL}<br/>
                                             <br/><br/><br/>
                                             <b> ชื่อผู้ทำการอนุมัติ </b> {ApproveBy}<br/></br/>
                                             <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={next.EMAIL}'>คลิกที่นี่</a> <br/>
                                         ";

                                    }

                                    var sendNextEmail = _emailService.SendEmail(
                                       $"Subject : {Subject}",
                                       $@"{Body}",
                                       new List<string> { next.EMAIL },
                                       new List<string> { },
                                       "",
                                       "",
                                       new List<string> { }
                                    );

                                    if (sendNextEmail.Result == false)
                                    {
                                        throw new Exception(sendNextEmail.Message);
                                    }

                                    test += "Trans ID : " + skip.ID + "\r\n" +
                                            " ApproveMasterID :  " + skip.APPROVEMASTERID + "\r\n" +
                                            " RequestID :  " + skip.REQUESTID + "\r\n" +
                                            " Email Skip : " + skip.EMAIL + "\r\n" +
                                            " RequestName : " + EmpFullName + "\r\n" +
                                            " NextStep : " + NextStep + "\r\n" +
                                            " Next Trans ID: " + next.ID + "\r\n" +
                                            " Next Approve Email: " + next.EMAIL + "\r\n" +
                                            " Next Approve Name: " + ApproveBy + "\r\n" +
                                            "-------------------------------------------- \r\n";
                                }

                                var VendLogSendRepo = new GenericRepository<VenderLogSendEmailAlert_TB>(unitOfWork2.Transaction);
                                await VendLogSendRepo.InsertAsync(new VenderLogSendEmailAlert_TB
                                {
                                    REQUESTID = skip.REQUESTID,
                                    EMAIL = skip.EMAIL,
                                    SENDEMAILDATE = DatetimeNow,
                                    ISLASTLOG = 1
                                });

                            }

                            unitOfWork2.Complete();
                        }
                        await VendTableRepo.UpdateAsync(VendTable);
                        //update next approve trans
                        var VendTranNextRepo = new GenericRepository<VenderApproveTrans_TB>(unitOfWork.Transaction);
                        var approveTransNextAll = await VendTranNextRepo.GetAllAsync();
                        var approveTransNext = approveTransNextAll.Where(x => x.REQUESTID == datawait.REQUESTID &&
                                                x.APPROVELEVEL == NextStep && x.ISCURRENTAPPROVE == 1);

                        foreach (var nextupdate in approveTransNext)
                        {
                            nextupdate.SENDEMAILDATE = DatetimeNow;
                            await VendTranNextRepo.UpdateAsync(nextupdate);

                            test += "Next Update ID : " + nextupdate.ID +"\r\n" +
                                    "SENDMaildate : " + nextupdate.SENDEMAILDATE + "\r\n";
                        }


                    }

                    unitOfWork.Complete();
                    return new JsonResult("Successfully");
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
