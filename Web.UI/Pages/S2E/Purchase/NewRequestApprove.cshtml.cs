using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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

namespace Web.UI.Pages.S2E.Purchase
{
    public class NewRequestApproveModel : PageModel
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
        public string Requestdate { get; set; }
        [BindProperty]
        public decimal Qty { get; set; }
        [BindProperty]
        public int ApproveStatus { get; set; }
        [BindProperty]
        public string ApproveRemark { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public int isKeyin { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        public List<SelectListItem> UnitMaster { get; set; }
        [BindProperty]
        public bool ApproveStatus1 { get; set; }
        [BindProperty]
        public bool ApproveStatus2 { get; set; }
        [BindProperty]
        public bool ApproveStatus3 { get; set; }
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

        public NewRequestApproveModel(
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
        public async Task<IActionResult> OnGetAsync(int RequestID, int TranID, string nonce, string email, int isKeyinWhenApprove)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var _nonce = await unitOfWork.S2EControl.GetNewRequestNonceByKey(nonce);

                    if (_nonce.ISUSED == 1)
                    {
                        AlertError = "Link Is Used.";
                        return Redirect($"/S2E/Purchase/NewRequestTodolist?Email={email}");
                    }

                    CurrencyCodeMaster = await GetCurrencyCodeMaster();
                    CurrencyCodeREFMaster = await GetCurrencyCodeREFMaster();
                    PerUnitMaster = await GetPerUnitMaster();
                    PerUnitREFMaster = await GetPerUnitREFMaster();

                    RequestId = RequestID;
                    isKeyin = isKeyinWhenApprove;
                    await GetData(RequestID);

                    UnitMaster = await GetUnitMaster();

                    if (isKeyin == 0)
                    {
                        await GetQty(RequestID);
                    }

                    return Page();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task GetData(int RequestID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                Requestcode = NewRequestByID.REQUESTCODE;
                Requestdate = Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss");
                VendorID = NewRequestByID.VENDORID;
                SupplierName = NewRequestByID.SUPPLIERNAME;
                Dealer = NewRequestByID.DEALER;
                ProductionSite = NewRequestByID.PRODUCTIONSITE;
                DealerAddress = NewRequestByID.DEALERADDRESS;
                ItemCode = NewRequestByID.ITEMCODE;
                ItemName = NewRequestByID.ITEMNAME;
                Price = NewRequestByID.PRICE;
                CurrencyCode = NewRequestByID.CURRENCYCODE;
                PerUnit = NewRequestByID.PERUNIT;
                Process = NewRequestByID.ISCOMPAIRE == 1 ? "1" : "0";

                if (NewRequestByID.ISCOMPAIRE == 1)
                {
                    var NewRequestCompireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                    var NewRequestCompireALL = await NewRequestCompireRepo.GetAllAsync();
                    var NewRequestCompireByRequestID = NewRequestCompireALL.Where(x => x.REQUESTID == RequestID).FirstOrDefault();

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
        public async Task GetQty(int RequestID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                Qty = NewRequestByID.QTY;
                ApproveRemark = NewRequestByID.REMARK;
                Unit = NewRequestByID.UNIT;
                Plant = NewRequestByID.PLANT;

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
        public async Task<List<SelectListItem>> GetUnitMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var UnitRepo = new GenericRepository<Unit_TB>(unitOfWork.Transaction);

                var UnitALL = await UnitRepo.GetAllAsync();

                return UnitALL.Select(x => new SelectListItem
                {
                    Value = x.UNIT,
                    Text = x.UNIT
                })
                    .ToList();
            }
        }
        public async Task<IActionResult> OnPostAsync(int RequestID, int TranID, string nonce, string email, int isKeyinWhenApprove)
        {

            try
            {
                if (ApproveStatus == 0)
                {
                    AlertError = "กรุณาเลือกว่าจะอนุมัติ หรือ ไม่อนุมัติ หรือ ขอข้อมูลเพิ่มเติม !!";
                    return Redirect($"/S2E/Purchase/NewRequestApprove?RequestID={RequestID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");
                }
                if (ApproveStatus == 2 && ApproveRemark == null)
                {
                    AlertError = "กรุณาใส่เหตุผล !!";
                    return Redirect($"/S2E/Purchase/NewRequestApprove?RequestID={RequestID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");
                }
                if (ApproveStatus == 3 && ApproveRemark == null)
                {
                    AlertError = "กรุณาใส่เหตุผล !!";
                    return Redirect($"/S2E/Purchase/NewRequestApprove?RequestID={RequestID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");
                }
                if (ApproveStatus == 1 && isKeyinWhenApprove == 1)
                {
                    if (ApproveRemark == null || Plant == null || Qty == 0 || Unit == null)
                    {
                        AlertError = "กรุณาใส่ข้อมูลให้ครบ !!";
                        return Redirect($"/S2E/Purchase/NewRequestApprove?RequestID={RequestID}&TranID={TranID}&nonce={nonce}&email={email}&isKeyinWhenApprove={isKeyinWhenApprove}");
                    }
                }
                if (!ModelState.IsValid)
                {
                    RequestId = RequestID;
                    isKeyin = isKeyinWhenApprove;
                    await GetData(RequestID);

                    UnitMaster = await GetUnitMaster();

                    if (isKeyin == 0)
                    {
                        await GetQty(RequestID);
                    }

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);
                    var DatetimeNow = DateTime.Now;
                    int approvemasterid = NewRequestByID.APPROVEMASTERID;

                    //UPDATE OLD DATA
                    var nonceRepo = new GenericRepository<S2ENewRequestNonce_TB>(unitOfWork.Transaction);
                    var _nonce = await unitOfWork.S2EControl.GetNewRequestNonceByKey(nonce);
                    if (_nonce.ISUSED == 1)
                    {
                        throw new Exception("Link Is Used.");
                    }
                    _nonce.ISUSED = 1;

                    //UPDATE Approve Trans
                    var NewRequestTransRepo = new GenericRepository<S2ENewRequestApproveTrans_TB>(unitOfWork.Transaction);
                    var NewRequestTransByID = await NewRequestTransRepo.GetAsync(TranID);
                    var ApproveLevel = NewRequestTransByID.APPROVELEVEL;
                    var ApproveGroupID = NewRequestTransByID.APPROVEGROUPID;

                    var NewRequestApproveTransRepo = new GenericRepository<S2ENewRequestApproveTrans_TB>(unitOfWork.Transaction);
                    var NewRequestApproveTransALL = await NewRequestApproveTransRepo.GetAllAsync();
                    var NewRequestApproveTranLevel = NewRequestApproveTransALL.Where(x => x.REQUESTID == RequestID &&
                                                                    x.APPROVEMASTERID == approvemasterid &&
                                                                    x.APPROVELEVEL == ApproveLevel &&
                                                                    x.ISCURRENTAPPROVE == 1 &&
                                                                    x.APPROVEGROUPID == ApproveGroupID);

                    foreach (var UpdateApproveTrans in NewRequestApproveTranLevel)
                    {
                        UpdateApproveTrans.ISDONE = 1;
                        if (UpdateApproveTrans.EMAIL == email)
                        {
                            UpdateApproveTrans.REMARK = ApproveRemark;
                            if (ApproveStatus == 1)
                            {
                                UpdateApproveTrans.APPROVEDATE = DatetimeNow;
                            }
                            else if (ApproveStatus == 2 || ApproveStatus == 3 )
                            {
                                UpdateApproveTrans.REJECTDATE = DatetimeNow;
                            }
                        }
                        await NewRequestApproveTransRepo.UpdateAsync(UpdateApproveTrans);
                    }

                    //GET REQUEST BY FULL NAME
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserALL = await UserRepo.GetAsync(NewRequestByID.CREATEBY);

                    //GET APPROVE TRANS ALL LEVEL
                    var ApproveTransAll = await unitOfWork.S2EControl.GetApproveTransByRequestIDAllLevel(RequestID, approvemasterid, ApproveGroupID);
                    var AllLevel = ApproveTransAll.ToList().Count;

                    // CHECK IS FINAL APPROVE?
                    //isFinal
                    if (NewRequestByID.CURRENTAPPROVESTEP == AllLevel && ApproveStatus == 1)
                    {
                        if (NewRequestByID.COMPLETEDATE == null)
                        {
                            //UPDATE PCREQUEST_TB (HEAD TABLE)
                            NewRequestByID.APPROVESTATUS = RequestStatusModel.Complete;

                            if (NewRequestByID.PLANT == null)
                            {
                                NewRequestByID.PLANT = Plant;
                            }
                            if (NewRequestByID.UNIT == null)
                            {
                                NewRequestByID.UNIT = Unit;
                            }
                            if (NewRequestByID.REMARK == null)
                            {
                                NewRequestByID.REMARK = ApproveRemark;
                            }
                            if (NewRequestByID.QTY == 0)
                            {
                                NewRequestByID.QTY = Qty;
                            }

                            //GET EMAIL SUCCESSFULLY
                            var EmailSuccess = new List<string>();
                            var ApproveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                            var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();

                            var ApproveFlowReceiveComplete = ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid &&
                                                                                  x.ReceiveWhenComplete == 1 && x.IsActive == 1);
                            //CASE WHEN SET IN FLOW MASTER
                            if (ApproveFlowReceiveComplete != null)
                            {
                                foreach (var emaillog in ApproveFlowReceiveComplete)
                                {
                                    EmailSuccess.Add(emaillog.Email);
                                }
                            }

                            //Email Requester
                            EmailSuccess.Add(UserALL.Email);

                            var ischeck1 = ""; if (NewRequestByID.PLANT == "DSL") { ischeck1 = "checked"; } else { ischeck1 = ""; }
                            var ischeck2 = ""; if (NewRequestByID.PLANT == "DRB") { ischeck2 = "checked"; } else { ischeck2 = ""; }
                            var ischeck3 = ""; if (NewRequestByID.PLANT == "DSI") { ischeck3 = "checked"; } else { ischeck3 = ""; }
                            var ischeck4 = ""; if (NewRequestByID.PLANT == "DSR") { ischeck4 = "checked"; } else { ischeck4 = ""; }
                            var ischeck5 = ""; if (NewRequestByID.PLANT == "STR") { ischeck5 = "checked"; } else { ischeck5 = ""; }

                            string ALLApproveby = "";
                            var TransALLApproveRepo = new GenericRepository<S2ENewRequestApproveTrans_TB>(unitOfWork.Transaction);
                            var TransALLApproveALL = await TransALLApproveRepo.GetAllAsync();
                            var TransALLApprove = TransALLApproveALL.Where(x => x.REQUESTID == RequestID &&
                                                                                x.ISCURRENTAPPROVE == 1 &&
                                                                                x.APPROVEGROUPID == ApproveGroupID);
                            foreach (var i in TransALLApprove)
                            {
                                ALLApproveby += i.EMAIL + ",";
                            }

                            ALLApproveby = ALLApproveby.Substring(0, ALLApproveby.Length - 1);

                            var BodyEmail = "";
                            if (NewRequestByID.ISCOMPAIRE == 1)
                            {
                                var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                                var NewRequestCompaireALL = await NewRequestCompaireRepo.GetAllAsync();
                                var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == RequestID).FirstOrDefault();

                                BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายละเอียดผู้ขาย/วัตถุดิบ (ที่มีอยู่)  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestCompaireByRequestID.VENDORIDREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestCompaireByRequestID.SUPPLIERNAMEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {NewRequestCompaireByRequestID.DEALERREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {NewRequestCompaireByRequestID.PRODUCTIONSITEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestCompaireByRequestID.DEALERADDRESSREF.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestCompaireByRequestID.ITEMCODEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {NewRequestCompaireByRequestID.ITEMNAMEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}",NewRequestCompaireByRequestID.PRICEREF)} {NewRequestCompaireByRequestID.CURRENCYCODEREF} / {NewRequestCompaireByRequestID.PERUNITREF}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b> รายการวัตถุดิบที่นำเข้า / นำมาเปรียบเทียบ  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.VENDORID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {NewRequestByID.DEALER}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {NewRequestByID.PRODUCTIONSITE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestByID.ITEMCODE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {NewRequestByID.ITEMNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)} {NewRequestByID.CURRENCYCODE} / {NewRequestByID.PERUNIT}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                     <table>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b>Plant : </b>
                                            </td>
                                            <td>
                                                <label>
                                                    <input type = 'checkbox' id = 'Plant1' name = 'Plant1' class='checkbox2' {ischeck1}>
                                                    DSL
                                                </label>
                                            </td>
                                            <td>
                                                <label>
                                                    <input type = 'checkbox' id = 'Plant2' name = 'Plant2' class='checkbox2' {ischeck2}>
                                                    DRB
                                                </label>
                                            </td>
                                            <td>
                                                <label>
                                                    <input type = 'checkbox' id = 'Plant3' name = 'Plant3' class='checkbox2' {ischeck3}>
                                                    DSI
                                                </label>
                                            </td>
                                            <td>
                                                <label>
                                                    <input type = 'checkbox' id = 'Plant4' name = 'Plant4' class='checkbox2' {ischeck4}>
                                                    DSR
                                                </label>
                                            </td>
                                            <td>
                                                <label>
                                                    <input type = 'checkbox' id = 'Plant5' name = 'Plant5' class='checkbox2' {ischeck5}>
                                                    STR
                                                </label>
                                            </td>
                                        </tr>
                                    </table>
                                    <b>ปริมาณที่ต้องการทดสอบ : </b> {String.Format("{0:#,##0.#0}", NewRequestByID.QTY)}  {NewRequestByID.UNIT} <br/>
                                    <b>เหตุผล : </b> {NewRequestByID.REMARK} <br/>
                                    <b>อนุมัติโดย : </b> {ALLApproveby}
                                ";
                            }
                            else
                            {
                                BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายการวัตถุดิบที่นำเข้า / นำมาเปรียบเทียบ  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.VENDORID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {NewRequestByID.DEALER}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {NewRequestByID.PRODUCTIONSITE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestByID.ITEMCODE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {NewRequestByID.ITEMNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)} {NewRequestByID.CURRENCYCODE} / {NewRequestByID.PERUNIT}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                     <table>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b>Plant : </b>
                                            </td>
                                            <td>
                                                <label>
                                                    <input type = 'checkbox' id = 'Plant1' name = 'Plant1' class='checkbox2' {ischeck1}>
                                                    DSL
                                                </label>
                                            </td>
                                            <td>
                                                <label>
                                                    <input type = 'checkbox' id = 'Plant2' name = 'Plant2' class='checkbox2' {ischeck2}>
                                                    DRB
                                                </label>
                                            </td>
                                            <td>
                                                <label>
                                                    <input type = 'checkbox' id = 'Plant3' name = 'Plant3' class='checkbox2' {ischeck3}>
                                                    DSI
                                                </label>
                                            </td>
                                            <td>
                                                <label>
                                                    <input type = 'checkbox' id = 'Plant4' name = 'Plant4' class='checkbox2' {ischeck4}>
                                                    DSR
                                                </label>
                                            </td>
                                            <td>
                                                <label>
                                                    <input type = 'checkbox' id = 'Plant5' name = 'Plant5' class='checkbox2' {ischeck5}>
                                                    STR
                                                </label>
                                            </td>
                                        </tr>
                                    </table>
                                    <b>ปริมาณที่ต้องการทดสอบ : </b> {String.Format("{0:#,##0.#0}", NewRequestByID.QTY)}  {NewRequestByID.UNIT} <br/>
                                    <b>เหตุผล : </b> {NewRequestByID.REMARK} <br/>
                                    <b>อนุมัติโดย : </b> {ALLApproveby}
                                ";
                            }

                            var sendEmail = _emailService.SendEmail(
                                  $"{NewRequestByID.REQUESTCODE} / ต้องการมาทดสอบ / ร้องขอเพื่อทดสอบวัตถุดิบ",
                                  BodyEmail,
                                  EmailSuccess,
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
                    //isReject or More Detail
                    else if ((ApproveStatus == 2 && ApproveRemark != null) || (ApproveStatus == 3 && ApproveRemark != null))
                    {
                        var Subject = "";
                        var Body1 = "";

                        //UPDATE PCREQUEST_TB (HEAD TABLE)
                        if (ApproveStatus == 2)
                        {
                            NewRequestByID.APPROVESTATUS = RequestStatusModel.Reject;
                            Subject = "Reject " + NewRequestByID.REQUESTCODE + " / ไม่ต้องการทดสอบ / ร้องขอเพื่อทดสอบวัตถุดิบ";
                            Body1 = "สาเหตุไม่ต้องการทดสอบ : ";
                        }
                        if (ApproveStatus == 3)
                        {
                            NewRequestByID.APPROVESTATUS = RequestStatusModel.MoreDetail;
                            Subject = "Reject " + NewRequestByID.REQUESTCODE + "/ ต้องการข้อมูลเพิ่มเติม / ร้องขอเพื่อทดสอบวัตถุดิบ";
                            Body1 = "ต้องการข้อมูลเพิ่มเติม : ";
                        }

                        NewRequestByID.PLANT = null;
                        NewRequestByID.UNIT = null;
                        NewRequestByID.REMARK = null;
                        NewRequestByID.QTY = 0;

                        //PREPARE FILE UPLOAD
                        //var fileok = new List<string>();
                        var RequestCode = NewRequestByID.REQUESTCODE;
                        var RequestCodefilePath = "S2E_" + RequestCode.Substring(4, 3) + "_" +
                            RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2);

                        var filePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/NewRequest";

                        var NewRequestLogFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                        var NewRequestLogFileALL = await NewRequestLogFileRepo.GetAllAsync();
                        foreach (var filelog in NewRequestLogFileALL.Where(x => x.REQUESTID == RequestID))
                        {
                            string fileName = filelog.FILENAME;
                        }

                        //GET EMAIL REJECT
                        var EmailReject = new List<string>();
                        //CASE SET IN FLOW
                        var ApproveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);
                        var ApproveFlowALL = await ApproveFlowRepo.GetAllAsync();
                        //foreach (var emaillog in ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid && x.ReceiveWhenFailed == 1 && x.IsActive == 1 && x.ApproveLevel < PCRequestBYID.CURRENTAPPROVESTEP))
                        //{
                        //    EmaiReject.Add(emaillog.Email);
                        //}
                        EmailReject.Add(UserALL.Email);

                        var ApproveByALL = ApproveFlowALL.Where(x => x.ApproveMasterId == approvemasterid &&
                                                                  x.ApproveLevel == NewRequestByID.CURRENTAPPROVESTEP &&
                                                                  x.IsActive == 1);

                        var ApproveByFristName = ApproveByALL.Select(s => s.Name).FirstOrDefault();
                        var ApproveByLastName = ApproveByALL.Select(s => s.LastName).FirstOrDefault();

                        var ApproveBy = ApproveByFristName + " " + ApproveByLastName;

                        var BodyEmail = "";
                        if (NewRequestByID.ISCOMPAIRE == 1)
                        {
                            var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                            var NewRequestCompaireALL = await NewRequestCompaireRepo.GetAllAsync();
                            var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == RequestID).FirstOrDefault();

                            BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายละเอียดผู้ขาย/วัตถุดิบ (ที่มีอยู่)  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestCompaireByRequestID.VENDORIDREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestCompaireByRequestID.SUPPLIERNAMEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {NewRequestCompaireByRequestID.DEALERREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {NewRequestCompaireByRequestID.PRODUCTIONSITEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestCompaireByRequestID.DEALERADDRESSREF.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestCompaireByRequestID.ITEMCODEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {NewRequestCompaireByRequestID.ITEMNAMEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestCompaireByRequestID.PRICEREF)} {NewRequestCompaireByRequestID.CURRENCYCODEREF} / {NewRequestCompaireByRequestID.PERUNITREF}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b> รายการวัตถุดิบที่นำเข้า / นำมาเปรียบเทียบ  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.VENDORID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {NewRequestByID.DEALER}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {NewRequestByID.PRODUCTIONSITE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestByID.ITEMCODE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {NewRequestByID.ITEMNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)} {NewRequestByID.CURRENCYCODE} / {NewRequestByID.PERUNIT}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>{Body1}</b><b style='color:red'>{ApproveRemark}</b><br/>
                                    <b style='color:black'>Reject by : </b>{ApproveBy}
                                ";
                        }
                        else
                        {
                            BodyEmail = $@"
                                <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                <b> รายการวัตถุดิบที่นำเข้า / นำมาเปรียบเทียบ  </b><br/>
                                <table width='70%'>
                                    <tr style='vertical-align: top;'>
                                        <td>
                                            <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.VENDORID}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td>
                                            <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.SUPPLIERNAME}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td>
                                            <b> ตัวแทนจำหน่าย :</b> {NewRequestByID.DEALER}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td>
                                            <b> แหล่งผลิต :</b> {NewRequestByID.PRODUCTIONSITE}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td>
                                            <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}
                                        </td>
                                    </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestByID.ITEMCODE}
                                            </td>
                                        </tr>
                                    <tr style='vertical-align: top;'>
                                        <td>
                                            <b> Item Name :</b> {NewRequestByID.ITEMNAME}
                                        </td>
                                    </tr>
                                    <tr style='vertical-align: top;'>
                                        <td>
                                            <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)} {NewRequestByID.CURRENCYCODE} / {NewRequestByID.PERUNIT}
                                        </td>
                                    </tr>
                                </table>
                                <br/>
                                <b>{Body1}</b><b style='color:red'>{ApproveRemark}</b><br/>
                                <b style='color:black'>Reject by : </b>{ApproveBy}
                            ";
                        }

                        var sendEmail = _emailService.SendEmail(
                            $"{Subject}",
                            BodyEmail,
                            EmailReject,
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
                    //isApprove And Not Final Approve
                    else
                    {
                        //UPDATE PCREQUEST_TB (HEAD TABLE)
                        NewRequestByID.CURRENTAPPROVESTEP += 1;
                        NewRequestByID.APPROVESTATUS = RequestStatusModel.WaitingForApprove;

                        if (NewRequestByID.PLANT == null)
                        {
                            NewRequestByID.PLANT = Plant;
                        }
                        if (NewRequestByID.UNIT == null)
                        {
                            NewRequestByID.UNIT = Unit;
                        }
                        if (NewRequestByID.REMARK == null)
                        {
                            NewRequestByID.REMARK = ApproveRemark;
                        }
                        if (NewRequestByID.QTY == 0)
                        {
                            NewRequestByID.QTY = Qty;
                        }

                        //GENERATE NONCE
                        var nonceKey = Guid.NewGuid().ToString();
                        await nonceRepo.InsertAsync(new S2ENewRequestNonce_TB
                        {
                            NONCEKEY = nonceKey,
                            CREATEDATE = DatetimeNow,
                            EXPIREDATE = DatetimeNow.AddDays(7),
                            ISUSED = 0
                        });

                        //PREPARE FILE UPLOAD
                        // var fileok = new List<string>();
                        var RequestCode = NewRequestByID.REQUESTCODE;
                        var RequestCodefilePath = "S2E_" + RequestCode.Substring(4, 3) + "_" +
                            RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2);

                        var filePath = $"wwwroot/files/S2EFiles/{(string)RequestCodefilePath}/NewRequest";

                        var NewRequestLogFileRepo = new GenericRepository<S2ENewRequestLogsFile_TB>(unitOfWork.Transaction);
                        var NewRequestLogFileALL = await NewRequestLogFileRepo.GetAllAsync();
                        foreach (var filelog in NewRequestLogFileALL.Where(x => x.REQUESTID == RequestID))
                        {
                            string fileName = filelog.FILENAME;
                        }

                        //NEXT APPROVE LEVEL
                        var nextALL = new GenericRepository<S2ENewRequestApproveTrans_TB>(unitOfWork.Transaction);
                        var nextAllLevel = await nextALL.GetAllAsync();
                        var nextLevel = nextAllLevel.Where(x => x.REQUESTID == RequestID &&
                                                            x.APPROVELEVEL == NewRequestByID.CURRENTAPPROVESTEP &&
                                                            x.APPROVEMASTERID == approvemasterid &&
                                                            x.ISCURRENTAPPROVE == 1 &&
                                                            x.APPROVEGROUPID == ApproveGroupID);
                        foreach (var next in nextLevel)
                        {
                            var BodyEmail = "";
                            if (NewRequestByID.ISCOMPAIRE == 1)
                            {
                                var NewRequestCompaireRepo = new GenericRepository<S2ENewRequestCompaire_TB>(unitOfWork.Transaction);
                                var NewRequestCompaireALL = await NewRequestCompaireRepo.GetAllAsync();
                                var NewRequestCompaireByRequestID = NewRequestCompaireALL.Where(x => x.REQUESTID == RequestID).FirstOrDefault();

                                BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายละเอียดผู้ขาย/วัตถุดิบ (ที่มีอยู่)  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestCompaireByRequestID.VENDORIDREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestCompaireByRequestID.SUPPLIERNAMEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {NewRequestCompaireByRequestID.DEALERREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {NewRequestCompaireByRequestID.PRODUCTIONSITEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestCompaireByRequestID.DEALERADDRESSREF.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestCompaireByRequestID.ITEMCODEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {NewRequestCompaireByRequestID.ITEMNAMEREF}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestCompaireByRequestID.PRICEREF)} {NewRequestCompaireByRequestID.CURRENCYCODEREF} / {NewRequestCompaireByRequestID.PERUNITREF}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b> รายการวัตถุดิบที่นำเข้า / นำมาเปรียบเทียบ  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.VENDORID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {NewRequestByID.DEALER}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {NewRequestByID.PRODUCTIONSITE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestByID.ITEMCODE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {NewRequestByID.ITEMNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)} {NewRequestByID.CURRENCYCODE} / {NewRequestByID.PERUNIT}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/NewRequestTodolist?Email={next.EMAIL}'> คลิกที่นี่ </a>
                                ";
                            }
                            else
                            {
                                BodyEmail = $@"
                                    <b> REQUEST DATE :</b> {Convert.ToDateTime(NewRequestByID.REQUESTDATE).ToString("dd/MM/yyyy HH:mm:ss")} <br/><br/>
                                    <b> รายการวัตถุดิบที่นำเข้า / นำมาเปรียบเทียบ  </b><br/>
                                    <table width='70%'>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> รหัสผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.VENDORID}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ชื่อผู้ขาย/ผู้ผลิต :</b> {NewRequestByID.SUPPLIERNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ตัวแทนจำหน่าย :</b> {NewRequestByID.DEALER}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> แหล่งผลิต :</b> {NewRequestByID.PRODUCTIONSITE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ที่อยู่ขอตัวแทนจำหน่าย :</b> {NewRequestByID.DEALERADDRESS.Replace("\n", "<br>")}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Code :</b> {NewRequestByID.ITEMCODE}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> Item Name :</b> {NewRequestByID.ITEMNAME}
                                            </td>
                                        </tr>
                                        <tr style='vertical-align: top;'>
                                            <td>
                                                <b> ราคา :</b> {String.Format("{0:#,##0.#0}", NewRequestByID.PRICE)} {NewRequestByID.CURRENCYCODE} / {NewRequestByID.PERUNIT}
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <b>Link เพื่อดำเนินการ:</b> <a href='{_configuration["Config:BaseUrl"]}/S2E/Purchase/NewRequestTodolist?Email={next.EMAIL}'> คลิกที่นี่ </a>
                                ";
                            }

                            var sendEmail = _emailService.SendEmail(
                                  $"{NewRequestByID.REQUESTCODE} / Request / ร้องขอเพื่อทดสอบวัตถุดิบ",
                                  BodyEmail,
                                  new List<string> { next.EMAIL },
                                  new List<string> { },
                                  "",
                                  "",
                                  new List<string> { }
                            );

                            if (sendEmail.Result == false)
                            {
                                throw new Exception(sendEmail.Message);
                            }

                            var approveTrans_next = await NewRequestTransRepo.GetAsync(next.ID);
                            approveTrans_next.SENDEMAILDATE = DatetimeNow;
                            await NewRequestTransRepo.UpdateAsync(approveTrans_next);

                        }

                    }

                    await NewRequestRepo.UpdateAsync(NewRequestByID);
                    await nonceRepo.UpdateAsync(_nonce);

                    unitOfWork.Complete();
                    AlertSuccess = "Success.";
                    return Redirect($"/S2E/Purchase/NewRequestTodolist?Email={email}");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/");
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
