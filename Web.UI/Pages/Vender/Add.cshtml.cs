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

namespace Web.UI.Pages.Vender
{
    public class AddModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        [Required]
        [StringLength(20)]
        [MaxLength(13)]
        public string VenderIDNum { get; set; }

        [BindProperty]
        [Required]
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

       // [Required]
        [BindProperty]
        public int VenderGroup { get; set; }

        [BindProperty]
        public int VenderType { get; set; }

        [BindProperty]
        [StringLength(10)]
        public string Currency { get; set; }

       // [Required]
        [BindProperty]
        public string ProductType { get; set; }

        [BindProperty]
        public string ProductTypeDetail { get; set; }

        [BindProperty]
        public string Paymterm { get; set; }

        [BindProperty]
        [StringLength(250)]
        public string Remark { get; set; }

        public List<SelectListItem> VenderGroupMaster { get; set; }
        public List<SelectListItem> VenderTypeMaster { get; set; }
        public List<SelectListItem> CurrencyMaster { get; set; }
        public List<SelectListItem> ProductTypeMaster { get; set; }
        public List<SelectListItem> PaymtermMaster { get; set; }
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
        public string CreateEmployeeId { get; set; }

        [BindProperty]
        public IEnumerable<IFormFile> UploadFile_1 { get; set; }

        [BindProperty]
        [Required]
        public string DataAreaId { get; set; }

        [BindProperty]
        public int CheckISDV { get; set; }

