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

namespace Web.UI.Pages.S2E.Purchase.PurchaseSample
{
    public class ViewInfoModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int PCSampleID { get; set; }
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
        public decimal Qty { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public string ItemCode { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public decimal QtyTotal { get; set; }

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
        public async Task<IActionResult> OnGetAsync(int PCSAMPLEID)
        {
            try
            {

                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_PURCHASESAMPLE));
                PCSampleID = PCSAMPLEID;
                await GetData(PCSAMPLEID);

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase");
            }
        }
        public async Task GetData(int PCSAMPLEID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                var PCSampleByID = await PCSampleRepo.GetAsync(PCSAMPLEID);

                var LABID = PCSampleByID.LABID;
                var LABTestHeadRepo = new GenericRepository<S2ELABTestHead_TB>(unitOfWork.Transaction);
                var LABTestHeadByID = await LABTestHeadRepo.GetAsync(LABID);
                var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);
                var LABTestLineALL = await LABTestLineRepo.GetAllAsync();
                var LABTestLineByID = LABTestLineALL.Where(x => x.LABID == LABID && x.ISCURRENTLOGS == 1).FirstOrDefault();

                RequestNo = LABTestLineByID.PROJECTREFNO;
                Qty = LABTestLineByID.QTY;
                isPurchaseSample = LABTestLineByID.ISPURCHASESAMPLE == 1 ? 1 : 2;

                var RequestID = PCSampleByID.REQUESTID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;
                Unit = NewRequestByID.UNIT;

                VendorID = PCSampleByID.VENDORID;
                SupplierName = PCSampleByID.SUPPLIERNAME;
                ItemCode = PCSampleByID.ITEMCODE;
                ItemName = PCSampleByID.ITEMNAME;
                Plant = PCSampleByID.PLANT;

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

                            QtyTotal = NewRequestByID.QTY - QtyUse;
                        }
                    }

                }


                unitOfWork.Complete();
            }
        }

        public async Task<IActionResult> OnPostGridViewFileUploadAsync(int PCSAMPLEID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EPurchaseSampleLogsFileGridViewModel>($@"
                           SELECT ID,
		                    PCSAMPLEID,
		                    FILENAME,
                            CREATEBY,
		                    CONVERT(NVARCHAR,CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,CREATEDATE,108) AS CREATEDATE
                    FROM TB_S2EPurchaseSampleLogsFile 
                    WHERE PCSAMPLEID = {PCSAMPLEID}
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
        public async Task<IActionResult> OnGetDownloadFileUploadAsync(int PCSAMPLEID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var PurchaseSampleLogFileRepo = new GenericRepository<S2EPurchaseSampleLogsFile_TB>(unitOfWork.Transaction);
                    var PurchaseSampleLogFileByID = await PurchaseSampleLogFileRepo.GetAsync(Fileid);

                    var PCSampleRepo = new GenericRepository<S2EPurchaseSample_TB>(unitOfWork.Transaction);
                    var PCSampleByID = await PCSampleRepo.GetAsync(PCSAMPLEID);

                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(PCSampleByID.REQUESTID);

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + NewRequestByID.REQUESTCODE.Substring(4, 3) + "_" +
                        NewRequestByID.REQUESTCODE.Substring(8, 2) + "_" + NewRequestByID.REQUESTCODE.Substring(11, 2) + "/PurchaseSample";

                    var fileName = PurchaseSampleLogFileByID.FILENAME;

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
