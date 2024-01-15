using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
    public class ApproveVender_ReleaseModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        [StringLength(20)]
        [MaxLength(13)]
        public string VenderIDNum { get; set; }

        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public string VenderCode { get; set; }

        [BindProperty]
        public string VenderCodeAX { get; set; }

        [BindProperty]
        [StringLength(150)]
        public string VenderName { get; set; }

        [BindProperty]
        [StringLength(50)]
        public string ContactName { get; set; }

        [BindProperty]
        [StringLength(20)]
        public string Telephone { get; set; }

        [BindProperty]
        [StringLength(20)]
        public string Fax { get; set; }

        [BindProperty]
        [StringLength(100)]
        public string Website { get; set; }

        [BindProperty]
        [StringLength(50)]
        public string Email { get; set; }

        [BindProperty]
        public string VenderGroup { get; set; }

        [BindProperty]
        public string VenderType { get; set; }

        [BindProperty]
        [StringLength(10)]
        public string Currency { get; set; }

        [BindProperty]
        public string ProductType { get; set; }

        [BindProperty]
        public string Paymterm { get; set; }

        [BindProperty]
        public DateTime? RequestDate { get; set; }

        [BindProperty]
        [StringLength(250)]
        public string Remark { get; set; }

        [BindProperty]
        public bool DocRef1 { get; set; }
        [BindProperty]
        public bool DocRef2 { get; set; }
        [BindProperty]
        public bool DocRef3 { get; set; }
        [BindProperty]
        public bool DocRef4 { get; set; }
        [BindProperty]
        public bool DocRef5 { get; set; }
        [BindProperty]
        public bool DocRef6 { get; set; }
        [BindProperty]
        public bool DocRef7 { get; set; }
        [BindProperty]
        public bool DocRef8 { get; set; }
        [BindProperty]
        public bool DocRef9 { get; set; }
        [BindProperty]
        public bool DocRef10 { get; set; }
        [BindProperty]
        public bool DocRef11 { get; set; }
        [BindProperty]
        public bool DocRef12 { get; set; }
        [BindProperty]
        public bool DocRef13 { get; set; }
        [BindProperty]
        public bool DocRef14 { get; set; }
        [BindProperty]
        public string DocRef15 { get; set; }
        [BindProperty]
        public bool DocRef16 { get; set; }
        [BindProperty]
        public bool DocRef17 { get; set; }
        [BindProperty]
        public bool DocRef18 { get; set; }
        [BindProperty]
        public bool DocRef19 { get; set; }
        [BindProperty]
        public bool DocRef20 { get; set; }
        [BindProperty]
        public bool DocRef21 { get; set; }
        [BindProperty]
        public bool DocRef22 { get; set; }
        [BindProperty]
        public bool DocRef23 { get; set; }
        [BindProperty]
        public string DocRef6_Desc { get; set; }
        [BindProperty]
        public string DocRef16_Desc { get; set; }
        [BindProperty]
        public string DocRef9_Desc { get; set; }

        [BindProperty]
        public string Address { get; set; }

        [BindProperty]
        public int CreateBy { get; set; }

        [BindProperty]
        public int isActive { get; set; }

        [BindProperty]
        public int RequestID { get; set; }

        [BindProperty]
        public string ApproveRemark { get; set; }

        [BindProperty]
        [Required]
        public int ApproveResult { get; set; }
        [BindProperty]
        public string ProductTypeDetail { get; set; }
        [BindProperty]
        public string DataAreaId { get; set; }
        [BindProperty]
        public bool DataAreaId1 { get; set; }
        [BindProperty]
        public bool DataAreaId2 { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public ApproveVender_ReleaseModel(
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

        public async Task<IActionResult> OnGetAsync(int VendID, int TranID, string nonce)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var _nonce = await unitOfWork.VenderNonce.GetNonceByKey(nonce);
                    // if (_nonce.ExpireDate <= DateTime.Now || _nonce.IsUsed == 1)
                    if (_nonce.IsUsed == 1)
                    {
                        AlertError = "Link Is Used.";
                        return Redirect($"/");
                    }

                    RequestID = VendID;
                    await GetData(VendID);

                    /*var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var vengroupRepo = new GenericRepository<VenderGroup_TB>(unitOfWork.Transaction);
                    var ventypeRepo = new GenericRepository<VenderType_TB>(unitOfWork.Transaction);
                    //var prodtypeRepo = new GenericRepository<VenderProductType_TB>(unitOfWork.Transaction);

                    var Vender = await venderTableRepo.GetAsync(VendID);
                    var VenderGroup_Desc = await vengroupRepo.GetAsync(Vender.VENDGROUPID);
                    var VenderType_Desc = await ventypeRepo.GetAsync(Vender.VENDTYPEID);
                    //var ProdType_Desc = await prodtypeRepo.GetAllAsync();

                    VenderIDNum = Vender.VENDIDNUM;
                    VenderName = Vender.VENDNAME;
                    VenderGroup = VenderGroup_Desc.DESCRIPTION;
                    VenderType = VenderType_Desc.DESCRIPTION;
                    ProductType = Vender.PRODTYPEID;
                   // ProductType = ProdType_Desc.Where(x => x.CODE == Vender.PRODTYPEID).Select(s => s.DESCRIPTION).FirstOrDefault();

                    unitOfWork.Complete();*/

                    return Page();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> OnPostAsync(int VendID, int TranID, string nonce,string email)
        {
            try
            {
                if (ApproveResult == 0)
                {
                    AlertError = "กรุณาเลือกว่าจะ อนุมัติ หรือ ไม่อนุมัติ";
                }
                if (ApproveResult == 2 && ApproveRemark == null)
                {

                    AlertError = "กรุณาใส่สาเหตุที่ไม่อนุมัติ !!";
                    return Redirect($"/Vender/ApproveVender_Release?VendID={VendID}&TranID={TranID}&nonce={nonce}&email={email}");
                }

                if (!ModelState.IsValid)
                {
                    return Redirect($"/Vender/ApproveVender_Release?VendID={VendID}&TranID={TranID}&nonce={nonce}&email={email}");
                }

                

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveTransRepo = new GenericRepository<VenderApproveTrans_TB>(unitOfWork.Transaction);
                    var nonceRepo = new GenericRepository<VenderNonce_TB>(unitOfWork.Transaction);

                    var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var venderTable = await venderTableRepo.GetAsync(VendID);

                    //int approvemasterid = 11;
                    var approveMapRepo = new GenericRepository<VenderApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approvemasterid = approveMapALL.Where(x => x.CreateBy == venderTable.CREATEBY && x.STEP == 2).Select(s => s.APPROVEMASTERID).FirstOrDefault();

                    var DatetimeNow = DateTime.Now;
                    // check nonce
                    var _nonce = await unitOfWork.VenderNonce.GetNonceByKey(nonce);

  
                    if (_nonce.IsUsed == 1)
                    {
                        throw new Exception("Link Is Used.");
                    }

                    _nonce.IsUsed = 1;

                    var approveTrans = await approveTransRepo.GetAsync(TranID);
                    var approvelevel = approveTrans.APPROVELEVEL;

                    var approveTransRepo2 = new GenericRepository<VenderApproveTrans_TB>(unitOfWork.Transaction);
                    var approveTranAllLevel = await approveTransRepo2.GetAllAsync();
                    var approveTranLevel = approveTranAllLevel.Where(x => x.REQUESTID == VendID &&
                                                                    x.APPROVEMASTERID == approvemasterid &&
                                                                    x.APPROVELEVEL == approvelevel &&
                                                                    x.ISCURRENTAPPROVE == 1);

                    foreach (var approveTransUpdate in approveTranLevel)
                    {
                        // update approve trans
                        approveTransUpdate.ISDONE = 1;

                        if (approveTransUpdate.EMAIL == email)
                        {
                            approveTransUpdate.REMARK = ApproveRemark;

                            if (ApproveResult == 1)
                            {
                                approveTransUpdate.APPROVEDATE = DatetimeNow;
                            }
                            else if (ApproveResult == 2)
                            {

                                approveTransUpdate.REJECTDATE = DatetimeNow;
                            }
                        }

                        await approveTransRepo2.UpdateAsync(approveTransUpdate);
                    }

                    var approveTransAll = await unitOfWork.VenderControl.GetTotalLevelInApproveTrans(venderTable.ID, approvemasterid);

                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(venderTable.CREATEBY);
                    var EmployeeId = UserALL.EmployeeId;
                    var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var EmpALL = await EmpRepo.GetAllAsync();
                    var EmpFName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.Name).FirstOrDefault();
                    var EmpLName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.LastName).FirstOrDefault();
                    var EmpFullName = EmpFName + " " + EmpLName;

                    // is final approve ?
                    if (venderTable.CURRENTAPPROVESTEP == approveTransAll.ToList().Count && ApproveResult == 1)
                    {
                        if (venderTable.COMPLETEDATE == null)
                        {
                            venderTable.APPROVESTATUS = 7;
                            venderTable.SENDMAILSUCCESS = 1;
                            venderTable.COMPLETEDATE = DatetimeNow;

                            var emailcomplete = new List<string>();
                            var ApproveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                            var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();
                            /*foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenComplete == 1 && x.IsActive == 1))
                            {
                                emailcomplete.Add(emaillog.Email);
                            }*/
                            emailcomplete.Add(UserALL.Email);

                            var connString = "AXCust";
                            using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection(connString)))
                            {
                                var upax = unitOfWork2.Transaction.Connection.Execute(@"UPDATE VENDTABLE
                                        SET DSG_VendorType  = @a
                                    WHERE ACCOUNTNUM =  @b
                                        AND DATAAREAID = @c",
                                    new
                                    {
                                        @a = 2,
                                        @b = venderTable.VENDCODE_AX,
                                        @c = venderTable.DATAAREAID
                                    },
                                    unitOfWork2.Transaction
                                );
                                unitOfWork2.Complete();
                            }

                            var sendCompleteEmail = _emailService.SendEmail(
                                   //การแจ้งขอ Release ผ่านการอนุมัติเรียบร้อยแล้ว
                                   $"Subject : {venderTable.REQUESTCODE} / {venderTable.VENDCODE_AX} / ดำเนินการ Release เรียบร้อยแล้ว",
                                   $@"
                                     <b>ดำเนินการ Release เรียบร้อยแล้ว</b> <br/><br/>
                                     <b>เลขที่คำขอ   {venderTable.REQUESTCODE} </b> <br/>
                                     <b>รหัสผู้ขาย    {venderTable.VENDCODE_AX} </b> <br/>
                                     <b>ชื่อผู้ขาย    {venderTable.VENDNAME} </b> <br/> <br/><br/><br/>
                                 ",
                                   emailcomplete,
                                   new List<string> { }
                               );

                            if (sendCompleteEmail.Result == false)
                            {
                                throw new Exception(sendCompleteEmail.Message);
                            }
                        }

                        else
                        {
                            var vendcodeax = venderTable.VENDCODE_AX;
                            int isactive = 0;
                            var isactivesub = "";
                            if (venderTable.ISACTIVE == 0)
                            {
                                isactive = 2;
                                isactivesub = "Inactive";
                            }
                            else
                            {
                                isactive = 0;
                                isactivesub = "Active";
                            }
                            //isactive
                            venderTable.APPROVESTATUS = RequestStatusModel.Successfully;
                            venderTable.SENDMAILSUCCESS = 1;
                            venderTable.ISACTIVE = isactive;
                            venderTable.UPDATEDATE = DatetimeNow;
                            var emailcomplete = new List<string>();
                            var ApproveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                            var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();
                            /*foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenComplete == 1 && x.IsActive == 1))
                            {
                                emailcomplete.Add(emaillog.Email);
                            }*/
                            emailcomplete.Add(UserALL.Email);
                            var sendCompleteEmail = _emailService.SendEmail(
                                     $"Subject : {venderTable.REQUESTCODE} / {venderTable.VENDCODE_AX} / ดำเนินการ {isactivesub} เรียบร้อยแล้ว",
                                    $@"
                                     <b>ดำเนินการ {isactivesub} เรียบร้อยแล้ว</b> <br/><br/>
                                     <b>เลขที่คำขอ   {venderTable.REQUESTCODE} </b> <br/>
                                     <b>รหัสผู้ขาย    {venderTable.VENDCODE_AX} </b> <br/>
                                     <b>ชื่อผู้ขาย    {venderTable.VENDNAME} </b> <br/> <br/><br/><br/>
                                 ",
                                    emailcomplete,
                                    new List<string> { }
                                );

                            if (sendCompleteEmail.Result == false)
                            {
                                throw new Exception(sendCompleteEmail.Message);
                            }
                            //
                            //test
                            //update ax
                            /*var upax = unitOfWork.Transaction.Connection.Execute(@"UPDATE [PENTOS\DEVELOP].[AX_Customize].[dbo].[VENDTABLE] 
                                    SET BLOCKED = @a
                                WHERE ACCOUNTNUM =  @b",
                                new
                                {
                                    @a = isactive,
                                    @b = vendcodeax
                                },
                                unitOfWork.Transaction
                            );*/

                            var connString = "AXCust";
                            using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection(connString)))
                            {
                                var upax = unitOfWork2.Transaction.Connection.Execute(@"UPDATE VENDTABLE
                                        SET BLOCKED = @a
                                    WHERE ACCOUNTNUM =  @b
                                        AND DATAAREAID = @c",
                                    new
                                    {
                                        @a = isactive,
                                        @b = vendcodeax,
                                        @c = venderTable.DATAAREAID
                                    },
                                    unitOfWork2.Transaction
                                );
                                unitOfWork2.Complete();
                            }
                            
                        }

                    }

                    //is approve or reject?
                    else if (ApproveResult == 2 && ApproveRemark != null)
                    {
                        venderTable.APPROVESTATUS = RequestStatusModel.Reject;
                        //var fileok = new List<string>();
                        var filePath = $"wwwroot/files/VenderFiles/" + venderTable.REQUESTCODE.Substring(0, 4) + "_" + venderTable.REQUESTCODE.Substring(5, 2) + "_" + venderTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();

                        var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                        var vendLogFileALL = await vendLogFileRepo.GetAllAsync();

                        foreach (var filelog in vendLogFileALL.Where(x => x.REQUESTID == venderTable.ID && x.ISACTIVE == 1))
                        {
                            string fileName = filelog.FILENAME;
                            //fileok.Add($"{filePath}/{fileName}");
                        }

                        var emailreject = new List<string>();
                        var ApproveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                        var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();
                        foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenFailed == 1 && x.IsActive == 1 && x.ApproveLevel < venderTable.CURRENTAPPROVESTEP))
                        {
                            emailreject.Add(emaillog.Email);
                        }
                        emailreject.Add(UserALL.Email);
                        var vendercode = "";
                        if (venderTable.VENDCODE_AX != null)
                        {
                            vendercode = venderTable.VENDCODE_AX;
                        }
                        else
                        {
                            vendercode = venderTable.VENDCODE;
                        }

                        var approveFlowName = ApproveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid && x.ApproveLevel == venderTable.CURRENTAPPROVESTEP && x.IsActive == 1).Select(s => s.Name).FirstOrDefault();
                        var ApproveBy = approveFlowName;

                        var sendRejectEmail = _emailService.SendEmail(
                                $"Subject : {venderTable.REQUESTCODE} / Reject Release / ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ {vendercode}",
                                $@"
                                    <b> ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ {vendercode} </b><br/>
                                    <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                    <b> รหัสผู้ขาย </b> {vendercode}<br/>
                                    <b> ชื่อผู้ขาย  </b> {venderTable.VENDNAME}<br/>
                                    <b> ที่อยู่     </b> {venderTable.ADDRESS}<br/>
                                    <b> ชื่อผู้ติดต่อ </b> {venderTable.CONTACTNAME}<br/>
                                    <b> โทรศัพท์  </b> {venderTable.TEL}
                                    <br/><br/>
                                    <b style='color:red'> สาเหตุการร้องขอ Reject : <b/> <b style='color:black'> {ApproveRemark} </b>
                                    <br/><br/>
                                    <b style='color:black'> Reject By </b> <b style='color:black'>{ApproveBy}<b><br/></br/>
                                 ",
                                emailreject,
                                new List<string> { },
                                "",
                                "",
                                new List<string> { }
                            );

                        if (sendRejectEmail.Result == false)
                        {
                            throw new Exception(sendRejectEmail.Message);
                        }
                    }
                    
                    else
                    {
                        // update head table
                        venderTable.CURRENTAPPROVESTEP += 1;
                        venderTable.APPROVESTATUS = RequestStatusModel.WaitingForApprove;

                        // generate nonce
                        var nonceKey = Guid.NewGuid().ToString();

                        await nonceRepo.InsertAsync(new VenderNonce_TB
                        {
                            NonceKey = nonceKey,
                            CreateDate = DatetimeNow,
                            ExpireDate = DatetimeNow.AddDays(7),
                            IsUsed = 0
                        });

                        //var fileok = new List<string>();
                        var filePath = $"wwwroot/files/VenderFiles/" + venderTable.REQUESTCODE.Substring(0, 4) + "_" + venderTable.REQUESTCODE.Substring(5, 2) + "_" + venderTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();

                        var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                        var vendLogFileALL = await vendLogFileRepo.GetAllAsync();

                        foreach (var filelog in vendLogFileALL.Where(x => x.REQUESTID == venderTable.ID && x.ISACTIVE == 1))
                        {
                            string fileName = filelog.FILENAME;
                            //fileok.Add($"{filePath}/{fileName}");
                        }

                        // next approve trans
                        var nextALL = new GenericRepository<VenderApproveTrans_TB>(unitOfWork.Transaction);
                        var nextAllLevel = await nextALL.GetAllAsync();
                        var nextLevel = nextAllLevel.Where(x => x.REQUESTID == venderTable.ID &&
                                                            x.APPROVELEVEL == venderTable.CURRENTAPPROVESTEP &&
                                                            x.APPROVEMASTERID == approvemasterid &&
                                                            x.ISCURRENTAPPROVE == 1);
                        foreach (var i in nextLevel)
                        {
                            var appoveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                            var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                            var approveFlowName = appoveFlowALL.Where(x => x.ApproveMasterId == venderTable.APPROVEMASTERID &&
                                                                    x.ApproveLevel == venderTable.CURRENTAPPROVESTEP &&
                                                                    x.IsActive == 1 && x.Email == i.EMAIL).Select(s => s.Name).FirstOrDefault();

                            var ApproveBy = approveFlowName;

                            var sendNextEmail = _emailService.SendEmail(
                                   $"Subject : {venderTable.REQUESTCODE} / Release / ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ / {venderTable.VENDCODE_AX}",
                                   $@"
                                    <b> ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ {venderTable.VENDCODE_AX} </b><br/>
                                    <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                    <b> รหัสผู้ขาย </b> {venderTable.VENDCODE_AX}<br/>
                                    <b> ชื่อผู้ขาย  </b> {venderTable.VENDNAME}<br/>
                                    <b> ที่อยู่     </b> {venderTable.ADDRESS}<br/>
                                    <b> ชื่อผู้ติดต่อ </b> {venderTable.CONTACTNAME}<br/>
                                    <b> โทรศัพท์  </b> {venderTable.TEL}<br/>
                                    <br/><br/><br/>
                                    <b> ชื่อผู้ทำการอนุมัติ </b> {ApproveBy}<br/></br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={i.EMAIL}'>คลิกที่นี่</a> <br/>
                                ",
                                    //<b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/ApproveVender_Release?VendID={VendID}&TranID={nextApproveTrans.ID}&nonce={nonceKey}'>คลิกที่นี่</a> <br/>
                                    new List<string> { i.EMAIL },
                                    new List<string> { },
                                    "",
                                    "",
                                    new List<string> { }
                                );

                            if (sendNextEmail.Result == false)
                            {
                                throw new Exception(sendNextEmail.Message);
                            }

                            var approveTrans_next = await approveTransRepo.GetAsync(i.ID);
                            approveTrans_next.SENDEMAILDATE = DatetimeNow;
                            await approveTransRepo.UpdateAsync(approveTrans_next);
                        }
                        
                    }

                    int isapp, isreject;
                    if (ApproveResult == 1)
                    {
                        isapp = 1;
                        isreject = 0;
                    }
                    else
                    {
                        isapp = 0;
                        isreject = 1;
                    }
                    var newLogsApproveRepo = new GenericRepository<VenderLogsApprove_TB>(unitOfWork.Transaction);
                    var newLogsApprove = new VenderLogsApprove_TB
                    {
                        REQUESTID = VendID,
                        EMAIL = email,
                        CREATEDATE = DatetimeNow,
                        ISAPPROVE = isapp,
                        ISREJECT = isreject
                    };
                    var LogsApprove = await newLogsApproveRepo.InsertAsync(newLogsApprove);

                    await venderTableRepo.UpdateAsync(venderTable);
                    await nonceRepo.UpdateAsync(_nonce);



                    unitOfWork.Complete();

                    AlertSuccess = "ดำเนินการเสร็จสิ้น";
                    return Redirect($"/Vender/VenderRequestList?Email={email}");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/");
            }
        }

        public async Task GetData(int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                var vend = await venderTableRepo.GetAsync(id);

                if (vend.VENDCODE_AX == null) { VenderCodeAX = "-"; } else { VenderCodeAX = vend.VENDCODE_AX; }
                RequestCode = vend.REQUESTCODE;
                RequestDate = vend.REQUESTDATE;
                VenderCode = vend.VENDCODE;
                VenderIDNum = vend.VENDIDNUM;
                VenderName = vend.VENDNAME;
                if (vend.DATAAREAID == "dv") { DataAreaId1 = true; } else { DataAreaId1 = false; }
                if (vend.DATAAREAID == "dsc") { DataAreaId2 = true; } else { DataAreaId2 = false; }
                DataAreaId = vend.DATAAREAID;
                Address = vend.ADDRESS;
                ContactName = vend.CONTACTNAME;
                Telephone = vend.TEL;
                Fax = vend.FAX;
                Email = vend.EMAIL;
                Website = vend.WEBSITE;
                Remark = vend.REMARK;
                Currency = vend.CURRENCY;
                isActive = vend.ISACTIVE;
                ProductType = vend.PRODTYPEID;
                Paymterm = vend.PAYMTERMID;
                ProductTypeDetail = vend.PRODTYPEDETAIL;

                var vendGroupRepo = new GenericRepository<VenderGroup_TB>(unitOfWork.Transaction);
                var vendGroupALL = await vendGroupRepo.GetAllAsync();
                var vendTypeRepo = new GenericRepository<VenderType_TB>(unitOfWork.Transaction);
                var vendTypeALL = await vendTypeRepo.GetAllAsync();

                VenderGroup = vendGroupALL.Where(x => x.ID == vend.VENDGROUPID).Select(s => s.DESCRIPTION).FirstOrDefault();
                VenderType = vendTypeALL.Where(x => x.ID == vend.VENDTYPEID).Select(s => s.DESCRIPTION).FirstOrDefault();

                var vendDocRepo = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);
                var vendDocALL = await vendDocRepo.GetAllAsync();
                foreach (var docLog in vendDocALL.Where(x => x.REQUESTID == id && x.ISACTIVE == 1))
                {
                    if (docLog.DOCID == 1) { DocRef1 = true; }
                    if (docLog.DOCID == 2) { DocRef2 = true; }
                    if (docLog.DOCID == 3) { DocRef3 = true; }
                    if (docLog.DOCID == 4) { DocRef4 = true; }
                    if (docLog.DOCID == 5) { DocRef5 = true; }
                    if (docLog.DOCID == 6) { DocRef6 = true; DocRef6_Desc = docLog.REMARK; }
                    if (docLog.DOCID == 7) { DocRef7 = true; }
                    if (docLog.DOCID == 8) { DocRef8 = true; }
                    if (docLog.DOCID == 9) { DocRef9 = true; DocRef9_Desc = docLog.REMARK; }
                    if (docLog.DOCID == 10) { DocRef10 = true; }
                    if (docLog.DOCID == 11) { DocRef11 = true; }
                    if (docLog.DOCID == 12) { DocRef12 = true; }
                    if (docLog.DOCID == 13) { DocRef13 = true; }
                    if (docLog.DOCID == 14) { DocRef14 = true; }
                    if (docLog.DOCID == 15) { DocRef15 = docLog.REMARK; }
                    if (docLog.DOCID == 16) { DocRef16 = true; DocRef16_Desc = docLog.REMARK; }
                    if (docLog.DOCID == 17) { DocRef17 = true; }
                    if (docLog.DOCID == 18) { DocRef18 = true; }
                    if (docLog.DOCID == 19) { DocRef19 = true; }
                    if (docLog.DOCID == 20) { DocRef20 = true; }
                    if (docLog.DOCID == 21) { DocRef21 = true; }
                    if (docLog.DOCID == 22) { DocRef22 = true; }
                    if (docLog.DOCID == 23) { DocRef23 = true; }

                }

                unitOfWork.Complete();
            }
        }

        public async Task<IActionResult> OnPostGridAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<LogFileGridViewModel>($@"
                        SELECT ID,
		                    REQUESTID,
		                    FILENAME,
		                    UPLOADDATE,
		                    ISACTIVE,
		                    CREATEBY
                    FROM TB_VenderLogFile 
                    WHERE (REQUESTID = {id} AND ISACTIVE = 1) OR (REQUESTID = {id} AND ISTEMP = 1)
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

        public async Task<IActionResult> OnPostGridApproveAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<LogApproveGridViewModel>($@"
                        SELECT *
                        FROM
                        (
                        SELECT VT.ID,VT.REQUESTID,
	                        VT.APPROVEMASTERID,VT.EMAIL,
	                        VT.APPROVELEVEL,
	                        CONVERT(VARCHAR,VT.APPROVEDATE,103)+' '+
	                        CONVERT(VARCHAR,VT.APPROVEDATE,108) APPROVEDATE,
	                        VG.DESCRIPTION,AF.Name
	                        --,ROW_NUMBER() OVER(PARTITION BY VT.REQUESTID,VT.APPROVEMASTERID,VT.APPROVELEVEL ORDER BY VT.ID DESC) AS Row
	                        FROM TB_VenderApproveTrans VT JOIN
	                        TB_VenderApproveMapping VM ON VT.APPROVEMASTERID = VM.APPROVEMASTERID JOIN
	                        TB_VenderApproveGroup VG ON VM.APPROVEGROUPID = VG.ID JOIN
	                        TB_ApproveFlow AF ON VT.APPROVEMASTERID = AF.ApproveMasterId AND 
	                        VT.EMAIL = AF.Email AND VT.APPROVELEVEL = AF.ApproveLevel
	                        WHERE  VT.REQUESTID = {id} AND VT.APPROVEDATE IS NOT NULL 
	                        AND VT.ISCURRENTAPPROVE =1 
	                        GROUP BY VT.ID,VT.REQUESTID,
	                        VT.APPROVEMASTERID,VT.EMAIL,
	                        VT.APPROVELEVEL,
	                        CONVERT(VARCHAR,VT.APPROVEDATE,103)+' '+
	                        CONVERT(VARCHAR,VT.APPROVEDATE,108) ,
	                        VG.DESCRIPTION,AF.Name
                        )T
                        ORDER BY T.APPROVEDATE
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

        public async Task<IActionResult> OnGetViewFileUploadAsync(int id, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                    var vendLogFile = await vendLogFileRepo.GetAsync(Fileid);

                    var vendTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var vendTable = await vendTableRepo.GetAsync(id);

                    var filePath = $"wwwroot/files/VenderFiles/" + vendTable.REQUESTCODE.Substring(0, 4) + "_" + vendTable.REQUESTCODE.Substring(5, 2) + "_" + vendTable.REQUESTCODE.Substring(8, 5);
                    var fileName = vendLogFile.FILENAME;


                    var basePath = $"{filePath}/{fileName}";
                    if (!System.IO.File.Exists(basePath))
                    {
                        throw new Exception("File not found.");
                    }

                    byte[] fileBytes = System.IO.File.ReadAllBytes(basePath);

                    return File(fileBytes, "application/x-msdownload", fileName);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
