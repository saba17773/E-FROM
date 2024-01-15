using System;
using System.Collections.Generic;
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
    public class isActiveModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public string VenderIDNum { get; set; }

        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public string VenderCode { get; set; }

        [BindProperty]
        public string VenderCodeAX { get; set; }

        [BindProperty]
        public string VenderName { get; set; }

        [BindProperty]
        public string ContactName { get; set; }

        [BindProperty]
        public string Telephone { get; set; }

        [BindProperty]
        public string Fax { get; set; }

        [BindProperty]
        public string Website { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string VenderGroup { get; set; }

        [BindProperty]
        public string VenderType { get; set; }

        [BindProperty]
        public string Currency { get; set; }

        [BindProperty]
        public string ProductType { get; set; }

        [BindProperty]
        public string Paymterm { get; set; }

        [BindProperty]
        public DateTime? RequestDate { get; set; }

        [BindProperty]
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
        public int UpdateBy { get; set; }

        [BindProperty]
        public int isActive { get; set; }

        [BindProperty]
        public int RequestID { get; set; }
        [BindProperty]
        public string ProductTypeDetail { get; set; }

        [BindProperty]
        public string DataAreaId { get; set; }
        [BindProperty]
        public bool DataAreaId1 { get; set; }
        [BindProperty]
        public bool DataAreaId2 { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IHelperService _helperService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public isActiveModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IHelperService helperService,
          IEmailService emailService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatableService;
            _authService = authService;
            _helperService = helperService;
            _emailService = emailService;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {

            try
            {
                await _authService.CanAccess(nameof(VenderPermissionModel.MANAGE_VENDER));
                
                RequestID = id;
                await GetData(id);

                return Page();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
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
                /*var vendProdRepo = new GenericRepository<VenderProductType_TB>(unitOfWork.Transaction);
                var vendProdALL = await vendProdRepo.GetAllAsync();*/


                VenderGroup = vendGroupALL.Where(x => x.ID == vend.VENDGROUPID).Select(s => s.DESCRIPTION).FirstOrDefault();
                VenderType = vendTypeALL.Where(x => x.ID == vend.VENDTYPEID).Select(s => s.DESCRIPTION).FirstOrDefault();
                //ProductType = vendProdALL.Where(x => x.CODE == vend.PRODTYPEID).Select(s => s.CODE).FirstOrDefault();

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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                if (Remark == null)
                {
                    if (isActive == 0)
                    {
                        AlertError = "กรุณาใส่สาเหตุที่ต้องการระงับผู้จำหน่ายในระบบ";
                    }
                    else
                    {
                        AlertError = "กรุณาใส่สาเหตุที่ต้องการยกเลิกการระงับผู้จำหน่าย";
                    }
                    return Redirect($"/Vender/{id}/isActive");
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var nonceRepo = new GenericRepository<VenderNonce_TB>(unitOfWork.Transaction);

                    var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var venderTable = await venderTableRepo.GetAsync(id);

                    var UpdateDate = DateTime.Now;

                    //throw new Exception(isActive.ToString());
                    //update vender table
                    var process = "";
                    if (isActive == 0)
                    {
                        process = "InActive";
                    }
                    else
                    {
                        process = "Active";
                    }

                    venderTable.CURRENTAPPROVESTEP = 1;
                    venderTable.APPROVESTATUS = 3;
                    venderTable.UPDATEBY = UpdateBy;
                    venderTable.UPDATEDATE = UpdateDate;
                    venderTable.SENDMAILSUCCESS = 0;
                    venderTable.REMARK = Remark;

                    int approvemasterid = venderTable.APPROVEMASTERID;

                    //update iscurrent = 0
                    var VendTranRepo2 = new GenericRepository<VenderApproveTrans_TB>(unitOfWork.Transaction);

                    var VenderTrnasByRequestID = await unitOfWork.VenderControl.GetApproveTranIsCurrentByRequestID(id, approvemasterid);
                    var VenderTransFilter = VenderTrnasByRequestID.Select(s => s.ID);

                    foreach (var transfilter in VenderTransFilter)
                    {
                        var approveTrans_iscurrent = await VendTranRepo2.GetAsync(transfilter);
                        approveTrans_iscurrent.ISCURRENTAPPROVE = 0;
                        await VendTranRepo2.UpdateAsync(approveTrans_iscurrent);
                    }

                    //insert approve trans
                    var appoveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                    var appoveFlowALL = await appoveFlowRepo.GetAllAsync();

                    var approveFlow_data = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid && x.ApproveLevel != 0 && x.IsActive == 1).OrderBy(o => o.ApproveLevel);

                    await venderTableRepo.UpdateAsync(venderTable);

                    var VendTranRepo = new GenericRepository<VenderApproveTrans_TB>(unitOfWork.Transaction);
                    foreach (var item in approveFlow_data)
                    {
                        await VendTranRepo.InsertAsync(new VenderApproveTrans_TB
                        {
                            EMAIL = item.Email,
                            APPROVELEVEL = item.ApproveLevel,
                            APPROVEMASTERID = item.ApproveMasterId,
                            REQUESTID = (int)id,
                            ISCURRENTAPPROVE = 1,
                            PROCESS = process,
                            ISALERT = 0

                        });
                    }

                    // update approve trans
                    //var approveTransByRequestID = await unitOfWork.VenderControl.GetApproveTransByRequestID((int)id, approvemasterid);
                    //var approveTransLevel1 = approveTransByRequestID.Where(x => x.APPROVELEVEL == 1).FirstOrDefault();
                    //var approveTrans = await VendTranRepo.GetAsync(approveTransLevel1.ID);
                    //approveTrans.SENDEMAILDATE = UpdateDate;
                    //await VendTranRepo.UpdateAsync(approveTrans);


                    // attach file
               
                    //var fileok = new List<string>();
                    var filePath = $"wwwroot/files/VenderFiles/" + venderTable.REQUESTCODE.Substring(0, 4) + "_" + venderTable.REQUESTCODE.Substring(5, 2) + "_" + venderTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();

                    var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                    var vendLogFileALL = await vendLogFileRepo.GetAllAsync();

                    foreach (var filelog in vendLogFileALL.Where(x => (x.REQUESTID == venderTable.ID && x.ISACTIVE == 1) || (x.REQUESTID == venderTable.ID && x.ISTEMP == 1)))
                    {
                        string fileName = filelog.FILENAME;
                       // fileok.Add($"{filePath}/{fileName}");
                    }

                    // generate nonce
                    var nonceKey = Guid.NewGuid().ToString();

                    await nonceRepo.InsertAsync(new VenderNonce_TB
                    {
                        NonceKey = nonceKey,
                        CreateDate = UpdateDate,
                        ExpireDate = UpdateDate.AddDays(7),
                        IsUsed = 0
                    });

                    var subject = "";
                    var body = "";
                    if (isActive == 0)
                    {
                        subject += "/ Inactive / ร้องขอเพื่อระงับผู้จำหน่ายในระบบ / ";
                        body += "ร้องขอเพื่อระงับผู้จำหน่ายในระบบ";
                    }
                    else
                    {
                        subject += "/ Active / ร้องขอเพื่อยกเลิกการระงับผู้จำหน่าย / ";
                        body += "ร้องขอเพื่อยกเลิกการระงับผู้จำหน่าย";
                    }
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(UpdateBy);
                    var EmployeeId = UserALL.EmployeeId;
                    var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var EmpALL = await EmpRepo.GetAllAsync();
                    var EmpFName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.Name).FirstOrDefault();
                    var EmpLName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.LastName).FirstOrDefault();
                    var EmpFullName = EmpFName + " " + EmpLName;

                    // update approve trans
                    var approveTransByRequestID = await unitOfWork.VenderControl.GetApproveTransByRequestID((int)id, approvemasterid);
                    var approveTransLevel1 = approveTransByRequestID.Where(x => x.APPROVELEVEL == 1);
                    foreach (var i in approveTransLevel1)
                    {
                        var approveFlowName = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid && 
                                                                x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                x.Email == i.EMAIL).Select(s => s.Name).FirstOrDefault();

                        var ApproveBy = approveFlowName;

                        var approveTransAll = await VendTranRepo.GetAllAsync();
                        var approveTrans = approveTransAll.Where(x=>x.ID == i.ID).FirstOrDefault();
                        approveTrans.SENDEMAILDATE = UpdateDate;
                        await VendTranRepo.UpdateAsync(approveTrans);

                        var sendRevisemail = _emailService.SendEmail(
                                $"Subject : {venderTable.REQUESTCODE} {subject} {venderTable.VENDCODE_AX} ",
                                $@"
                                    <b>{body} {venderTable.VENDCODE_AX} </b> <br/>
                                    <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                    <b> รหัสผู้ขาย </b> {venderTable.VENDCODE_AX}<br/>
                                    <b> ชื่อผู้ขาย  </b> {venderTable.VENDNAME}<br/>
                                    <b> ที่อยู่     </b> {venderTable.ADDRESS}<br/>
                                    <b> ชื่อผู้ติดต่อ </b> {venderTable.CONTACTNAME}<br/>
                                    <b> โทรศัพท์  </b> {venderTable.TEL}<br/>
                                    <br/><br/><br/>
                                    <b style='color:red'>สาเหตุที่{body}  </b> {venderTable.REMARK}<br/>
                                    <br/><br/><br/>
                                    <b> ชื่อผู้ทำการอนุมัติ </b> {ApproveBy}<br/></br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={i.EMAIL}'>คลิกที่นี่</a> <br/>
                                    ",
                                //<b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/ApproveVender_isActive?VendID={id}&TranID={approveTransLevel1.ID}&nonce={nonceKey}'>คลิกที่นี่</a> <br/>
                                new List<string> { i.EMAIL },
                                new List<string> { },
                                "",
                                "",
                                new List<string> { }
                            );

                        if (sendRevisemail.Result == false)
                        {
                            throw new Exception(sendRevisemail.Message);
                        }

                    }

                    unitOfWork.Complete();

                    AlertSuccess = "ดำเนินการเสร็จสิ้น";
                    return Redirect($"/Vender");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/");
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

                    return new JsonResult(_datatablesService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
