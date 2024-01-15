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
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Purchase.NewRequest
{
    public class CreateModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string VendorID { get; set; }
        [BindProperty]
        public string SupplierName { get; set; }
        [BindProperty]
        public string Dealer { get; set; }
        [BindProperty]
        public string ProductionSite { get; set; }
        [BindProperty]
        public string DealerAddress { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public decimal Price { get; set; }
        [BindProperty]
        public string VendorIDREF { get; set; }
        [BindProperty]
        public string SupplierNameREF { get; set; }
        [BindProperty]
        public string DealerREF { get; set; }
        [BindProperty]
        public string ProductionSiteREF { get; set; }
        [BindProperty]
        public string DealerAddressREF { get; set; }
        [BindProperty]
        public string ItemCodeREF { get; set; }
        [BindProperty]
        public string ItemNameREF { get; set; }
        [BindProperty]
        public decimal PriceREF { get; set; }
        [BindProperty]
        public string Process { get; set; }
        [BindProperty]
        public List<IFormFile> FileUpload { get; set; }
        [BindProperty]
        public string CurrencyCode { get; set; }
        public List<SelectListItem> CurrencyCodeMaster { get; set; }
        [BindProperty]
        public string CurrencyCodeREF { get; set; }
        public List<SelectListItem> CurrencyCodeREFMaster { get; set; }
        [BindProperty]
        public string PerUnit { get; set; }
        public List<SelectListItem> PerUnitMaster { get; set; }
        [BindProperty]
        public string PerUnitREF { get; set; }
        public List<SelectListItem> PerUnitREFMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IHelperService _helperService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public CreateModel(
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
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_NEWREQUEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_PURCHASE));

                    CurrencyCodeMaster = await GetCurrencyCodeMaster();
                    CurrencyCodeREFMaster = await GetCurrencyCodeREFMaster();
                    PerUnitMaster = await GetPerUnitMaster();
                    PerUnitREFMaster = await GetPerUnitREFMaster();

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/NewRequest/Main");
            }
        }
        public async Task<IActionResult> OnPostAsync(string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RequestDate = DateTime.Now;
                    var RequestBy = _authService.GetClaim().UserId;
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestALL = await NewRequestRepo.GetAllAsync();

                    //GENERATE REQUEST CODE
                    var YearNow = @DateTime.Now.ToString("yyyy", new CultureInfo("en-US")).Substring(2, 2);
                    var MonthNow = @DateTime.Now.ToString("MM", new CultureInfo("en-US"));
                    var RequestCode = "";
                    var RequestCode_pathfile = "";
                    var chkYear = NewRequestALL.Where(x => x.REQUESTCODE.Substring(11, 2) == YearNow &&
                                                          x.REQUESTCODE.Substring(8, 2) == MonthNow
                                                    ).Max(a => a.REQUESTCODE.Substring(4, 3));
                    int reqCode_int;
                    if (chkYear == null)
                    {
                        RequestCode = "S2E-001-" + MonthNow + "-" + YearNow;
                        RequestCode_pathfile = "S2E_001_" + MonthNow + "_" + YearNow;
                    }
                    else
                    {
                        reqCode_int = Int32.Parse(chkYear) + 1;
                        RequestCode = "S2E-" + reqCode_int.ToString().PadLeft(3, '0') + "-" + MonthNow + "-" + YearNow;
                        RequestCode_pathfile = "S2E_" + reqCode_int.ToString().PadLeft(3, '0') + "_" + MonthNow + "_" + YearNow;
                    }

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapALLByRequestBy = approveMapALL.Where(x => x.CreateBy == RequestBy &&
                                                                   x.STEP == 1 &&
                                                                   x.ISNEWREQUEST == 1
                                                              ).FirstOrDefault();

                    var approvemasterid = approveMapALLByRequestBy.APPROVEMASTERID;
                    var ApproveGroupID = approveMapALLByRequestBy.APPROVEGROUPID;

                    //Draft or Save
                    var ApproveStatus = 0;
                    if (!string.IsNullOrEmpty(draft))
                    {
                        ApproveStatus = RequestStatusModel.Draft;
                    }
                    if (!string.IsNullOrEmpty(save))
                    {
                        ApproveStatus = RequestStatusModel.WaitingForApprove;
                    }

                    //INSERT NEW REQUEST
                    var newRequest = new S2ENewRequest_TB
                    {
                        REQUESTCODE = RequestCode,
                        REQUESTDATE = RequestDate,
                        VENDORID = VendorID,
                        SUPPLIERNAME = SupplierName,
                        DEALER = Dealer,
                        PRODUCTIONSITE = ProductionSite,
                        DEALERADDRESS = DealerAddress,
                        ITEMCODE = ItemCode,
                        ITEMNAME = ItemName,
                        PRICE = Price,
                        CURRENCYCODE = CurrencyCode,
                        PERUNIT = PerUnit,
                        ISCOMPAIRE = Process == "1" ? 1 : 0,
                        APPROVEMASTERID = approvemasterid,
                        APPROVEGROUPID = ApproveGroupID,
                        CURRENTAPPROVESTEP = 1,
                        APPROVESTATUS = ApproveStatus,
                        ISACTIVE = 1,
                        CREATEBY = RequestBy,
                        CREATEDATE = RequestDate
                        
                    };
                    
                    var RequestID = await NewRequestRepo.InsertAsync(newRequest);

                    //INSERT NEW REQUEST COMPAIRE
                    if (Process == "1")
                    {
                        //UPDATE OLD COMPAIRE
                        var NewRequestCompaireOLDRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                        var NewRequestCompaireOLDALL = NewRequestCompaireOLDRepo.GetAll();
                        var NewRequestCompaireOLD = NewRequestCompaireOLDALL.Where(x => x.REQUESTID == RequestID && x.ISCURRENTCOMPAIRE == 1);
                        if (NewRequestCompaireOLD.ToList().Count != 0)
                        {
                            foreach (var CompaireOld in NewRequestCompaireOLD)
                            {
                                var NewRequestCompaireOLDUpdate = await NewRequestCompaireOLDRepo.GetAsync(CompaireOld.ID);
                                NewRequestCompaireOLDUpdate.ISCURRENTCOMPAIRE = 0;
                                await NewRequestCompaireOLDRepo.UpdateAsync(NewRequestCompaireOLDUpdate);
                            }
                        }

                        var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                        await NewRequestCompaireRepo.InsertAsync(new S2ENewRequestCompaire_TB
                        {
                            REQUESTID = (int)RequestID,
                            VENDORIDREF = VendorIDREF,
                            SUPPLIERNAMEREF = SupplierNameREF,
                            DEALERREF = DealerREF,
                            PRODUCTIONSITEREF = ProductionSiteREF,
                            DEALERADDRESSREF = DealerAddressREF,
                            ITEMCODEREF = ItemCodeREF,
                            ITEMNAMEREF = ItemNameREF,
                            PRICEREF = PriceREF,
                            CURRENCYCODEREF = CurrencyCodeREF,
                            PERUNITREF = PerUnitREF,
                            ISCURRENTCOMPAIRE = 1
                        });
                    }

                    //UPLOAD FILE & INSERT LOG FILE
                    int row = FileUpload.Count();
                    string basePath = $"wwwroot/files/S2EFiles/{(string)RequestCode_pathfile}/NewRequest";
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }
                    //var fileok = new List<string>();
                    var filePath = Path.GetTempFileName();
                    string fileName = "";
                    var S2ELogsFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                    for (int i = 0; i < row; i++)
                    {
                        if (FileUpload[i] != null)
                        {
                            fileName = Path.GetFileName(FileUpload[i].FileName);
                            using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                            {
                                await FileUpload[i].CopyToAsync(stream);
                                await S2ELogsFileRepo.InsertAsync(new S2ENewRequestLogsFile_TB
                                {
                                    REQUESTID = (int)RequestID,
                                    FILENAME = fileName,
                                    CREATEBY = RequestBy,
                                    CREATEDATE = RequestDate
                                });
                            }

                        }
                    }


                    if (!string.IsNullOrEmpty(save))
                    {
                        //GET APPROVE FLOW BY APPROVE MASTER ID
                        var appoveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                        var appoveFlowALL = await appoveFlowRepo.GetAllAsync();
                        var approveFlow_data = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                        x.ApproveLevel != 0 &&
                                                                        x.IsActive == 1
                                                                   ).OrderBy(o => o.ApproveLevel);

                        // GENERATE NONCE
                        var nonceKey = Guid.NewGuid().ToString();
                        var nonceRepo = new GenericRepository<S2ENewRequestNonce_TB>(unitOfWork.Transaction);
                        await nonceRepo.InsertAsync(new S2ENewRequestNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = RequestDate,
                            EXPIREDATE = RequestDate.AddDays(7),
                            ISUSED = 0
                        });

                        // INSERT PC APPROVE TRANSECTION
                        var NewRequestAppTranRepo = new GenericRepository<S2ENewRequestApproveTrans_TB>(unitOfWork.Transaction);
                        foreach (var AppFlow in approveFlow_data)
                        {

                            await NewRequestAppTranRepo.InsertAsync(new S2ENewRequestApproveTrans_TB
                            {
                                REQUESTID = (int)RequestID,
                                APPROVEMASTERID = AppFlow.ApproveMasterId,
                                APPROVEGROUPID = ApproveGroupID,
                                EMAIL = AppFlow.Email,
                                APPROVELEVEL = AppFlow.ApproveLevel,
                                ISCURRENTAPPROVE = 1,
                                ISKEYINWHENAPPROVE = AppFlow.IsKeyinWhenApprove
                            });
                        }

                        var currentRecord = await NewRequestRepo.GetAsync((int)RequestID);
                        currentRecord.CURRENTAPPROVESTEP = 1;
                        await NewRequestRepo.UpdateAsync(currentRecord);

                        //GET APPROVE TRANS LEVEL 1
                        var AppTransByRequestID = await unitOfWork.S2EControl.GetApproveTransByRequestID((int)RequestID, approvemasterid, ApproveGroupID);
                        var AppTransLevel1 = AppTransByRequestID.Where(x => x.APPROVELEVEL == 1);
                        foreach (var AppTrans in AppTransLevel1)
                        {
                            var ApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                      x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                      x.Email == AppTrans.EMAIL).Select(s => s.Name).FirstOrDefault();

                            var AppTransByALL = await NewRequestAppTranRepo.GetAllAsync();
                            var AppTransByID = AppTransByALL.Where(x => x.ID == AppTrans.ID).FirstOrDefault();
                            AppTransByID.SENDEMAILDATE = RequestDate;
                            await NewRequestAppTranRepo.UpdateAsync(AppTransByID);

                            var BodyEmail = "";
                            if (Process == "1")
                            {
                                BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {RequestDate.ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายละเอียดผู้ขาย/วัตถุดิบ (ที่มีอยู่)  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td colspan='2'>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {VendorIDREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td colspan='2'>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {SupplierNameREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td width='50%'>
                                                <b> ตัวแทนจำหน่าย :</b> {DealerREF}
                                            </td>
                                            <td width='50%'>
                                                <b> แหล่งผลิต :</b> {ProductionSiteREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td colspan='2'>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {DealerAddressREF.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td colspan='2'>
                                                <b> Item Code :</b> {ItemCodeREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td colspan='2'>
                                                <b> Item Name :</b> {ItemNameREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td colspan='2'>
                                                <b> ราคา :</b> <b> ราคา :</b> {String.Format("{0:#,##0.#0}", PriceREF)} {CurrencyCodeREF} / {PerUnitREF}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b> รายการวัตถุดิบที่นำเข้า / นำมาเปรียบเทียบ  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {VendorID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {SupplierName}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {Dealer}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {ProductionSite}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {DealerAddress.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {ItemCode}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {ItemName}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", Price)} {CurrencyCode} / {PerUnit}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/NewRequestTodolist?Email={AppTrans.EMAIL}'> คลิกที่นี่ </a>
                                    <br/>
                                ";
                            }
                            else
                            {
                                BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {RequestDate.ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายการวัตถุดิบที่นำเข้า / นำมาเปรียบเทียบ  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {VendorID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {SupplierName}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {Dealer}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                 <b> แหล่งผลิต :</b> {ProductionSite}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {DealerAddress.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {ItemCode}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {ItemName}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", Price)} {CurrencyCode} / {PerUnit}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/NewRequestTodolist?Email={AppTrans.EMAIL}'> คลิกที่นี่ </a>
                                    <br/>
                                ";
                            }

                            var sendEmail = _emailService.SendEmail(
                                  $"{RequestCode} / Request / ร้องขอเพื่อทดสอบวัตถุดิบ",
                                  BodyEmail,
                                  new List<string> { AppTrans.EMAIL },
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

                    AlertSuccess = "สร้างใบร้องขอทดสอบวัตถุดิบสำเร็จ";
                    return Redirect("/S2E/Purchase/NewRequest/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/NewRequest/Create");
            }
        }
        public async Task<JsonResult> OnPostVendorGridAsync(string VendID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        ACCOUNTNUM = "ACCOUNTNUM",
                        NAME = "NAME",
                        VENDGROUP = "VENDGROUP"

                    };

                    var filter = _datatablesService.Filter(Request, field);

                    var VendorAXByGroup = await unitOfWork.Transaction.Connection.QueryAsync<S2EMasterAX_Vendor_TB>(@"
                        SELECT VENDGROUP,ACCOUNTNUM,NAME
                            FROM TB_MasterAX_Vendor
                            WHERE DATAAREAID = 'DV' 
                            AND ACCOUNTNUM <> @VendID
                            AND " + filter + @" 
                        ", 
                        new
                        {
                            @VendID = VendID
                        }, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.Format(Request, VendorAXByGroup.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<JsonResult> OnPostItemGridAsync(string ItemGroup)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        ITEMID = "ITEMID",
                        ITEMNAME = "ITEMNAME"

                    };

                    var filter = _datatablesService.Filter(Request, field);

                    var VendorAXByGroup = await unitOfWork.Transaction.Connection.QueryAsync<S2EMasterAX_Item_TB>(@"
                            SELECT ITEMID,ITEMNAME
                                FROM TB_MasterAX_Item
                                WHERE DATAAREAID = 'DV' 
                                AND ITEMGROUPID = @ItemGroup
                                AND " + filter + @" 
                            ",
                        new
                        {
                            @ItemGroup = ItemGroup
                        }, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.Format(Request, VendorAXByGroup.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<List<SelectListItem>> GetCurrencyCodeMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var CurrencyCodeRepo = new GenericRepository<CurrencyTable>(unitOfWork.Transaction);

                var CurrencyCodeALL = await CurrencyCodeRepo.GetAllAsync();

                return CurrencyCodeALL
                    .Select(x => new SelectListItem
                    {
                        Value = x.CURRENCYCODE.ToString(),
                        Text = x.CURRENCYCODE
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetCurrencyCodeREFMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var CurrencyCodeREFRepo = new GenericRepository<CurrencyTable>(unitOfWork.Transaction);

                var CurrencyCodeREFALL = await CurrencyCodeREFRepo.GetAllAsync();

                return CurrencyCodeREFALL
                    .Select(x => new SelectListItem
                    {
                        Value = x.CURRENCYCODE.ToString(),
                        Text = x.CURRENCYCODE
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetPerUnitMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var UnitRepo = new GenericRepository<Unit_TB>(unitOfWork.Transaction);

                var UnitALL = await UnitRepo.GetAllAsync();

                return UnitALL
                    .Select(x => new SelectListItem
                    {
                        Value = x.UNIT,
                        Text = x.UNIT
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetPerUnitREFMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var UnitRepo = new GenericRepository<Unit_TB>(unitOfWork.Transaction);

                var UnitALL = await UnitRepo.GetAllAsync();

                return UnitALL
                    .Select(x => new SelectListItem
                    {
                        Value = x.UNIT,
                        Text = x.UNIT
                    })
                    .ToList();
            }
        }
    }
}
