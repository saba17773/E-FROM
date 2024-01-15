using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NPOI.HSSF.Record;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Vender
{
    public class ReviseSuccessVenderModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public DateTime? RequestDate { get; set; }
        [BindProperty]
        public string VenderCode { get; set; }

        [BindProperty]
        public string VenderCodeAX { get; set; }

        [BindProperty]
        [StringLength(20)]
        [MaxLength(13)]
        public string VenderIDNum { get; set; }

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
        public int VenderGroup { get; set; }

        [BindProperty]
        public int VenderType { get; set; }

        [BindProperty]
        [StringLength(10)]
        public string Currency { get; set; }

        [BindProperty]
        public string ProductType { get; set; }

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
        public int UpdateBy { get; set; }

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

        public ReviseSuccessVenderModel(
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
        public async Task GetData(int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var vendTbTempRepo = new GenericRepository<VenderTable_TempTB>(unitOfWork.Transaction);
                var vendTbTempALL = await vendTbTempRepo.GetAllAsync();
                var vendTbTemp = vendTbTempALL.Where(x => x.REQUESTID == id && x.ISCOMPLETE == 0).FirstOrDefault();
                if (vendTbTemp != null)
                {
                    var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var vend = await venderTableRepo.GetAsync(id);

                    if (vend.VENDCODE_AX == null) { VenderCodeAX = null; } else { VenderCodeAX = vend.VENDCODE_AX; }
                    RequestCode = vend.REQUESTCODE;
                    RequestDate = vend.REQUESTDATE;
                    VenderCode = vend.VENDCODE;
                    VenderIDNum = vend.VENDIDNUM;
                    isActive = vend.ISACTIVE;
                    VenderName = vend.VENDNAME;
                    if (vend.DATAAREAID == "dv") { DataAreaId1 = true; } else { DataAreaId1 = false; }
                    if (vend.DATAAREAID == "dsc") { DataAreaId2 = true; } else { DataAreaId2 = false; }
                    DataAreaId = vend.DATAAREAID;
                    //temp
                    VenderGroup = vendTbTemp.VENDGROUPID_TEMP;
                    VenderType = vendTbTemp.VENDTYPEID_TEMP;
                    Currency = vendTbTemp.CURRENCY_TEMP;
                    ProductType = vendTbTemp.PRODTYPEID_TEMP;
                    Paymterm = vendTbTemp.PAYMTERMID_TEMP;
                    Address = vendTbTemp.ADDRESS_TEMP;
                    ContactName = vendTbTemp.CONTACTNAME_TEMP;
                    Telephone = vendTbTemp.TEL_TEMP;
                    Fax = vendTbTemp.FAX_TEMP;
                    Email = vendTbTemp.EMAIL_TEMP;
                    Website = vendTbTemp.WEBSITE_TEMP;
                    Remark = vendTbTemp.REMARK_TEMP;
                    ProductTypeDetail = vendTbTemp.PRODTYPEDETAIL_TEMP;

                    var vendDocRepo = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);
                    var vendDocALL = await vendDocRepo.GetAllAsync();

                    //checkbox
                    foreach (var docLog in vendDocALL.Where(x => x.REQUESTID == id && x.ISACTIVE == 0 && x.ISTEMP == 1))
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

                }
                else
                {
                    var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    //var vend = await venderTableRepo.GetAsync(id);
                    var vend = await venderTableRepo.GetAsync(id);

                    if (vend.VENDCODE_AX == null) { VenderCodeAX = null; } else { VenderCodeAX = vend.VENDCODE_AX; }
                    RequestCode = vend.REQUESTCODE;
                    RequestDate = vend.REQUESTDATE;
                    VenderCode = vend.VENDCODE;
                    VenderIDNum = vend.VENDIDNUM;
                    VenderGroup = vend.VENDGROUPID;
                    VenderType = vend.VENDTYPEID;
                    Currency = vend.CURRENCY;
                    ProductType = vend.PRODTYPEID;
                    Paymterm = vend.PAYMTERMID;
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
                    isActive = vend.ISACTIVE;
                    ProductTypeDetail = vend.PRODTYPEDETAIL;

                    var vendDocRepo = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);
                    var vendDocALL = await vendDocRepo.GetAllAsync();

                    //checkbox
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
                }

                var vendGroupRepo = new GenericRepository<VenderGroup_TB>(unitOfWork.Transaction);
                var vendGroupALL = await vendGroupRepo.GetAllAsync();
                var vendGroupActive = vendGroupALL.Where(x => x.ISACTIVE == 1).ToList();

                var vendTypeRepo = new GenericRepository<VenderType_TB>(unitOfWork.Transaction);
                var vendTypeALL = await vendTypeRepo.GetAllAsync();
                var vendTypeActive = vendTypeALL.Where(x => x.ISACTIVE == 1).ToList();

                var currencyRepo = new GenericRepository<VenderCurrency_TB>(unitOfWork.Transaction);
                var currencyALL = await currencyRepo.GetAllAsync();
                var currencyActive = currencyALL.Where(x => x.ISACTIVE == 1).ToList();

                var prodTypeRepo = new GenericRepository<VenderProductType_TB>(unitOfWork.Transaction);
                var prodTypeALL = await prodTypeRepo.GetAllAsync();
                var prodTypeActive = prodTypeALL.Where(x => x.ISACTIVE == 1).ToList();

                VenderGroupMaster = await GetVenderGroupMaster();
                VenderTypeMaster = await GetVenderTypeMaster();
                CurrencyMaster = await GetCurrencyMaster();
                ProductTypeMaster = await GetProductTypeMaster();
                PaymtermMaster = await GetPaymtermMaster();

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
		                    CONVERT(VARCHAR,UPLOADDATE,103)+' '+CONVERT(VARCHAR,UPLOADDATE,108) AS UPLOADDATE,
		                    ISACTIVE,
		                    CREATEBY,
		                    ISTEMP
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
        public async Task<IActionResult> OnGetUpdateFileAsync(int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                    var vendLogFile = await vendLogFileRepo.GetAsync(Fileid);

                    vendLogFile.ISACTIVE = 0;

                    await vendLogFileRepo.UpdateAsync(vendLogFile);
                    var vendLogFileALL = await vendLogFileRepo.GetAllAsync();
                    var vendTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var vendTable = await vendTableRepo.GetAsync(vendLogFile.REQUESTID);

                    var ReqCode = vendLogFile.REQUESTID;

                    var filePath = $"wwwroot/files/VenderFiles/" + vendTable.REQUESTCODE.Substring(0, 4) + "_" + vendTable.REQUESTCODE.Substring(5, 2) + "_" + vendTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();
                    var fileName = vendLogFile.FILENAME;

                    System.IO.File.Delete($"{filePath}/{fileName}");

                    unitOfWork.Complete();

                    AlertSuccess = "ดำเนินการเสร็จสิ้น";
                    return Redirect("/Vender/" + ReqCode + "/ReviseSuccessVender");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
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

            if (Remark == null)
            {
                
                AlertError = "กรุณาใส่สาเหตุที่ต้องการแก้ไขข้อมูลผู้ขาย";
                return Redirect($"/Vender/{id}/ReviseSuccessVender");
            }


            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    var nonceRepo = new GenericRepository<VenderNonce_TB>(unitOfWork.Transaction);

                    var venderTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var venderTable = await venderTableRepo.GetAsync(id);
                    var UpdateDate = DateTime.Now;
                    //update vender table
                    venderTable.CURRENTAPPROVESTEP = 1;
                    venderTable.APPROVESTATUS = 3;
                    venderTable.SENDMAILSUCCESS = 0;
                    venderTable.ISREVISE = 1;


                    var ReviseBy = _authService.GetClaim().UserId;

                    int approvemasterid = 0;

                    var vendgroupid = venderTable.VENDPROCESSID;

                    var venderMappingRepo = new GenericRepository<VenderApproveMapping_TB>(unitOfWork.Transaction);
                    var venderMappingALL = await venderMappingRepo.GetAllAsync();
                    var venderMappingByCreateBy = venderMappingALL.Where(x => x.CreateBy == ReviseBy &&
                                                                            x.DATAAREAID == venderTable.DATAAREAID &&
                                                                            x.APPROVEGROUPID == vendgroupid).FirstOrDefault();

                    approvemasterid = venderMappingByCreateBy.APPROVEMASTERID;
                   
                    venderTable.APPROVEMASTERID = approvemasterid;


                    if (Address == null) { Address = ""; } else { Address = Address; }
                    if (ContactName == null) { ContactName = ""; } else { ContactName = ContactName; }
                    if (Telephone == null) { Telephone = ""; } else { Telephone = Telephone; }
                    if (Fax == null) { Fax = ""; } else { Fax = Fax; }
                    if (Website == null) { Website = ""; } else { Website = Website; }
                    if (Email == null) { Email = ""; } else { Email = Email; }

                    var vendTBTempRepo = new GenericRepository<VenderTable_TempTB>(unitOfWork.Transaction);
                    await vendTBTempRepo.InsertAsync(new VenderTable_TempTB
                    {
                        REQUESTID = venderTable.ID,
                        ADDRESS_TEMP = Address,
                        CONTACTNAME_TEMP = ContactName,
                        TEL_TEMP = Telephone,
                        FAX_TEMP = Fax,
                        WEBSITE_TEMP = Website,
                        EMAIL_TEMP = Email,
                        VENDGROUPID_TEMP = VenderGroup,
                        VENDTYPEID_TEMP = VenderType,
                        CURRENCY_TEMP = Currency,
                        PRODTYPEID_TEMP = ProductType,
                        PAYMTERMID_TEMP = Paymterm,
                        REMARK_TEMP = Remark,
                        ISCOMPLETE = 0,
                        PRODTYPEDETAIL_TEMP = ProductTypeDetail
                    });

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
                            PROCESS = "Revise",
                            ISALERT = 0,
                            ISSKIPALERT = item.IsSkipAlert
                        });
                    }

                    //delete logdoc / insert logdoc
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

                    var docrefRepo = new GenericRepository<VenderDocumentRef_TB>(unitOfWork.Transaction);
                    var docrefALL = await docrefRepo.GetAllAsync();

                    var vendLogdocRepo2 = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);

                    // insert TB_VenderLogDoc
                    var vendLogdocRepo = new GenericRepository<VenderLogDoc_TB>(unitOfWork.Transaction);
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

                            await vendLogdocRepo.InsertAsync(new VenderLogDoc_TB
                            {
                                REQUESTID = (int)id,
                                DOCID = str,
                                DOCDESCRIPTION = DocRef_Name,
                                REMARK = Remark,
                                CREATEBY = UpdateBy,
                                CREATEDATE = UpdateDate,
                                ISACTIVE = 0,
                                ISTEMP = 1

                            });
                        }
                    }

                    // Upload file. & Insert TB_VenderLogFile
                    string basePath = $"wwwroot/files/VenderFiles/" + venderTable.REQUESTCODE.Substring(0, 4) + "_" + venderTable.REQUESTCODE.Substring(5, 2) + "_" + venderTable.REQUESTCODE.Substring(8, 5);

                    var filePath2 = Path.GetTempFileName();
                    var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                    if (UploadFile_1 != null)
                    {
                        foreach (IFormFile postedFile in UploadFile_1)
                        {
                            string fileName = Path.GetFileName(postedFile.FileName);

                            using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                            {
                                await postedFile.CopyToAsync(stream);
                                await vendLogFileRepo.InsertAsync(new VenderLogFile_TB
                                {
                                    REQUESTID = venderTable.ID,
                                    FILENAME = fileName,
                                    UPLOADDATE = RequestDate,
                                    CREATEBY = UpdateBy,
                                    ISACTIVE = 0,
                                    ISTEMP = 1
                                });
                            }
                        }
                    }


                    //var fileok = new List<string>();
                    var filePath = $"wwwroot/files/VenderFiles/" + venderTable.REQUESTCODE.Substring(0, 4) + "_" + venderTable.REQUESTCODE.Substring(5, 2) + "_" + venderTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();


                    var vendLogFileALL = await vendLogFileRepo.GetAllAsync();

                    foreach (var filelog in vendLogFileALL.Where(x => (x.REQUESTID == venderTable.ID && x.ISACTIVE == 1) || (x.REQUESTID == venderTable.ID && x.ISTEMP == 1)))
                    {
                        string fileName = filelog.FILENAME;
                        //fileok.Add($"{filePath}/{fileName}");
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
                        var approveTrans = approveTransAll.Where(x => x.ID == i.ID).FirstOrDefault();
                        approveTrans.SENDEMAILDATE = UpdateDate;
                        await VendTranRepo.UpdateAsync(approveTrans);

                        var sendRevisemail = _emailService.SendEmail(
                              $"Subject : {venderTable.REQUESTCODE} / Revise / ร้องขอเพื่อแก้ไขข้อมูลผู้ขาย / {venderTable.VENDCODE_AX}",
                              $@"
                                    <b> ร้องขอเพื่อแก้ไขข้อมูลผู้ขาย {venderTable.VENDCODE_AX} </b><br/>
                                    <b> ผู้ร้องขอ  </b> {EmpFullName}<br/><br/>
                                    <b> รหัสผู้ขาย </b> {venderTable.VENDCODE_AX}<br/>
                                    <b> ชื่อผู้ขาย  </b> {venderTable.VENDNAME}<br/>
                                    <b> ที่อยู่     </b> {Address}<br/>
                                    <b> ชื่อผู้ติดต่อ </b> {ContactName}<br/>
                                    <b> โทรศัพท์  </b> {Telephone}<br/>
                                    <br/><br/><br/>
                                    <b style='color:red'> สาเหตุการร้องขอ Revise </b> {Remark}
                                    <br/><br/><br/>
                                    <b style='color:black'> ชื่อผู้ทำการอนุมัติ </b> {ApproveBy}<br/></br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/VenderRequestList?Email={i.EMAIL}'>คลิกที่นี่</a> <br/>       
                                ",
                               // <b style='color:black'>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/Vender/ApproveVender_Revise?VendID={id}&TranID={approveTransLevel1.ID}&nonce={nonceKey}'>คลิกที่นี่</a> <br/>
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

        public async Task<IActionResult> OnGetDelelteFile(int FileID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var vendLogFileRepo = new GenericRepository<VenderLogFile_TB>(unitOfWork.Transaction);
                    var vendLogFile = await vendLogFileRepo.GetAsync(FileID);

                    vendLogFile.ISACTIVE = 0;

                    await vendLogFileRepo.UpdateAsync(vendLogFile);
                    var vendLogFileALL = await vendLogFileRepo.GetAllAsync();
                    var vendTableRepo = new GenericRepository<VenderTable_TB>(unitOfWork.Transaction);
                    var vendTable = await vendTableRepo.GetAsync(vendLogFile.REQUESTID);

                    var ReqCode = vendLogFile.REQUESTID;

                    var filePath = $"wwwroot/files/VenderFiles/" + vendTable.REQUESTCODE.Substring(0, 4) + "_" + vendTable.REQUESTCODE.Substring(5, 2) + "_" + vendTable.REQUESTCODE.Substring(8, 5); //Path.GetTempFileName();
                    var fileName = vendLogFile.FILENAME;

                    System.IO.File.Delete($"{filePath}/{fileName}");

                    unitOfWork.Complete();

                    AlertSuccess = "ลบไฟล์สำเร็จ";

                    return new JsonResult(true);

                }
            }
            catch (Exception)
            {
                return new JsonResult(false);
            }

        }
    }
}
