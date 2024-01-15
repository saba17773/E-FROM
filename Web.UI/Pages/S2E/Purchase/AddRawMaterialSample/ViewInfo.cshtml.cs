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

namespace Web.UI.Pages.S2E.Purchase.AddRawMaterialSample
{
    public class ViewInfoModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int AddRMSampleId { get; set; }
        [BindProperty]
        public decimal Qty { get; set; }
        [BindProperty]
        public string RequestCode { get; set; }
        [BindProperty]
        public string ProjectRefNo { get; set; }
        [BindProperty]
        public string VendorID { get; set; }
        [BindProperty]
        public string SupplierName { get; set; }
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
        public string CurrencyCode { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public string PageBack { get; set; }

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
        public async Task<IActionResult> OnGetAsync(int AddRMSampleID)
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_ADDRAWMATERIALSAMPLE));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    AddRMSampleId = AddRMSampleID;
                    PageBack = "Main";
                    await GetData(AddRMSampleID);

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase/AddRawMaterialSample/Main");
            }
        }

        public async Task GetData(int AddRMSampleID)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                var RMAssessmentRepo = new GenericRepository<S2ERMAssessment_TB>(unitOfWork.Transaction);
                var RMAssessmentByID = await RMAssessmentRepo.GetAsync(AddRMSampleByID.ASSESSMENTID);

                var RequestID = RMAssessmentByID.REQUESTID;

                var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                RequestCode = NewRequestByID.REQUESTCODE;
                VendorID = NewRequestByID.VENDORID;

                SupplierName = NewRequestByID.SUPPLIERNAME;
                ProductionSite = NewRequestByID.PRODUCTIONSITE;
                DealerAddress = NewRequestByID.DEALERADDRESS;
                ItemName = NewRequestByID.ITEMNAME;
                Price = NewRequestByID.PRICE;
                Unit = NewRequestByID.UNIT;
                Qty = NewRequestByID.QTY;
                CurrencyCode = NewRequestByID.CURRENCYCODE;

                Plant = AddRMSampleByID.PLANT;

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostGridViewFileUploadAsync(int AddRMSampleID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EAddRawMaterialSampleLogsFileGridViewModel>($@"
                        SELECT *
                        FROM
                        (
                            SELECT ID,
		                        ADDRMSAMPLEID,
		                        FILENAME,
                                CREATEBY,
		                        CONVERT(NVARCHAR,CREATEDATE,103) + ' '+ CONVERT(NVARCHAR,CREATEDATE,108) AS CREATEDATE,
		                        ISACTIVE
                            FROM TB_S2EAddRawMaterialSampleLogsFile 
                            WHERE ADDRMSAMPLEID = {AddRMSampleID} AND ISACTIVE = 1
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
        public async Task<IActionResult> OnGetDownloadFileUploadAsync(int AddRMSampleID, int Fileid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AddRMSampleLogFileRepo = new GenericRepository<S2EAddRawMaterialSampleLogsFile_TB>(unitOfWork.Transaction);
                    var AddRMSampleLogFileByFileID = await AddRMSampleLogFileRepo.GetAsync(Fileid);

                    var AddRMSampleRepo = new GenericRepository<S2EAddRawMaterialSample_TB>(unitOfWork.Transaction);
                    var AddRMSampleByID = await AddRMSampleRepo.GetAsync(AddRMSampleID);

                    var RequestID = AddRMSampleByID.REQUESTID;
                    var NewRequestRepo = new GenericRepository<S2ENewRequest_TB>(unitOfWork.Transaction);
                    var NewRequestByID = await NewRequestRepo.GetAsync(RequestID);

                    var RequestCode = NewRequestByID.REQUESTCODE;

                    var filePath = $"wwwroot/files/S2EFiles/S2E_" + RequestCode.Substring(4, 3) + "_" +
                        RequestCode.Substring(8, 2) + "_" + RequestCode.Substring(11, 2) + "/AddRawMaterialSample/" + AddRMSampleID;

                    var fileName = AddRMSampleLogFileByFileID.FILENAME;

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
