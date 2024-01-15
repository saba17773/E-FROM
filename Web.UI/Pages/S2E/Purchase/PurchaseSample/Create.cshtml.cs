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
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Purchase.PurchaseSample
{
    public class CreateModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int LabID { get; set; }
        [BindProperty]
        public string VendorID { get; set; }
        [BindProperty]
        public string SupplierName { get; set; }
        [BindProperty]
        public int isPurchaseSample { get; set; }
        [BindProperty]
        public string RequestNo { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public int RequestBy { get; set; }
        [BindProperty]
        public int isVendor { get; set; }
        [BindProperty]
        public decimal Qty { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        //[BindProperty]
        //public int isItem { get; set; }
        [BindProperty]
        public List<IFormFile> FileUpload { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public decimal QtyTotal { get; set; }

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

        public async Task<IActionResult> OnGetAsync(int LABID)
        {
            try
            {
               
                await _authService.CanAccess(nameof(S2EPermissionModel.MANAGE_PURCHASESAMPLE));
                LabID = LABID;
                await GetData(LABID);

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/PurchaseSample/Main");
            }
        }
        public async Task GetData(int LABID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                Qty = LABTestLineByID.QTY;
                isPurchaseSample = LABTestLineByID.ISPURCHASESAMPLE == 1 ? 1 : 2;

                var RequestID = LABTestHeadByID.REQUESTID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;
                VendorID = NewRequestByID.VENDORID;
                SupplierName = NewRequestByID.DEALER;
                Unit = NewRequestByID.UNIT;
                ItemCode = NewRequestByID.ITEMCODE;
                ItemName = NewRequestByID.ITEMNAME;

                var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                var AddRMSampleALL = await AddRMSampleRepo.GetAllAsync();
                var AddRMSampleByRequestID = AddRMSampleALL.Where(x=> x.REQUESTID == RequestID).FirstOrDefault();

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

                            QtyTotal = NewRequestByID.QTY - QtyUse;
                        }
                    }

                }

                var AccountNum = NewRequestByID.VENDORID;
                var VendorAXByAccountNum = await unitOfWork.Transaction.Connection.QueryAsync<S2EMasterAX_Vendor_TB>(@"
                        SELECT VENDGROUP,ACCOUNTNUM,NAME,DSG_VENDORTYPE
                            FROM TB_MasterAX_Vendor
                            WHERE DATAAREAID = 'DV' AND ACCOUNTNUM = @AccountNum
                        ",
                        new
                        {
                            @AccountNum = AccountNum
                        }
                        , unitOfWork.Transaction);

                if (VendorAXByAccountNum.Count() == 0)
                {
                    isVendor = 0;
                }
                else
                {
                    isVendor = 1;
                }

                //var itmCode = NewRequestByID.ITEMCODE;
                //var ItemByitmCode = await unitOfWork.Transaction.Connection.QueryAsync<S2EMasterAX_Item_TB>(@"
                //        SELECT ITEMID,DATAAREAID,ITEMGROUPID,ITEMNAME
                //        FROM TB_MasterAX_Item 
                //        WHERE DATAAREAID = 'DV' AND ITEMID = @itmCode
                //        ",
                //        new
                //        {
                //            @itmCode = itmCode
                //        }
                //        , unitOfWork.Transaction);

                //if (ItemByitmCode.Count() == 0)
                //{
                //    isItem = 0;
                //}
                //else
                //{
                //    isItem = 1;
                //}

                unitOfWork.Complete();
            }
        }
        public async Task<JsonResult> OnPostVendorAXGridAsync()
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

                    var filter = _datatableService.Filter(Request, field);

                    var VendorAXByGroup = await unitOfWork.Transaction.Connection.QueryAsync<S2EMasterAX_Vendor_TB>(@"
                        SELECT VENDGROUP,ACCOUNTNUM,NAME,DSG_VENDORTYPE
                            FROM TB_MasterAX_Vendor
                            WHERE DATAAREAID = 'DV' AND DSG_VENDORTYPE = 1
                            AND " + filter + @" 
                        ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, VendorAXByGroup.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<JsonResult> OnPostItemGridAsync()
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

                    var filter = _datatableService.Filter(Request, field);

                    var ItemSample = await unitOfWork.Transaction.Connection.QueryAsync<S2EMasterAX_Item_TB>(@"
                       SELECT ITEMID,ITEMNAME
                       FROM TB_MasterAX_Item
                       where ITEMID LIKE 'S%'
                            AND " + filter + @" 
                        ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, ItemSample.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<IActionResult> OnPostAsync(int LABID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //if (FileUpload.Count() == 0)
            //{
            //    AlertError = "กรุณา Upload File ก่อน !!";
            //    return Redirect($"/S2E/Purchase/PurchaseSample/{LABID}/Create");
            //}

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
                                                                   x.ISPURCHASESAMPLE == 1 &&
                                                                   x.PLANT == Plant
                                                              ).FirstOrDefault();

                    if (approveMapByCreateBy == null)
                    {
                        LabID = LABID;
                        await GetData(LABID);

                        AlertError = "ไม่มีสายอนุมัติ Approve Plant นี้";
                        return Redirect($"/S2E/Purchase/PurchaseSample/{LABID}/Create");

                    }


                    //UPDATE LABTest STATUS SUCCESS
                    var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                    var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);

                    var RequestID = LABTestHeadByID.REQUESTID;
                    var AssessmentID = LABTestHeadByID.ASSESSMENTID;

                    var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                    var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                    var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                    LABTestLineByID.APPROVESTATUS = RequestStatusModel.Successfully;
                    LABTestLineByID.COMPLETEBY = CreateBy;
                    LABTestLineByID.COMPLETEDATE = CreateDate;
                    await LABTestLineRepo.UpdateAsync(LABTestLineByID);

                    

                    var PurchaseSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    

                    var approvemasterid = approveMapByCreateBy.APPROVEMASTERID;
                    var ApproveGroupID = approveMapByCreateBy.APPROVEGROUPID;


                    //INSERT NEW REQUEST
                    var insertPCSample = new S2EPurchaseSample_TB
                    {
                        REQUESTID = RequestID,
                        ASSESSMENTID = AssessmentID,
                        LABID = LABID,
                        LABLINEID = LABTestLineByID.ID,
                        VENDORID = VendorID,
                        SUPPLIERNAME = SupplierName,
                        ITEMCODE = ItemCode,
                        ITEMNAME = ItemName,
                        PLANT = Plant,
                        APPROVEMASTERID = approvemasterid,
                        APPROVEGROUPID = ApproveGroupID,
                        APPROVESTATUS = RequestStatusModel.Complete,
                        CREATEBY = CreateBy,
                        CREATEDATE = CreateDate
                    };

                    var PCSampleID = await PurchaseSampleRepo.InsertAsync(insertPCSample);

                    //UPLOAD FILE & INSERT LOG FILE
                    var RequestCodefilePath = "S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                       NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2);
                    int row = FileUpload.Count();
                    string basePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/PurchaseSample";
                    if (!System.IO.Directory.Exists(basePath))
                    {
                        System.IO.Directory.CreateDirectory(basePath);
                    }
                    //var fileok = new List<string>();
                    var filePath = Path.GetTempFileName();
                    string fileName = "";
                    var S2ELogsFileRepo = new GenericRepository<S2EPurchaseSampleLogsFile_TB>(unitOfWork.Transaction);
                    for (int i = 0; i < row; i++)
                    {
                        if (FileUpload[i] != null)
                        {
                            fileName = Path.GetFileName(FileUpload[i].FileName);
                            using (var stream = System.IO.File.Create($"{basePath}/{fileName}"))
                            {
                                await FileUpload[i].CopyToAsync(stream);
                                await S2ELogsFileRepo.InsertAsync(new S2EPurchaseSampleLogsFile_TB
                                {
                                    PCSAMPLEID = (int)PCSampleID,
                                    FILENAME = fileName,
                                    CREATEBY = CreateBy,
                                    CREATEDATE = CreateDate
                                });
                            }

                        }
                    }


                    if (LABTestLineByID.ISPURCHASESAMPLE == 1)
                    {
                        //GET Fullname CreateBy
                        var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                        var UserALL = await UserRepo.GetAsync(CreateBy);
                        var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                        var EmpALL = await EmpRepo.GetAllAsync();
                        var EmpByEmpID = EmpALL.Where(x => x.EmployeeId == UserALL.EmployeeId).FirstOrDefault();
                        var FName = EmpByEmpID.Name;
                        var LName = EmpByEmpID.LastName;
                        var CreateByFullName = FName + " " + LName;

                        //GET ApproveFlow
                        var AppFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                        var AppFlowALL = await AppFlowRepo.GetAllAsync();
                        var AppFlowByAppMasterID = AppFlowALL.Where(x => x.ApproveMasterId == approvemasterid & x.IsActive == 1);

                        // INSERT PC APPROVE TRANSECTION
                        var PCSampleLogsSendmailRepo = new GenericRepository<S2EPurchaseSampleLogsSendEmail_TB>(unitOfWork.Transaction);
                        foreach (var AppFlow in AppFlowByAppMasterID)
                        {
                            await PCSampleLogsSendmailRepo.InsertAsync(new S2EPurchaseSampleLogsSendEmail_TB
                            {
                                PCSAMPLEID = (int)PCSampleID,
                                EMAIL = AppFlow.Email,
                                SENDEMAILBY = CreateBy,
                                SENDEMAILDATE = CreateDate,
                                APPROVEGROUPID = ApproveGroupID,
                                APPROVEMASTERID = approvemasterid,
                                ISLASTSENDEMAIL = 1
                            });
                        }

                        
                        //GET Email in ApproveTrans
                        var AppTransByRequestID = await unitOfWork.S2EControl.GetLogsSendEmailByPCSampleID((int)PCSampleID, approvemasterid, ApproveGroupID);
                        var AppTransALL = AppTransByRequestID.Where(x => x.ISLASTSENDEMAIL == 1);
                        foreach (var AppTrans in AppTransALL)
                        {
                            var BodyEmail = "";
                            BodyEmail = $@"
                                <b> ข้อมูลที่ต้องการให้เปิด PR  </b><br/>
                                <table width='70%'>
                                    <tr style='vertical-align: top;'>
                                        <td>
                                            <b> รหัสผู้ขาย/ผู้ผลิต :</b> {VendorID}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td>
                                            <b> ตัวแทนจำหน่าย :</b> {SupplierName}
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
                                            <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)}  {NewRequestByID.CURRENCYCODE}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td>
                                            <b> จำนวนที่ต้องการซื้อเพิ่ม :</b> {String.Format("{0:#,##0.#0}", LABTestLineByID.QTY)}  {NewRequestByID.UNIT}
                                        </td>
                                    </tr>
                                </table>
                                <br/><br/>
                                <a href='{_configuration["Config:BaseUrl"]}/S2E/PurchaseSampleViewInfo/?PCSampleID={PCSampleID}&EMAILAPPROVE={AppTrans.EMAIL}'> คลิกที่นี่ เพื่อดูรายละเอียด </a>
                            ";

                            var sendEmail = _emailService.SendEmail(
                                  $"{RequestCode} / ทำการเปิด PR ได้",
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

                    AlertSuccess = "Purchase Sample Success.";
                    return Redirect("/S2E/Purchase/PurchaseSample/Main");
                }

            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/Purchase/PurchaseSample/{LABID}/Create");
            }
        }
    }
}