        [BindProperty]
        public int CheckISDSC { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IHelperService _helperService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public AddModel(
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
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(VenderPermissionModel.ADD_VENDER));

                VenderGroupMaster = await GetVenderGroupMaster();
                ProductTypeMaster = await GetProductTypeMaster();
                VenderTypeMaster = await GetVenderTypeMaster();
                CurrencyMaster = await GetCurrencyMaster();
                PaymtermMaster = await GetPaymtermMaster();

                await CheckDataAreaID();

                return Page();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task CheckDataAreaID()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var approveMapRepo = new GenericRepository<VenderApproveMapping_TB>(unitOfWork.Transaction);
                var approveMapALL = await approveMapRepo.GetAllAsync();
                //CheckDataAreaID
                var createby1 = _authService.GetClaim().UserId;
                var approveMapByCreateBy = approveMapALL.Where(x => x.CreateBy == createby1 &&
                                                                x.STEP == 1).Select(s => s.DATAAREAID);
                CheckISDSC = 0;
                CheckISDV = 0;
                foreach (var i in approveMapByCreateBy)
                {
                    if (i == "dv")
                    {
                        CheckISDV = 1;
                    }
                    if (i == "dsc")
                    {
                        CheckISDSC = 1;
                    }
                }
                unitOfWork.Complete();
            }
        }
        public async Task<List<SelectListItem>> GetVenderGroupMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var vendergroupRepo = new GenericRepository<VenderGroup_TB>(unitOfWork.Transaction);

                var vendergroupAll = await vendergroupRepo.GetAllAsync();

                return vendergroupAll
                    .Where(x => x.ISACTIVE == 1)
                    //.OrderBy(x1 => x1.LastOption).ThenBy(x2 => x2.ID)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DESCRIPTION
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetVenderTypeMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var vendertypeRepo = new GenericRepository<VenderType_TB>(unitOfWork.Transaction);

                var vendertypeAll = await vendertypeRepo.GetAllAsync();

                return vendertypeAll
                    .Where(x => x.ISACTIVE == 1)
                    //.OrderBy(x1 => x1.LastOption).ThenBy(x2 => x2.ID)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DESCRIPTION
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetCurrencyMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var currencyRepo = new GenericRepository<VenderCurrency_TB>(unitOfWork.Transaction);

                var currencyeAll = await currencyRepo.GetAllAsync();

                return currencyeAll
                    .Where(x => x.ISACTIVE == 1)
                    //.OrderBy(x1 => x1.LastOption).ThenBy(x2 => x2.ID)
                    .Select(x => new SelectListItem
                    {
                        Value = x.CODE,
                        Text = x.CODE
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetProductTypeMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var producttypeRepo = new GenericRepository<VenderProductType_TB>(unitOfWork.Transaction);

                var producttypeAll = await producttypeRepo.GetAllAsync();

                return producttypeAll
                    .Where(x => x.ISACTIVE == 1)
                    //.OrderBy(x1 => x1.LastOption).ThenBy(x2 => x2.ID)
                    .Select(x => new SelectListItem
                    {
                        Value = x.CODE.ToString(),
                        Text = x.CODE
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetPaymtermMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var paymtermRepo = new GenericRepository<VenderPaymtermMaster_TB>(unitOfWork.Transaction);

                var paymtermAll = await paymtermRepo.GetAllAsync();

                return paymtermAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.PAYMTERMID,
                        Text = x.PAYMTERMID
                        /* + "  " + x.DESCRIPTION*/
                    })
                    .ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                VenderGroupMaster = await GetVenderGroupMaster();
                ProductTypeMaster = await GetProductTypeMaster();
                VenderTypeMaster = await GetVenderTypeMaster();
                CurrencyMaster = await GetCurrencyMaster();
                PaymtermMaster = await GetPaymtermMaster();

                return Page();
            }

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
                    var VendLogDocALL = await VendLogDocRepo.GetAllAsync();

                    var YearNow = @DateTime.Now.ToString("yyyy", new CultureInfo("en-US"));
                    var MonthNow = @DateTime.Now.ToString("MM", new CultureInfo("en-US"));

                    var PreReqCode = YearNow + "/" + MonthNow + "/";
                    var RequestCode = "";
                    var RequestCode_pathfile = "";
                    var chkYear = VendTableALL.Where(x => x.REQUESTCODE.Substring(0, 4) == YearNow && x.REQUESTCODE.Substring(5, 2) == MonthNow).Max(a => a.REQUESTCODE.Substring(8, 5));

                    var VendCode = VendTableALL.Max(x => x.VENDCODE.Substring(3, 5));
                    int reqCode_int;
                    var reqCode = "";

                    //autorun vendercode
                    if (VendCode == null)
                    {
                        VendCode = "VD-00001";
                    }
                    else
                    {
                        int vdcode = Int32.Parse(VendCode) + 1;
                        VendCode = "VD-" + vdcode.ToString().PadLeft(5, '0');
                    }
                    /*throw new Exception(VendCode);*/

                    //gen requestcode
                    if (chkYear == null)
                    {
                        RequestCode = PreReqCode + "00001";
                        RequestCode_pathfile = YearNow + "_" + MonthNow + "_00001";
                    }
                    else
                    {
                        reqCode = VendTableALL.Max(a => a.REQUESTCODE.Substring(8, 5));
                        reqCode_int = Int32.Parse(chkYear) + 1;
                        RequestCode = PreReqCode + reqCode_int.ToString().PadLeft(5, '0');
                        RequestCode_pathfile = YearNow + "_" + MonthNow +"_"+ reqCode_int.ToString().PadLeft(5, '0');
                    }

                    int doc1, doc2, doc3, doc4, doc5, doc6, doc7, doc8, doc9, doc10,
                        doc11, doc12, doc13, doc14, doc15, doc16, doc17, doc18, doc19, 
                        doc20, doc21, doc22, doc23;

                    if (DocRef1 == true) { doc1 = 1; } else { doc1 = 0; }
                    if (DocRef2 == true) { doc2 = 2; } else { doc2 = 0; }
                    if (DocRef3 == true) { doc3 = 3; } else { doc3 = 0; }
                    if (DocRef4 == true) { doc4 = 4; } else { doc4 = 0; }
                    if (DocRef5 == true) { doc5 = 5; } else { doc5 = 0; }
                    if (DocRef7 == true) { doc7 = 7; } else { doc7 = 0; }
                    if (DocRef8 == true) { doc8 = 8; } else { doc8 = 0; }
                    if (DocRef10 == true) { doc10 = 10; } else { doc10 = 0; }
                    if (DocRef11 == true) { doc11 = 11; } else { doc11 = 0; }
                    if (DocRef12 == true) { doc12 = 12; } else { doc12 = 0; }
                    if (DocRef13 == true) { doc13 = 13; } else { doc13 = 0; }
                    if (DocRef14 == true) { doc14 = 14; } else { doc14 = 0; }
                    if (DocRef15 != null) { doc15 = 15; } else { doc15 = 0; }
                    if (DocRef17 == true) { doc17 = 17; } else { doc17 = 0; }
                    if (DocRef18 == true) { doc18 = 18; } else { doc18 = 0; }
                    if (DocRef19 == true) { doc19 = 19; } else { doc19 = 0; }
                    if (DocRef20 == true) { doc20 = 20; } else { doc20 = 0; }
                    if (DocRef21 == true) { doc21 = 21; } else { doc21 = 0; }
                    if (DocRef22 == true) { doc22 = 22; } else { doc22 = 0; }
                    if (DocRef23 == true) { doc23 = 23; } else { doc23 = 0; }

                    if (DocRef6.ToString() == "True")
                    {
                        if (DocRef6_Desc == null)
                        {
                            throw new Exception("ใส่เงื่อนไขการชำระเงินก่อน");
                        }
                        else
                        {
                            doc6 = 6;
                            DocRef6_Desc = DocRef6_Desc.ToString();
                        }

                    }
                    else
                    {
                        doc6 = 0;
                        DocRef6_Desc = "-";
                    }

                    if (DocRef9.ToString() == "True")
                    {
                        if (DocRef9_Desc == null)
                        {
                            throw new Exception("ใส่เงื่อนไขการชำระเงินก่อน");
                        }
                        else
                        {
                            doc9 = 9;
                            DocRef9_Desc = DocRef9_Desc.ToString();
                        }

                    }
                    else
                    {
                        doc9 = 0;
                        DocRef9_Desc = "-";
                    }

                    if (DocRef16.ToString() == "True")
                    {
                        if (DocRef16_Desc == null)
                        {
                            throw new Exception("ใส่เงื่อนไขการชำระเงินก่อน");
                        }
                        else
                        {
                            doc16 = 16;
                            DocRef16_Desc = DocRef16_Desc.ToString();
                        }

                    }
                    else
                    {
                        doc16 = 0;
                        DocRef16_Desc = "-";
                    }

                    int[] doc_all = new int[23] { doc1, doc2, doc3, doc4, doc5, doc6, doc7, doc8, doc9, doc10,
                                                  doc11, doc12, doc13, doc14, doc15, doc16, doc17, doc18, doc19,
                                                  doc20, doc21, doc22, doc23};

                    var RequestDate = DateTime.Now; 

                    var provinRepo = new GenericRepository<ProvincesMaster_TB>(unitOfWork.Transaction);
                    var provinALL = await provinRepo.GetAllAsync();
                    var distRepo = new GenericRepository<DistrictsMaster_TB>(unitOfWork.Transaction);
                    var distALL = await distRepo.GetAllAsync();
                    var subdistRepo = new GenericRepository<SubDistrictTable>(unitOfWork.Transaction);
                    var subdistALL = await subdistRepo.GetAllAsync();
                    var docrefRepo = new GenericRepository<VenderDocumentRef_TB>(unitOfWork.Transaction);
                    var docrefALL = await docrefRepo.GetAllAsync();

                    //approveid get from user login
                    var approveMapRepo = new GenericRepository<VenderApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approvemasterid = approveMapALL.Where(x => x.CreateBy == CreateBy && x.STEP == 1 && x.DATAAREAID == DataAreaId).Select(s => s.APPROVEMASTERID).FirstOrDefault();
                    var vendprocessid = approveMapALL.Where(x => x.CreateBy == CreateBy && x.STEP == 1 && x.DATAAREAID == DataAreaId).Select(s => s.APPROVEGROUPID).FirstOrDefault();
                    //var approvemasterid = approveMapALL.Where(x => x.CreateBy == CreateBy && x.STEP == 1).Select(s=>s.APPROVEMASTERID).FirstOrDefault();
                    //var vendprocessid = approveMapALL.Where(x => x.CreateBy == CreateBy && x.STEP == 1).Select(s => s.APPROVEGROUPID).FirstOrDefault();


                    /*throw new Exception(approvemasterid.ToString());*/
                    /*int approvemasterid = 10;*/
                    /*throw new Exception("Test");*/
                    var appoveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);
                    var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                    var approveFlow_data = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid && x.ApproveLevel != 0 && x.IsActive == 1).OrderBy(o=>o.ApproveLevel);

                    //insert VenderTable
                    var newVender = new VenderTable_TB
                    {
                        REQUESTCODE = RequestCode,
                        REQUESTDATE = RequestDate,
                        VENDCODE = VendCode,
                        VENDIDNUM = VenderIDNum,
                        VENDNAME = VenderName,
                        ADDRESS = Address,
                        CONTACTNAME = ContactName,
                        TEL = Telephone,
                        FAX = Fax,
                        WEBSITE = Website,
                        EMAIL = Email,
                        VENDGROUPID = VenderGroup,
                        VENDTYPEID = VenderType,
                        CURRENCY = Currency,
                        PRODTYPEID = ProductType,
                        PAYMTERMID = Paymterm,
                        REMARK = Remark,
                        VENDPROCESSID = vendprocessid,
                        APPROVESTATUS = 3,
                        CURRENTAPPROVESTEP = 1,
                        /* COMPLETEBY = null,
                         COMPLETEDATE = null,*/
                        CREATEBY = CreateBy,
                        CREATEDATE = RequestDate,
                        UPDATEBY = CreateBy,
                        UPDATEDATE = RequestDate,
                        APPROVEMASTERID = approvemasterid,
                        ISACTIVE = 0,
                        SENDMAILSUCCESS = 0,
                        ISREVISE = 0,
                        PRODTYPEDETAIL = ProductTypeDetail,
                        DATAAREAID = DataAreaId
                    };
                    var VendID = await VendTableRepo.InsertAsync(newVender);

                    // insert approve transaction
                    foreach (var item in approveFlow_data)
                    {

                        await VendTranRepo.InsertAsync(new VenderApproveTrans_TB
                        {
                            EMAIL = item.Email,
                            APPROVELEVEL = item.ApproveLevel,
                            APPROVEMASTERID = item.ApproveMasterId,
                            REQUESTID = (int)VendID,
                            ISCURRENTAPPROVE = 1,
                            ISALERT = 0,
                            ISSKIPALERT = item.IsSkipAlert
                        });
                    }

                    var currentRecord = await VendTableRepo.GetAsync((int)VendID);
                    currentRecord.CURRENTAPPROVESTEP = 1;
                    await VendTableRepo.UpdateAsync(currentRecord);

                    // update approve trans
                    //var approveTransByRequestID = await unitOfWork.VenderControl.GetApproveTransByRequestID((int)VendID,approvemasterid);
                    //var approveTransLevel1 = approveTransByRequestID.Where(x => x.APPROVELEVEL == 1).FirstOrDefault();
                    //var approveTrans = await VendTranRepo.GetAsync(approveTransLevel1.ID);
                    //approveTrans.SENDEMAILDATE = RequestDate;
                    //await VendTranRepo.UpdateAsync(approveTrans);

                    // insert TB_VenderLogDoc
                    foreach (int str in doc_all)
                    {
                        if (str > 0)
                        {
                            var DocRef_Name = docrefALL.Where(x => x.ID == str).Select(s => s.DESCRIPTION).FirstOrDefault();
                            var Remark = "";

                            if (str == 6) { Remark = DocRef6_Desc; }
                            else if (str == 9) { Remark = DocRef9_Desc; }
                            else if (str == 15) { Remark = DocRef15; }
                            else if (str == 16) { Remark = DocRef16_Desc; }
                            else { Remark = null; }

                            await VendLogDocRepo.InsertAsync(new VenderLogDoc_TB
                            {
                                REQUESTID = (int)VendID,
                                DOCID = str,
                                DOCDESCRIPTION = DocRef_Name,
                                REMARK = Remark,
                                CREATEBY = CreateBy,
                                CREATEDATE = RequestDate,
                                ISACTIVE = 1

                            });
                        }
                    }

                    // Upload file. & Insert TB_VenderLogFile
                    string basePath = $"wwwroot/files/VenderFiles/{(string)RequestCode_pathfile}";
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }

                    //list file
                    //var fileok = new List<string>();
                    var filePath = Path.GetTempFileName();
                    if (UploadFile_1 != null)
                    {
                        foreach (IFormFile postedFile in UploadFile_1)
                        {
                            string fileName = Path.GetFileName(postedFile.FileName);
                            //fileok.Add($"{basePath}/{fileName}");

                            using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                            {
                                await postedFile.CopyToAsync(stream);
                                await VendLogFileRepo.InsertAsync(new VenderLogFile_TB
                                {
                                    REQUESTID = (int)VendID,
                                    FILENAME = fileName,
                                    UPLOADDATE = RequestDate,
                                    CREATEBY = CreateBy,
                                    ISACTIVE = 1
                                });
                            }
                        }
                    }

                    // generate nonce
                    var nonceKey = Guid.NewGuid().ToString();

                    await nonceRepo.InsertAsync(new VenderNonce_TB
                    {
                        NonceKey = nonceKey,
                        CreateDate = RequestDate,
                        ExpireDate = RequestDate.AddDays(7),
                        IsUsed = 0
                    });

                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(CreateBy);
                    var EmployeeId = UserALL.EmployeeId;
                    var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var EmpALL = await EmpRepo.GetAllAsync();
                    var EmpFName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.Name).FirstOrDefault();
                    var EmpLName = EmpALL.Where(x => x.EmployeeId == EmployeeId).Select(s => s.LastName).FirstOrDefault();
                    var EmpFullName = EmpFName + " " + EmpLName;

                    //throw new Exception(EmpFullName);
                    var AppMapRepo = new GenericRepository<VenderApproveMapping_TB>(unitOfWork.Transaction);
                    var AppMapALL = await AppMapRepo.GetAllAsync();
                    var AppMap = AppMapALL.Where(x => x.APPROVEMASTERID == approvemasterid && x.CreateBy == CreateBy).Select(s=>s.APPROVEGROUPID).FirstOrDefault();
                    var AppGroupRepo = new GenericRepository<VenderApproveGroupMaster_TB>(unitOfWork.Transaction);
                    var AppGroup = await AppGroupRepo.GetAsync(AppMap);
                    /*throw new Exception(AppGroup.DESCRIPTION);*/

                    var approveTransByRequestID = await unitOfWork.VenderControl.GetApproveTransByRequestID((int)VendID, approvemasterid);
                    var approveTransLevel1 = approveTransByRequestID.Where(x => x.APPROVELEVEL == 1);
                    foreach (var i in approveTransLevel1)
                    {
                        var approveFlowName = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid && 
                                                                  x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                  x.Email == i.EMAIL).Select(s => s.Name).FirstOrDefault();

                        var ApproveBy = approveFlowName;

                        var approveTransAll = await VendTranRepo.GetAllAsync();
                        var approveTrans = approveTransAll.Where(x => x.ID == i.ID).FirstOrDefault();
                        approveTrans.SENDEMAILDATE = RequestDate;
                        await VendTranRepo.UpdateAsync(approveTrans);

                        var sendEmail = _emailService.SendEmail(
                              $"Subject : {RequestCode} / Create {AppGroup.DESCRIPTION} / ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ / {VendCode}",
                               $@"
                                    <b> ร้องขอเพื่อสร้างผู้ขายรายใหม่ในระบบ {VendCode} </b><br/>
                                    <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                    <b> รหัสผู้ขาย </b> {VendCode}<br/>
                                    <b> ชื่อผู้ขาย  </b> {VenderName}<br/>
                                    <b> ที่อยู่     </b> {Address}<br/>
                                    <b> ชื่อผู้ติดต่อ </b> {ContactName}<br/>
                                    <b> โทรศัพท์  </b> {Telephone}<br/>
                                    <br/><br/><br/>
                                    <b> ชื่อผู้ทำการอนุมัติ </b> {ApproveBy}<br/></br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={i.EMAIL}'>คลิกที่นี่</a> <br/>
                                ",
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

                    unitOfWork.Complete();

                    AlertSuccess = "Add Vendor Success.";

                    return Redirect("/Vender");

                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Vender/Add");
            }
        }

    }
}
