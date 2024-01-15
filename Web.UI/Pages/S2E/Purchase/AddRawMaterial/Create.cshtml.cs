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
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Purchase.AddRawMaterial
{
    public class CreateModel : PageModel
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
        public decimal QtyPO { get; set; }
        [BindProperty]
        public int isVendor { get; set; }
        [BindProperty]
        public string CurrencyCode { get; set; }
        [BindProperty]
        [Required]
        public string Plant { get; set; }
        [BindProperty]
        public decimal QtyLABSample { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public CreateModel(
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
        public async Task<IActionResult> OnGetAsync(int PCSampleID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_ADDRAWMATERIAL));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadALL = await AddRMHeadRepo.GetAllAsync();
                    var CheckRequest = AddRMHeadALL.Where(x => x.PCSAMPLEID == PCSampleID).FirstOrDefault();
                    if (CheckRequest != null)
                    {
                        AlertError = "Request นี้ได้ทำการสร้างแล้ว";
                        return Redirect($"/S2E/Purchase/AddRawMaterial/Main");
                    }

                    PurchaseSampleID = PCSampleID;
                    await GetData(PCSampleID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/AddRawMaterial/Main");
            }
        }
        public async Task GetData(int PCSampleID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                var RequestID = PCSampleByID.REQUESTID;
                var AssessmentID = PCSampleByID.ASSESSMENTID;
                var LABID = PCSampleByID.LABID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AssessmentID);

                var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                RequestCode = NewRequestByID.REQUESTCODE;

                isPurchaseSample = LABTestLineByID.ISPURCHASESAMPLE;

                VendorID = PCSampleByID.VENDORID;
                SupplierName = PCSampleByID.SUPPLIERNAME;
                if (PCSampleByID.VENDORID == null || PCSampleByID.VENDORID == "")
                {
                    isVendor = 0;
                }
                else {
                    isVendor = 1;
                }

                Dealer = NewRequestByID.DEALER;
                ProductionSite = NewRequestByID.PRODUCTIONSITE;
                DealerAddress = NewRequestByID.DEALERADDRESS;
                ItemCode = PCSampleByID.ITEMCODE;
                ItemName = PCSampleByID.ITEMNAME;
                //Price = NewRequestByID.PRICE;      
                Unit = NewRequestByID.UNIT;

                isStartTest = RMAssessmentByID.ISSTARTTEST == 1 ? 1 : 2;
                isStartTestRemark = RMAssessmentByID.ISSTARTTESTREMARK;

                if (isPurchaseSample == 0)
                {
                    var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                    var AddRMSampleALL = await AddRMSampleRepo.GetAllAsync();
                    var AddRMSampleByRequestID = AddRMSampleALL.Where(x => x.REQUESTID == RequestID).FirstOrDefault();

                    if (AddRMSampleByRequestID != null)
                    {
                        var RMReqSampleHeadRepo = new GenericRepository<S2EMaterialRequestSampleHead_TB>(unitOfWork.Transaction);
                        var RMReqSampleHeadALL = await RMReqSampleHeadRepo.GetAllAsync();
                        var RMReqSampleHeadByAddRMSample = RMReqSampleHeadALL.Where(x => x.ADDRMSAMPLEID == AddRMSampleByRequestID.ID).FirstOrDefault();

                        if (RMReqSampleHeadByAddRMSample != null)
                        {
                            var RMReqSampleLineRepo = new GenericRepository<S2EMaterialRequestSampleLine_TB>(unitOfWork.Transaction);
                            var RMReqSampleLineALL = await RMReqSampleLineRepo.GetAllAsync();
                            var RMReqSampleLineByReqID = RMReqSampleLineALL.Where(x => x.RMREQSAMID == RMReqSampleHeadByAddRMSample.ID &&
                                                                               x.ISACTIVE == 1 &&
                                                                               x.APPROVESTATUS != 2);
                            decimal QtyUse = 0;
                            if (RMReqSampleLineByReqID != null)
                            {
                                foreach (var MaterialReqLineQTY in RMReqSampleLineByReqID)
                                {
                                    QtyUse += MaterialReqLineQTY.QTY;
                                }

                                Qty = NewRequestByID.QTY - QtyUse;
                            }
                        }

                    }
                }
                

                unitOfWork.Complete();
            }
        }
        public async Task<JsonResult> OnPostPONoGridAsync(string VendorID,string ItemID,int isVendor)
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
        public async Task<IActionResult> OnPostAsync(int PCSampleID, string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                PurchaseSampleID = PCSampleID;
                await GetData(PCSampleID);
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
                                                                   x.ISADDRM == 1).FirstOrDefault();
                    if (approveMapByCreateBy == null)
                    {
                        PurchaseSampleID = PCSampleID;
                        await GetData(PCSampleID);

                        AlertError = "ไม่มีสายอนุมัติ Approve Plant นี้";
                        return Redirect($"/S2E/Purchase/AddRawMaterial/{PCSampleID}/Create");

                    }

                    //UPDATE Purchase Sample STATUS SUCCESS
                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                    var RequestID = PCSampleByID.REQUESTID;
                    var AssessmentID = PCSampleByID.ASSESSMENTID;
                    var LABID = PCSampleByID.LABID;

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

                    if (LABTestLineByID.ISPURCHASESAMPLE == 1 && PONo == null)
                    {
                        PurchaseSampleID = PCSampleID;
                        await GetData(PCSampleID);

                        AlertError = "กรุณาใส่ PO No. ก่อน";
                        return Redirect($"/S2E/Purchase/AddRawMaterial/{PCSampleID}/Create");
                    }

                    PCSampleByID.APPROVESTATUS = RequestStatusModel.Successfully;
                    PCSampleByID.COMPLETEBY = CreateBy;
                    PCSampleByID.COMPLETEDATE = CreateDate;
                    await PCSampleRepo.UpdateAsync(PCSampleByID);

                   

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

                    //INSERT ADD RAW MATERIAL HEAD
                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadInsert = new S2EAddRawMaterialHead_TB
                    {
                        REQUESTID = RequestID,
                        ASSESSMENTID = AssessmentID,
                        LABID = LABID,
                        LABLINEID = LABTestLineByID.ID,
                        PCSAMPLEID = PCSampleID,
                        CURRENCYCODE = CurrencyCode,
                        PLANT = Plant,
                        ISACTIVE = 1,
                        VENDORID = VendorID,
                        SUPPLIERNAME = SupplierName
                    };
                    var AddRMID = await AddRMHeadRepo.InsertAsync(AddRMHeadInsert);
                    //INSERT ADD RAW MATERIAL LINE
                    var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                    var AddRMLineInsert = new S2EAddRawMaterialLine_TB
                    {
                        ADDRMID = (int)AddRMID,
                        REQUESTDATE = CreateDate,
                        PONO = PONo,
                        QTY = Qty,
                        QTYPO = QtyPO,
                        PRICE = Price,
                        APPROVEMASTERID = approvemasterid,
                        APPROVEGROUPID = ApproveGroupID,
                        CURRENTAPPROVESTEP = 1,
                        APPROVESTATUS = ApproveStatus,
                        ISACTIVE = 1,
                        CREATEBY = CreateBy,
                        CREATEDATE = CreateDate,
                        ISCURRENTLOGS = 1,
                        ISPURCHASESAMPLE = LABTestLineByID.ISPURCHASESAMPLE
                    };
                    var AddRMLineID = await AddRMLineRepo.InsertAsync(AddRMLineInsert);

                    //UPLOAD FILE & INSERT ADD RAW MATERIAL LOG FILE
                    var RequestCodefilePath = "S2E_" + RequestCode.Substring(4, 3) + "_" +
                       RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2);
                    int row = FileUpload.Count();
                    string basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/AddRawMaterial/{(int)AddRMLineID}";
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
                            //fileok.Add($"{basePath}/{fileName}");
                            using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                            {
                                await FileUpload[i].CopyToAsync(stream);
                                await AddRMLogsFileRepo.InsertAsync(new S2EAddRawMaterialLogsFile_TB
                                {
                                    ADDRMID = (int)AddRMID,
                                    ADDRMLINEID = (int)AddRMLineID,
                                    FILENAME = fileName,
                                    CREATEBY = CreateBy,
                                    CREATEDATE = CreateDate,
                                    ISACTIVE = 1
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
                                ADDRMID = (int)AddRMID,
                                ADDRMLINEID = (int)AddRMLineID,
                                APPROVEMASTERID = AppFlow.ApproveMasterId,
                                APPROVEGROUPID = ApproveGroupID,
                                EMAIL = AppFlow.Email,
                                APPROVELEVEL = AppFlow.ApproveLevel,
                                ISCURRENTAPPROVE = 1,
                                ISKEYINWHENAPPROVE = AppFlow.IsKeyinWhenApprove
                            });
                        }

                        var currentRecord = await AddRMLineRepo.GetAsync((int)AddRMLineID);
                        currentRecord.CURRENTAPPROVESTEP = 1;
                        await AddRMLineRepo.UpdateAsync(currentRecord);

                        //GET APPROVE TRANS LEVEL 1
                        var AppTransByAddRMID = await unitOfWork.S2EControl.GetApproveTransByAddRMID((int)AddRMID, approvemasterid, ApproveGroupID,(int)AddRMLineID);
                        var AppTransLevel1 = AppTransByAddRMID.Where(x => x.APPROVELEVEL == 1);

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
                                            <td style='text-align:right;'>ตัวแทนจำหน่าย : </td>
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

                    AlertSuccess = "สร้างใบร้องขอเพิ่มวัตถุดิบเข้าระบบสำเร็จ";
                    return Redirect("/S2E/Purchase/AddRawMaterial/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Purchase/AddRawMaterial/{PCSampleID}/Create");
            }
        }
    }
}
