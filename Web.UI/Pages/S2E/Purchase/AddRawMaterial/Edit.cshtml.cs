using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Purchase.AddRawMaterial
{
    public class EditModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int PurchaseSampleID { get; set; }
        [BindProperty]
        [Required]
        public decimal Qty { get; set; }
        [BindProperty]
        public string PONo { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public string ProjectRefNo { get; set; }
        [BindProperty]
        public string VendorID { get; set; }
        [BindProperty]
        public string SupplierName { get; set; }
        [BindProperty]
        public string Dealer { get; set; }
        [BindProperty]
        public string DealerAddress { get; set; }
        [BindProperty]
        public string ProductionSite { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public decimal Price { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public int isStartTest { get; set; }
        [BindProperty]
        public string isStartTestRemark { get; set; }
        [BindProperty]
        public int isPurchaseSample { get; set; }
        [BindProperty]
        public List<IFormFile> FileUpload { get; set; }
        [BindProperty]
        public int ADDRMID { get; set; }
        [BindProperty]
        public DateTime? RequestDate { get; set; }
        [BindProperty]
        public int ADDRMLineID { get; set; }
        [BindProperty]
        public decimal QtyPO { get; set; }
        [BindProperty]
        public int isVendor { get; set; }
        [BindProperty]
        public string CurrencyCode { get; set; }
        [BindProperty]
        public string Plant { get; set; }

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
        public async Task<IActionResult> OnGetAsync(int AddRMID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_ADDRAWMATERIAL));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    ADDRMID = AddRMID;
                    await GetData(AddRMID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/AddRawMaterial/Main");
            }
        }
        public async Task GetData(int AddRMID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                //var AddRMRepo = new GenericRepository<S2EAddRawMaterial_TB>(unitOfWork.Transaction);
                //var AddRMByID = await AddRMRepo.GetAsync(AddRMID);

                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                var AddRMLine = AddRMLineALL.Where(x => x.ADDRMID == AddRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                ADDRMLineID = AddRMLine.ID;

                var RequestID = AddRMHeadByID.REQUESTID;
                var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                var LABID = AddRMHeadByID.LABID;
                var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                RequestDate = AddRMLine.REQUESTDATE;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;
                Dealer = NewRequestByID.DEALER;
                ProductionSite = NewRequestByID.PRODUCTIONSITE;
                DealerAddress = NewRequestByID.DEALERADDRESS;
                
                Unit = NewRequestByID.UNIT;

                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                isStartTest = RMAssessmentByID.ISSTARTTEST == 1 ? 1 : 2;
                isStartTestRemark = RMAssessmentByID.ISSTARTTESTREMARK;

                var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                isPurchaseSample = LABTestLineByID.ISPURCHASESAMPLE;

                var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                if (PCSampleByID.VENDORID == null || PCSampleByID.VENDORID == "")
                {
                    isVendor = 0;
                }
                else
                {
                    isVendor = 1;
                }

                VendorID = AddRMHeadByID.VENDORID;
                SupplierName = AddRMHeadByID.SUPPLIERNAME;
                CurrencyCode = AddRMHeadByID.CURRENCYCODE;
                Plant = AddRMHeadByID.PLANT;

                ItemCode = PCSampleByID.ITEMCODE;
                ItemName = PCSampleByID.ITEMNAME;

                Price = AddRMLine.PRICE;
                Qty = AddRMLine.QTY;
                QtyPO = AddRMLine.QTYPO;
                PONo = AddRMLine.PONO;

                unitOfWork.Complete();
            }
        }
        public async Task<JsonResult> OnPostPONoGridAsync(string VendorID, string ItemID, int isVendor)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        PONO = "PONO"
                    };

                    var filter = _datatableService.Filter(Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EGetPONoByVendorIDGridModel>(@"
                            EXEC S2EGetPONo @VendorID,@ItemID 
                        ",
                        new
                        {
                            @VendorID = VendorID,
                            @ItemID = ItemID
                        }, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));

                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<IActionResult> OnPostGridViewFileUploadAsync(int ADDRMID,int ADDRMLineID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EAddRawMaterialLogsFileGridViewModel>($@"
                         SELECT *
                        FROM
                        (
                            SELECT ID,
		                        ADDRMID,
                                ADDRMLINEID,
		                        FILENAME,
                                CREATEBY,
		                        CONVERT(NVARCHAR,CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,CREATEDATE,108) AS CREATEDATE,
		                        ISACTIVE
                            FROM TB_S2EAddRawMaterialLogsFile 
                            WHERE ADDRMID = {ADDRMID} AND ADDRMLINEID = {ADDRMLineID} AND ISACTIVE = 1
                        )T
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
        public async Task<IActionResult> OnGetDownloadFileUploadAsync(int ADDRMID, int ADDRMLineID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMLogFileRepo = new GenericRepository<S2EAddRawMaterialLogsFile_TB>(unitOfWork.Transaction);
                    var AddRMLogFileByFileID = await AddRMLogFileRepo.GetAsync(Fileid);

                    var AddRMRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMByID = await AddRMRepo.GetAsync(ADDRMID);

                    var RequestID = AddRMByID.REQUESTID;
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + RequestCode.Substring(4, 3) + "_" +
                        RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2) + "/AddRawMaterial/"+ADDRMLineID;

                    var fileName = AddRMLogFileByFileID.FILENAME;

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
        public async Task<IActionResult> OnGetDelelteFile(int FileID,int ADDRMLineID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMLogFileRepo = new GenericRepository<S2EAddRawMaterialLogsFile_TB>(unitOfWork.Transaction);
                    var AddRMLogFileByID = await AddRMLogFileRepo.GetAsync(FileID);

                    var AddRMRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMByID = await AddRMRepo.GetAsync(AddRMLogFileByID.ADDRMID);

                    var RequestID = AddRMByID.REQUESTID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        DELETE FROM TB_S2EAddRawMaterialLogsFile 
                        WHERE ID = {FileID}
                    ", null, unitOfWork.Transaction);

                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + RequestCode.Substring(4, 3) + "_" +
                        RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2) + "/AddRawMaterial/"+ ADDRMLineID;

                    var fileName = AddRMLogFileByID.FILENAME;

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
        public async Task<IActionResult> OnPostAsync(int AddRMID, string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                ADDRMID = AddRMID;
                await GetData(AddRMID);

                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var CreateDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByCreateBy = approveMapALL.Where(x => x.CreateBy == CreateBy &&
                                                                   x.STEP == 1 &&
                                                                   x.PLANT == Plant &&
                                                                   x.ISADDRM == 1
                                                              ).FirstOrDefault();
                    if (approveMapByCreateBy == null)
                    {
                        ADDRMID = AddRMID;
                        await GetData(AddRMID);

                        AlertError = "ไม่มีสายอนุมัติ Approve Plant นี้";
                        return Redirect($"/S2E/Purchase/AddRawMaterial/{AddRMID}/Edit");

                    }

                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                    var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                    var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                    var AddRMLine = AddRMLineALL.Where(x => x.ADDRMID == AddRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();
                    var AddRMLineByID = await AddRMLineRepo.GetAsync(AddRMLine.ID);

                    var RequestID = AddRMHeadByID.REQUESTID;
                    var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                    var LABID = AddRMHeadByID.LABID;
                    var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                    var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                    var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                    if (LABTestLineByID.ISPURCHASESAMPLE == 1 && PONo == null)
                    {
                        AlertError = "กรุณาใส่ PO No. ก่อน";
                        return Redirect($"/S2E/Purchase/AddRawMaterial/{AddRMID}/Edit");
                    }

                    

                    var approvemasterid = approveMapByCreateBy.APPROVEMASTERID;
                    var ApproveGroupID = approveMapByCreateBy.APPROVEGROUPID;

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

                    //UPDATE ADD RM Head
                    AddRMHeadByID.VENDORID = VendorID;
                    AddRMHeadByID.SUPPLIERNAME = SupplierName;
                    AddRMHeadByID.CURRENCYCODE = CurrencyCode;
                    AddRMHeadByID.PLANT = Plant;

                    //UPDATE ADD RAW MATERIAL
                    AddRMLineByID.REQUESTDATE = CreateDate;
                    AddRMLineByID.PONO = PONo;
                    AddRMLineByID.QTY = Qty;
                    AddRMLineByID.QTYPO = QtyPO;
                    AddRMLineByID.PRICE = Price;
                    AddRMLineByID.APPROVESTATUS = ApproveStatus;
                    AddRMLineByID.APPROVEMASTERID = approvemasterid;
                    AddRMLineByID.CURRENTAPPROVESTEP = 1;
                    AddRMLineByID.UPDATEBY = CreateBy;
                    AddRMLineByID.UPDATEDATE = CreateDate;

                    //UPLOAD FILE & INSERT ADD RAW MATERIAL LOG FILE
                    var RequestCodefilePath = "S2E_" + RequestCode.Substring(4, 3) + "_" +
                       RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2);

                    int row = FileUpload.Count();

                    string basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{AddRMLineByID.ID}";
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }

                    var filePath = Path.GetTempFileName();
                    string fileName = "";
                    var AddRMLogsFileRepo = new GenericRepository<S2EAddRawMaterialLogsFile_TB>(unitOfWork.Transaction);
                    for (int i = 0; i < row; i++)
                    {
                        if (FileUpload[i] != null)
                        {
                            fileName = Path.GetFileName(FileUpload[i].FileName);
                            using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                            {
                                await FileUpload[i].CopyToAsync(stream);
                                await AddRMLogsFileRepo.InsertAsync(new S2EAddRawMaterialLogsFile_TB
                                {
                                    ADDRMID = AddRMID,
                                    ADDRMLINEID = AddRMLineByID.ID,
                                    FILENAME = fileName,
                                    CREATEBY = CreateBy,
                                    CREATEDATE = CreateDate,
                                    ISACTIVE = 1
                                });
                            }

                        }
                    }
                    await AddRMLineRepo.UpdateAsync(AddRMLineByID);
                    await AddRMHeadRepo.UpdateAsync(AddRMHeadByID);

                    if (!string.IsNullOrEmpty(save))
                    {
                        //UPDATE OLD APPROVE TRANS
                        var ApproveTransOldRepo = new GenericRepository<S2EAddRawMaterialApproveTrans_TB>(unitOfWork.Transaction);
                        var ApproveTransOldALL = ApproveTransOldRepo.GetAll();
                        var ApproveTransOld = ApproveTransOldALL.Where(x => x.ADDRMID == AddRMID && x.APPROVEGROUPID == ApproveGroupID);
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
                        var nonceRepo = new GenericRepository<S2EAddRawMaterialNonce_TB>(unitOfWork.Transaction);
                        await nonceRepo.InsertAsync(new S2EAddRawMaterialNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = CreateDate,
                            EXPIREDATE = CreateDate.AddDays(7),
                            ISUSED = 0
                        });

                        // INSERT PC APPROVE TRANSECTION
                        var AddRMAppTranRepo = new GenericRepository<S2EAddRawMaterialApproveTrans_TB>(unitOfWork.Transaction);
                        foreach (var AppFlow in approveFlow_data)
                        {
                            await AddRMAppTranRepo.InsertAsync(new S2EAddRawMaterialApproveTrans_TB
                            {
                                ADDRMID = AddRMID,
                                ADDRMLINEID = AddRMLine.ID,
                                APPROVEMASTERID = AppFlow.ApproveMasterId,
                                APPROVEGROUPID = ApproveGroupID,
                                EMAIL = AppFlow.Email,
                                APPROVELEVEL = AppFlow.ApproveLevel,
                                ISCURRENTAPPROVE = 1,
                                ISKEYINWHENAPPROVE = AppFlow.IsKeyinWhenApprove
                            });
                        }

                        var currentRecord = await AddRMLineRepo.GetAsync(AddRMLine.ID);
                        currentRecord.CURRENTAPPROVESTEP = 1;
                        await AddRMLineRepo.UpdateAsync(currentRecord);

                        

                        //GET APPROVE TRANS LEVEL 1
                        var AppTransByRequestID = await unitOfWork.S2EControl.GetApproveTransByAddRMID(AddRMID, approvemasterid, ApproveGroupID, AddRMLine.ID);
                        var AppTransLevel1 = AppTransByRequestID.Where(x => x.APPROVELEVEL == 1);

                        var ischeck1 = "";
                        var ischeck2 = "";
                        if (RMAssessmentByID.ISSTARTTEST == 1)
                        {
                            ischeck1 = "checked";
                            ischeck2 = "";
                        }
                        else
                        {
                            ischeck1 = "";
                            ischeck2 = "checked";
                        }

                        var ispcsample1 = "";
                        var ispcsample2 = "";
                        if (LABTestLineByID.ISPURCHASESAMPLE == 1)
                        {
                            ispcsample1 = "checked";
                            ispcsample2 = "";
                        }
                        else
                        {
                            ispcsample1 = "";
                            ispcsample2 = "checked";
                        }

                        foreach (var AppTrans in AppTransLevel1)
                        {
                            var approveFlowApproveBy = appoveFlowALL.Where(x => x.ApproveMasterId == (int)approvemasterid &&
                                                                       x.ApproveLevel == 1 && x.IsActive == 1 &&
                                                                       x.Email == AppTrans.EMAIL);

                            var FName = approveFlowApproveBy.Select(s => s.Name).FirstOrDefault();
                            var LName = approveFlowApproveBy.Select(s => s.LastName).FirstOrDefault();
                            var ApproveBy = FName + " " + LName;

                            var AppTransByALL = await AddRMAppTranRepo.GetAllAsync();
                            var AppTransByID = AppTransByALL.Where(x => x.ID == AppTrans.ID).FirstOrDefault();

                            AppTransByID.SENDEMAILDATE = CreateDate;
                            await AddRMAppTranRepo.UpdateAsync(AppTransByID);

                            var sendEmail = _emailService.SendEmail(
                                $"{RequestCode} / เพื่อแจ้งให้หน่วยงาน Store รับทราบ และจัดเก็บวัตถุดิบตัวอย่าง",
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
                                            <td>{VendorID}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ตัวแทนจำหน่าย: </td>
                                            <td colspan='3'>{SupplierName}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ที่อยู่ของตัวแทนจำหน่าย : </td>
                                            <td colspan='3'>{NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>Item Code : </td>
                                            <td>{PCSampleByID.ITEMCODE}</td>
                                            <td>PONo. : </td>
                                            <td>{PONo}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>Item Name : </td>
                                            <td colspan='3'>{PCSampleByID.ITEMNAME}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ราคา : </td>
                                            <td>{String.Format("{0:#,##0.#0}", Price)}  {CurrencyCode}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>ซื้อวัตถุดิบเพิ่มหรือไม่? : </td>
                                            <td> 
                                                <label>
                                                    <input type = 'radio' id = 'isPurchaseSampleYes' name = 'isPurchaseSample' {ispcsample1} disabled>
                                                    Yes
                                                </label>
                                                <label>
                                                    <input type = 'radio' id = 'isPurchaseSampleNo' name = 'isPurchaseSample' {ispcsample2} disabled>
                                                    No
                                                </label>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>จำนวน : </td>
                                            <td>{String.Format("{0:#,##0.#0}", Qty)}</td>
                                            <td style='text-align:right;'>หน่วย : </td>
                                            <td>{NewRequestByID.UNIT}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>เริ่มทดสอบ : </td>
                                            <td>
                                                <label>
                                                    <input type = 'radio' id = 'isStartTestYes' name = 'isStartTest' {ischeck1} disabled>
                                                    Yes
                                                </label>
                                                <label>
                                                    <input type = 'radio' id = 'isStartTestNo' name = 'isStartTest' {ischeck2} disabled>
                                                    No
                                                </label>

                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>สถานที่จัดเก็บ : </td>
                                            <td> {Plant} </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/AddRawMaterialTodolist?Email={AppTrans.EMAIL}'>  คลิกที่นี่ </a> <br/>
                                ",
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

                    AlertSuccess = "แก้ไขใบร้องขอเพิ่มวัตถุดิบเข้าระบบสำเร็จ";
                    return Redirect("/S2E/Purchase/AddRawMaterial/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Purchase/AddRawMaterial/{AddRMID}/Edit");
            }
        }
    }
}
