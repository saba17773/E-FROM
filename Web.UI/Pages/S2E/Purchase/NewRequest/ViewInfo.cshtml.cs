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

namespace Web.UI.Pages.S2E.Purchase.NewRequest
{
    public class ViewInfoModel : PageModel
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
        public string PageBack { get; set; }
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

        public ViewInfoModel(
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
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_NEWREQUEST));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
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
                return Redirect("/S2E/Purchase");
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

                UnitMaster = await GetUnitMaster();

                Qty = NewRequestByID.QTY;
                ApproveRemark = NewRequestByID.REMARK;
                Unit = NewRequestByID.UNIT;
                Plant = NewRequestByID.PLANT;
               
                ApproveStatus = NewRequestByID.APPROVESTATUS;
                if (NewRequestByID.APPROVESTATUS == 5 || NewRequestByID.APPROVESTATUS == 7)
                {
                    ApproveStatus1 = true;
                    ApproveStatus2 = false;
                    ApproveStatus3 = false;
                }
                else if(NewRequestByID.APPROVESTATUS == 4)
                {
                    ApproveStatus1 = false;
                    ApproveStatus2 = true;
                    ApproveStatus3 = false;
                }
                else if (NewRequestByID.APPROVESTATUS == 9)
                {
                    ApproveStatus1 = false;
                    ApproveStatus2 = false;
                    ApproveStatus3 = true;
                }
                else
                {
                    ApproveStatus1 = false;
                    ApproveStatus2 = false;
                    ApproveStatus3 = false;
                }

                if (NewRequestByID.APPROVESTATUS == 5 || NewRequestByID.APPROVESTATUS == 7)
                {
                    PageBack = "MainSuccess";
                }
                else if (NewRequestByID.APPROVESTATUS == 2)
                {
                    PageBack = "MainCancel";
                }
                else
                {
                    PageBack = "Main";
                }


                unitOfWork.Complete();
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
