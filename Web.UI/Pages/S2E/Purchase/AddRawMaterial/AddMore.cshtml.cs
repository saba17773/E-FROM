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
    public class AddMoreModel : PageModel
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
        public int AddRMID { get; set; }
        [BindProperty]
        public decimal QtyPO { get; set; }
        [BindProperty]
        public string CurrencyCode { get; set; }
        [BindProperty]
        public string Plant { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public AddMoreModel(
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
        public async Task<IActionResult> OnGetAsync(int ADDRMID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_ADDRAWMATERIAL));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    AddRMID = ADDRMID;
                    await GetData(ADDRMID);

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

                var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                var AddRMHeadByID = await AddRMHeadRepo.GetAsync(AddRMID);

                var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                var AddRMLine = AddRMLineALL.Where(x => x.ADDRMID == AddRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                var RequestID = AddRMHeadByID.REQUESTID;
                var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                var LABID = AddRMHeadByID.LABID;
                var PCSampleID = AddRMHeadByID.PCSAMPLEID;

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

                isPurchaseSample = 1;

                var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

                VendorID = AddRMHeadByID.VENDORID;
                SupplierName = AddRMHeadByID.SUPPLIERNAME;
                CurrencyCode = AddRMHeadByID.CURRENCYCODE;
                Plant = AddRMHeadByID.PLANT;

                ItemCode = PCSampleByID.ITEMCODE;
                ItemName = PCSampleByID.ITEMNAME;

                unitOfWork.Complete();
            }
        }
        public async Task<JsonResult> OnPostPONoGridAsync(string VendorID, string ItemID)
        {
            try
            {
                //AXCust
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
        public async Task<IActionResult> OnPostAsync(int ADDRMID, string draft, string save)
        {
            if (!ModelState.IsValid)
            {
                AddRMID = ADDRMID;
                await GetData(ADDRMID);

                return Page();
            }

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    if (PONo == null)
                    {
                        AlertError = "��س���� PO No. ��͹";
                        return Redirect($"/S2E/Purchase/AddRawMaterial/{ADDRMID}/AddMore");
                    }

                    var CreateDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;

                    var AddRMHeadRepo = new GenericRepository<S2EAddRawMaterialHead_TB>(unitOfWork.Transaction);
                    var AddRMHeadByID = await AddRMHeadRepo.GetAsync(ADDRMID);

                    //GET APPROVE MASTER ID FROM CREATEBY
                    var approveMapRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var approveMapALL = await approveMapRepo.GetAllAsync();
                    var approveMapByCreateBy = approveMapALL.Where(x => x.CreateBy == CreateBy &&
                                                                   x.STEP == 1 &&
                                                                   x.ISADDRM == 1 &&
                                                                   x.PLANT == AddRMHeadByID.PLANT).FirstOrDefault();

                    if (approveMapByCreateBy == null)
                    {
                        ADDRMID = AddRMID;
                        await GetData(AddRMID);

                        AlertError = "��������͹��ѵ� Approve Plant ���";
                        return Redirect($"/S2E/Purchase/AddRawMaterial/{ADDRMID}/Edit");

                    }

                    var AddRMLineRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                    var AddRMLineALL = await AddRMLineRepo.GetAllAsync();
                    var AddRMLineOLDLogs = AddRMLineALL.Where(x=>x.ADDRMID == ADDRMID && x.ISCURRENTLOGS == 1);

                    //UPDATE ADD RM LINE
                    foreach (var updateOLDLogs in AddRMLineOLDLogs)
                    {
                        var RMLineOldUpdate = await AddRMLineRepo.GetAsync(updateOLDLogs.ID);
                        RMLineOldUpdate.ISCURRENTLOGS = 0;
                        await AddRMLineRepo.UpdateAsync(RMLineOldUpdate);

                    }

                    var RequestID = AddRMHeadByID.REQUESTID;
                    var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                    var LABID = AddRMHeadByID.LABID;
                    var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSampleID);

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

                    var AddRMLineNewRepo = new GenericRepository<S2EAddRawMaterialLine_TB>(unitOfWork.Transaction);
                    var AddRMLineNewALL = await AddRMLineNewRepo.GetAllAsync();

                    //INSERT ADD RAW MATERIAL LINE
                    var AddRMLineInsert = new S2EAddRawMaterialLine_TB
                    {
                        ADDRMID = ADDRMID,
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
                        ISPURCHASESAMPLE = 1
                    };
                    var AddRMLineID = await AddRMLineNewRepo.InsertAsync(AddRMLineInsert);

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
                                    ADDRMID = ADDRMID,
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
                                ADDRMID = ADDRMID,
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
                        var AppTransByAddRMID = await unitOfWork.S2EControl.GetApproveTransByAddRMID(ADDRMID, approvemasterid, ApproveGroupID,(int)AddRMLineID);
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
                                  $"{RequestCode} / ���������˹��§ҹ Store �Ѻ��Һ ��ШѴ���ѵ�شԺ������ҧ",
                                   $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> ��������´����� </b><br/>
                                    <table style='text-align:left;'>
                                        <tr>
                                            <td style='text-align:right;'>Request Code : </td>
                                            <td>{NewRequestByID.REQUESTCODE}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>���ʼ����/����Ե : </td>
                                            <td>{AddRMHeadByID.VENDORID}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>���᷹��˹��� : </td>
                                            <td colspan='3'>{AddRMHeadByID.SUPPLIERNAME}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>�������ͧ���᷹��˹��� : </td>
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
                                            <td style='text-align:right;'>�Ҥ� : </td>
                                            <td>{String.Format("{0:#,##0.#0}", Price)}  {NewRequestByID.CURRENCYCODE}</td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>�����ѵ�شԺ�����������? : </td>
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
                                            <td style='text-align:right;'>�ӹǹ : </td>
                                            <td>{String.Format("{0:#,##0.#0}", Qty)}</td>
                                            <td style='text-align:right;'>˹��� : </td>
                                            <td>{NewRequestByID.UNIT}</td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:right;'>��������ͺ : </td>
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
                                            <td style='text-align:right;'>ʶҹ���Ѵ�� : </td>
                                            <td> {AddRMHeadByID.PLANT} </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link ���ʹ��Թ���:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/AddRawMaterialTodolist?Email={AppTrans.EMAIL}'>  ��ԡ����� </a> <br/>
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

                    AlertSuccess = "���ҧ���ͧ�������ѵ�شԺ����к������";
                    return Redirect("/S2E/Purchase/AddRawMaterial/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Purchase/AddRawMaterial/{AddRMID}/AddMore");
            }
        }
    }
}
