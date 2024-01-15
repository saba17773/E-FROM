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
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Purchase.NewRequest
{
    public class EditModel : PageModel
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
        public int RequestId { get; set; }
        [BindProperty]
        public string Requestcode { get; set; }
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
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public EditModel(
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
        public async Task<IActionResult> OnGetAsync(int RequestID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_NEWREQUEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    
                    var EditBy = _authService.GetClaim().UserId;
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestALL = await NewRequestRepo.GetAllAsync();
                    var NewRequestByEditBy = NewRequestALL.Where(x => x.ID == RequestID && 
                                                                    x.CREATEBY == EditBy).FirstOrDefault();
                    if (NewRequestByEditBy == null)
                    {
                        AlertError = "ไม่สามารถเข้าไปแก้ไข Request นี้ได้";
                        return Redirect($"/S2E/Purchase/NewRequest/Main");
                    }

                    CurrencyCodeMaster = await GetCurrencyCodeMaster();
                    CurrencyCodeREFMaster = await GetCurrencyCodeREFMaster();
                    PerUnitMaster = await GetPerUnitMaster();
                    PerUnitREFMaster = await GetPerUnitREFMaster();

                    RequestId = RequestID;
                    await GetData(RequestID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/NewRequest/Main");
            }
        }
        public async Task GetData(int RequestID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                Requestcode = NewRequestByID.REQUESTCODE;
                VendorID = NewRequestByID.VENDORID;
                SupplierName = NewRequestByID.SUPPLIERNAME;
                Dealer = NewRequestByID.DEALER;
                ProductionSite = NewRequestByID.PRODUCTIONSITE;
                DealerAddress = NewRequestByID.DEALERADDRESS;
                ItemCode = NewRequestByID.ITEMCODE;
                ItemName = NewRequestByID.ITEMNAME;
                Price = NewRequestByID.PRICE;
                Process = NewRequestByID.ISCOMPAIRE == 1 ? "1" : "0";
                CurrencyCode = NewRequestByID.CURRENCYCODE;
                PerUnit = NewRequestByID.PERUNIT;

                if (NewRequestByID.ISCOMPAIRE == 1)
                {
                    var NewRequestCompireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                    var NewRequestCompireALL = await NewRequestCompireRepo.GetAllAsync();
                    var NewRequestCompireByRequestID = NewRequestCompireALL.Where(x=>x.REQUESTID == RequestID).FirstOrDefault();

                    VendorIDREF = NewRequestCompireByRequestID.VENDORIDREF;
                    SupplierNameREF = NewRequestCompireByRequestID.SUPPLIERNAMEREF;
                    DealerREF = NewRequestCompireByRequestID.DEALERREF;
                    ProductionSiteREF = NewRequestCompireByRequestID.PRODUCTIONSITEREF;
                    DealerAddressREF = NewRequestCompireByRequestID.DEALERADDRESSREF;
                    ItemCodeREF = NewRequestCompireByRequestID.ITEMCODEREF;
                    ItemNameREF = NewRequestCompireByRequestID.ITEMNAMEREF;
                    PriceREF = NewRequestCompireByRequestID.PRICEREF;
                    CurrencyCodeREF = NewRequestCompireByRequestID.CURRENCYCODEREF;
                    PerUnitREF = NewRequestCompireByRequestID.PERUNITREF;
                }


                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostGridViewFileUploadAsync(int RequestID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ENewRequestLogsFileGridViewModel>($@"
                        SELECT ID,
		                    REQUESTID,
		                    FILENAME,
                            CREATEBY,
		                    CONVERT(NVARCHAR,CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,CREATEDATE,108) AS CREATEDATE
                    FROM TB_S2ENewRequestLogsFile 
                    WHERE REQUESTID = {RequestID}
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
        public async Task<IActionResult> OnGetDownloadFileUploadAsync(int RequestID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var NewRequestLogFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                    var NewRequestLogFileByID = await NewRequestLogFileRepo.GetAsync(Fileid);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/NewRequest";

                    var fileName = NewRequestLogFileByID.FILENAME;

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
        public async Task<IActionResult> OnGetDelelteFile(int FileID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var NewRequestLogFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                    var NewRequestLogFileByID = await NewRequestLogFileRepo.GetAsync(FileID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(NewRequestLogFileByID.REQUESTID);

                    await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        DELETE FROM TB_S2ENewRequestLogsFile 
                        WHERE ID = {FileID}
                    ", null, unitOfWork.Transaction);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/NewRequest";
                    var fileName = NewRequestLogFileByID.FILENAME;

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
        public async Task<IActionResult> OnPostAsync(int RequestID, string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                RequestId = RequestID;
                await GetData(RequestID);
                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var EditDate = DateTime.Now;
                    
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;     
                    var approvemasterid = NewRequestByID.APPROVEMASTERID;

                    var ApproveStatus = 0;
                    if (!string.IsNullOrEmpty(draft))
                    {
                        ApproveStatus = 8;
                    }
                    if (!string.IsNullOrEmpty(save))
                    {
                        ApproveStatus = 3;
                    }
                    var EditBy = _authService.GetClaim().UserId;

                    //UPDATE PC Request
                    NewRequestByID.REQUESTDATE = EditDate;
                    NewRequestByID.VENDORID = VendorID;
                    NewRequestByID.SUPPLIERNAME = SupplierName;
                    NewRequestByID.DEALER = Dealer;
                    NewRequestByID.PRODUCTIONSITE = ProductionSite;
                    NewRequestByID.DEALERADDRESS = DealerAddress;
                    NewRequestByID.ITEMCODE = ItemCode;
                    NewRequestByID.ITEMNAME = ItemName;
                    NewRequestByID.PRICE = Price;
                    NewRequestByID.CURRENCYCODE = CurrencyCode;
                    NewRequestByID.PERUNIT = PerUnit;
                    NewRequestByID.APPROVESTATUS = ApproveStatus;
                    NewRequestByID.UPDATEBY = EditBy;
                    NewRequestByID.UPDATEDATE = EditDate;
                    
                    //UPDATE OLD APPROVE TRANS
                    var NewRequestCompaireOLDRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                    var NewRequestCompaireOLDALL = await NewRequestCompaireOLDRepo.GetAllAsync();
                    var NewRequestCompaireOLD = NewRequestCompaireOLDALL.Where(x => x.REQUESTID == RequestID && x.ISCURRENTCOMPAIRE == 1);

                    if (NewRequestCompaireOLD.ToList().Count != 0)
                    {
                        if (NewRequestByID.ISCOMPAIRE == 1)
                        {
                            foreach (var CompaireOld in NewRequestCompaireOLD)
                            {
                                var NewRequestCompaireOLDUpdate = await NewRequestCompaireOLDRepo.GetAsync(CompaireOld.ID);
                                NewRequestCompaireOLDUpdate.VENDORIDREF = VendorIDREF;
                                NewRequestCompaireOLDUpdate.SUPPLIERNAMEREF = SupplierNameREF;
                                NewRequestCompaireOLDUpdate.DEALERREF = DealerREF;
                                NewRequestCompaireOLDUpdate.PRODUCTIONSITEREF = ProductionSiteREF;
                                NewRequestCompaireOLDUpdate.DEALERADDRESSREF = DealerAddressREF;
                                NewRequestCompaireOLDUpdate.ITEMCODEREF = ItemCodeREF;
                                NewRequestCompaireOLDUpdate.ITEMNAMEREF = ItemNameREF;
                                NewRequestCompaireOLDUpdate.PRICEREF = PriceREF;
                                NewRequestCompaireOLDUpdate.CURRENCYCODEREF = CurrencyCodeREF;
                                NewRequestCompaireOLDUpdate.PERUNITREF = PerUnitREF;
                                await NewRequestCompaireOLDRepo.UpdateAsync(NewRequestCompaireOLDUpdate);
                            }
                        }
                        
                    }
                    else
                    {
                        //INSERT NEW REQUEST COMPAIRE
                        if (NewRequestByID.ISCOMPAIRE == 1)
                        {
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
                                ISCURRENTCOMPAIRE = 1
                            });
                        }
                    }

                    //UPLOAD FILE & INSERT LOG FILE
                    var RequestCodefilePath = "S2E_" + RequestCode.Substring(4, 3) + "_" +
                       RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2);

                    string basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/NewRequest";
                    //var fileok = new List<string>();

                    //Old File Before Edit
                    var NewRequestLogsFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                    var NewRequestLogsFileALL = await NewRequestLogsFileRepo.GetAllAsync();
                    var FileOldALL = NewRequestLogsFileALL.Where(x => x.REQUESTID == RequestID);
                    foreach (var FileOld in FileOldALL)
                    {
                        string FileOldName = FileOld.FILENAME;
                    }

                    //New File Upload
                    int row = FileUpload.Count();
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }
                    string fileName = "";
                    var NewRequestLogsFileRepoNew = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                    for (int i = 0; i < row; i++)
                    {
                        if (FileUpload[i] != null)
                        {
                            fileName = Path.GetFileName(FileUpload[i].FileName);
                            //fileok.Add($"{basePath}/{fileName}");
                            using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                            {
                                await FileUpload[i].CopyToAsync(stream);
                                await NewRequestLogsFileRepoNew.InsertAsync(new S2ENewRequestLogsFile_TB
                                {
                                    REQUESTID = (int)RequestID,
                                    FILENAME = fileName,
                                    CREATEBY = EditBy,
                                    CREATEDATE = EditDate
                                });
                            }

                        }
                    }

                    await NewRequestRepo.UpdateAsync(NewRequestByID);

                    if (!string.IsNullOrEmpty(save))
                    {
                        //GET APPROVE GROUP ID FROM CREATEBY
                        var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                        var approveMapALL = await approveMapRepo.GetAllAsync();

                        var ApproveGroupID = approveMapALL.Where(x => x.CreateBy == EditBy &&
                                                                      x.APPROVEMASTERID == approvemasterid &&
                                                                      x.STEP == 1 &&
                                                                      x.ISNEWREQUEST == 1
                                                                ).Select(s => s.APPROVEGROUPID).FirstOrDefault();

                        //UPDATE OLD APPROVE TRANS
                        var ApproveTransOldRepo = new GenericRepository<S2ENewRequestApproveTrans_TB>(unitOfWork.Transaction);
                        var ApproveTransOldALL = ApproveTransOldRepo.GetAll();
                        var ApproveTransOld = ApproveTransOldALL.Where(x => x.REQUESTID == RequestID && x.APPROVEGROUPID == ApproveGroupID);
                        if (ApproveTransOld.ToList().Count != 0)
                        {
                            foreach (var App in ApproveTransOld)
                            {
                                var AppTransOldUpdate = await ApproveTransOldRepo.GetAsync(App.ID);
                                AppTransOldUpdate.ISCURRENTAPPROVE = 0;
                                await ApproveTransOldRepo.UpdateAsync(AppTransOldUpdate);
                            }
                        }

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
                            CREATEDATE = EditDate,
                            EXPIREDATE = EditDate.AddDays(7),
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
                                EMAIL = AppFlow.Email,
                                APPROVELEVEL = AppFlow.ApproveLevel,
                                ISCURRENTAPPROVE = 1,
                                ISKEYINWHENAPPROVE = AppFlow.IsKeyinWhenApprove,
                                APPROVEGROUPID = ApproveGroupID
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
                            AppTransByID.SENDEMAILDATE = EditDate;
                            await NewRequestAppTranRepo.UpdateAsync(AppTransByID);

                            var BodyEmail = "";
                            if (NewRequestByID.ISCOMPAIRE == 1)
                            {
                                BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {EditDate.ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายละเอียดผู้ขาย/วัตถุดิบ (ที่มีอยู่)  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {VendorIDREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {SupplierNameREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {DealerREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {ProductionSiteREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {DealerAddressREF.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {ItemCodeREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {ItemNameREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", PriceREF)} {CurrencyCodeREF} / {PerUnitREF}
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
                                    <b> REQUEST DATE :</b> {EditDate.ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
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
                    AlertSuccess = "แก้ไขใบร้องขอทดสอบวัตถุดิบสำเร็จ";
                    return Redirect($"/S2E/Purchase/NewRequest/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Purchase/NewRequest/{RequestID}/Edit");
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
