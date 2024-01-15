using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.UI.Contexts;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using System.Linq;
using Dapper;
using Web.UI.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.IO;

namespace Web.UI.Pages.Vender
{
    public class ReleaseVenderModel : PageModel
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
        public DateTime? RequestDate { get; set; }

        [BindProperty]
        public string Paymterm { get; set; }

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
        public IEnumerable<IFormFile> UploadFile_1 { get; set; }

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

        public ReleaseVenderModel(
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
                Paymterm = vend.PAYMTERMID;
                ProductTypeDetail = vend.PRODTYPEDETAIL;

                var vendGroupRepo = new GenericRepository<VenderGroup_TB>(unitOfWork.Transaction);
                var vendGroupALL = await vendGroupRepo.GetAllAsync();
                var vendTypeRepo = new GenericRepository<VenderType_TB>(unitOfWork.Transaction);
                var vendTypeALL = await vendTypeRepo.GetAllAsync();
                var vendProdRepo = new GenericRepository<VenderProductType_TB>(unitOfWork.Transaction);
                var vendProdALL = await vendProdRepo.GetAllAsync();


                VenderGroup = vendGroupALL.Where(x => x.ID == vend.VENDGROUPID).Select(s => s.DESCRIPTION).FirstOrDefault();
                VenderType = vendTypeALL.Where(x => x.ID == vend.VENDTYPEID).Select(s => s.DESCRIPTION).FirstOrDefault();
                ProductType = vendProdALL.Where(x => x.CODE == vend.PRODTYPEID).Select(s => s.DESCRIPTION).FirstOrDefault();

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

                    return new JsonResult(_datatablesService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var VendTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var VendTranRepo = new GenericRepository<VenderApproveTrans_TB>(unitOfWork.Transaction);
                    var VendLogDocRepo = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);
                    var VendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                    var nonceRepo = new GenericRepository<VenderNonce_TB>(unitOfWork.Transaction);

                    var VendTableALL = await VendTableRepo.GetAllAsync();
                    var VendTranALL = await VendTranRepo.GetAllAsync();
                    var VendLogDocALL = await VendLogDocRepo.GetAllAsync();

                    var DatetimeNow = DateTime.Now;

                    foreach (var VendTB in VendTableALL.Where(x => x.ID == id))
                    {
                        var approveMapRepo = new GenericRepository<VenderApproveMapping_TB>(unitOfWork.Transaction);
                        var approveMapALL = await approveMapRepo.GetAllAsync();
                        var approvemasterid = approveMapALL.Where(x => x.CreateBy == VendTB.CREATEBY && x.STEP == 2 && x.DATAAREAID == DataAreaId).Select(s => s.APPROVEMASTERID).FirstOrDefault();
                        var vendprocessid = approveMapALL.Where(x => x.CreateBy == VendTB.CREATEBY && x.STEP == 2 && x.DATAAREAID == DataAreaId).Select(s => s.APPROVEGROUPID).FirstOrDefault();

                        var appoveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                        var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                        var approveFlow_data = appoveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ApproveLevel != 0 && x.IsActive == 1).OrderBy(o => o.ApproveLevel);

                        var VendTB_Update = await VendTableRepo.GetAsync((int)VendTB.ID);
                        
                        VendTB_Update.VENDPROCESSID = vendprocessid;
                        VendTB_Update.APPROVESTATUS = 3;
                        VendTB_Update.SENDMAILSUCCESS = 0;
                        VendTB_Update.CURRENTAPPROVESTEP = 1;
                        VendTB_Update.APPROVEMASTERID = approvemasterid;
                        VendTB_Update.UPDATEBY = CreateBy;
                        VendTB_Update.UPDATEDATE = DatetimeNow;
                        await VendTableRepo.UpdateAsync(VendTB_Update);


                        var nonceKey = Guid.NewGuid().ToString();

                        await nonceRepo.InsertAsync(new VenderNonce_TB
                        {
                            NonceKey = nonceKey,
                            CreateDate = DatetimeNow,
                            ExpireDate = DatetimeNow.AddDays(7),
                            IsUsed = 0
                        });

                        // insert approve transaction
                        foreach (var item in approveFlow_data)
                        {

                            await VendTranRepo.InsertAsync(new VenderApproveTrans_TB
                            {
                                EMAIL = item.Email,
                                APPROVELEVEL = item.ApproveLevel,
                                APPROVEMASTERID = item.ApproveMasterId,
                                REQUESTID = VendTB.ID,
                                ISCURRENTAPPROVE = 1,
                                ISALERT = 0,
                                ISSKIPALERT = item.IsSkipAlert
                            });
                        }

                        string basePath = $"wwwroot/files/VenderFiles/" + VendTB.REQUESTCODE.Substring(0, 4) + "_" + VendTB.REQUESTCODE.Substring(5, 2) + "_" + VendTB.REQUESTCODE.Substring(8, 5);
                        //var fileok = new List<string>();
                        var filePath2 = Path.GetTempFileName();
                       // var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                        if (UploadFile_1 != null)
                        {
                            foreach (IFormFile postedFile in UploadFile_1)
                            {
                                string fileName = Path.GetFileName(postedFile.FileName);
                                using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                                {
                                    await postedFile.CopyToAsync(stream);
                                    await VendLogFileRepo.InsertAsync(new VenderLogFile_TB
                                    {
                                        REQUESTID = VendTB.ID,
                                        FILENAME = fileName,
                                        UPLOADDATE = DatetimeNow,
                                        CREATEBY = CreateBy,
                                        ISACTIVE = 1
                                    });
                                }
                            }
                        }

                      
                        var vendLogFileALL = await VendLogFileRepo.GetAllAsync();

                        foreach (var filelog in vendLogFileALL.Where(x => x.REQUESTID == VendTB.ID && x.ISACTIVE == 1))
                        {
                            string fileName = filelog.FILENAME;
                            //fileok.Add($"{basePath}/{fileName}");
                        }
                        var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                        var UserALL = await UserRepo.GetAsync(CreateBy);
                        var EmployeeId = UserALL.EmployeeId;
                        var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                        var EmpALL = await EmpRepo.GetAllAsync();
                        var EmpFName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.Name).FirstOrDefault();
                        var EmpLName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.LastName).FirstOrDefault();
                        var EmpFullName = EmpFName + " " + EmpLName;

                        
                        var approveTransByRequestCode = await unitOfWork.VenderControl.GetApproveTransByRequestID((int)VendTB.ID, approvemasterid);
                        var approveTransLevel1 = approveTransByRequestCode.Where(x => x.APPROVELEVEL == 1);
                        foreach (var i in approveTransLevel1)
                        {
                            var approveFlowName = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid && 
                                                                      x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                      x.Email == i.EMAIL).Select(s => s.Name).FirstOrDefault();

                            var ApproveBy = approveFlowName;

                            var approveTransAll = await VendTranRepo.GetAllAsync();
                            var approveTrans = approveTransAll.Where(x => x.ID == i.ID).FirstOrDefault();
                            approveTrans.SENDEMAILDATE = DatetimeNow;
                            await VendTranRepo.UpdateAsync(approveTrans);

                            var sendEmail = _emailService.SendEmail(
                                $"Subject : {RequestCode} / Release / ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ / {VendTB.VENDCODE_AX}",
                                $@"
                                    <b> ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ {VendTB.VENDCODE_AX} </b><br/>
                                    <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                    <b> รหัสผู้ขาย </b> {VenderCodeAX}<br/>
                                    <b> ชื่อผู้ขาย  </b> {VenderName}<br/>
                                    <b> ที่อยู่     </b> {Address}<br/>
                                    <b> ชื่อผู้ติดต่อ </b> {ContactName}<br/>
                                    <b> โทรศัพท์  </b> {Telephone}<br/>
                                    <br/><br/><br/>
                                    <b> ชื่อผู้ทำการอนุมัติ </b> {ApproveBy}<br/></br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={i.EMAIL}'>คลิกที่นี่</a> <br/>
                                ",
                                 //<b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/ApproveVender_Release?VendID={VendTB.ID}&TranID={approveTransLevel1.ID}&nonce={nonceKey}'>คลิกที่นี่</a> <br/>
                                 new List<string> { i.EMAIL },
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
                    }

                    
                    unitOfWork.Complete();
                    
                    AlertSuccess = "Release Complete";
                    return Redirect($"/Vender");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Vender");
            }
        }

    }
}
