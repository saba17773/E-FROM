using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Purchase.AddRawMaterial
{
    public class ViewInfoModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int PurchaseSampleID { get; set; }
        [BindProperty]
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
        public int ADDRMID { get; set; }
        [BindProperty]
        public int ADDRMLineID { get; set; }
        [BindProperty]
        public DateTime? RequestDate { get; set; }
        [BindProperty]
        public decimal QtyPO { get; set; }
        public string PageBack { get; set; }
        [BindProperty]
        public string CurrencyCode { get; set; }
        [BindProperty]
        public string Plant { get; set; }

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
        public async Task<IActionResult> OnGetAsync(int AddRMID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_ADDRAWMATERIAL));

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
                return Redirect("/S2E/Purchase");
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
                var AddRMLine = AddRMLineALL.Where(x=>x.ADDRMID == AddRMID && x.ISCURRENTLOGS == 1).FirstOrDefault();
                var AddRMLineByID = await AddRMLineRepo.GetAsync(AddRMLine.ID);

                ADDRMLineID = AddRMLine.ID;

                var RequestID = AddRMHeadByID.REQUESTID;
                var AssessmentID = AddRMHeadByID.ASSESSMENTID;
                var LABID = AddRMHeadByID.LABID;
                var PCSampleID = AddRMHeadByID.PCSAMPLEID;

                RequestDate = AddRMLineByID.REQUESTDATE;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;
                Dealer = NewRequestByID.DEALER;
                ProductionSite = NewRequestByID.PRODUCTIONSITE;
                DealerAddress = NewRequestByID.DEALERADDRESS;
                Price = NewRequestByID.PRICE;
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

                VendorID = AddRMHeadByID.VENDORID;
                SupplierName = AddRMHeadByID.SUPPLIERNAME;
                CurrencyCode = AddRMHeadByID.CURRENCYCODE;
                Plant = AddRMHeadByID.PLANT;

                ItemCode = PCSampleByID.ITEMCODE;
                ItemName = PCSampleByID.ITEMNAME;

                QtyPO = AddRMLineByID.QTYPO;
                Qty = AddRMLineByID.QTY;
                PONo = AddRMLineByID.PONO;

                if (AddRMLineByID.APPROVESTATUS == 2)
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
                        RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2) + "/AddRawMaterial/"+ ADDRMLineID;

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
    }
}
