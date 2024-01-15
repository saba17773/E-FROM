using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Ganss.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models.Import.Martket;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Purchase.Import
{
    public class MarketForecastModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        public IFormFile TemplateUpload { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;


        public MarketForecastModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          ICreditControlService creditControlService,
          IEmailService emailService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _creditControlService = creditControlService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (TemplateUpload == null)
                {
                    throw new Exception("File not found.");
                }

                var filePath = "";
                var rootPathUpload = "wwwroot/files/import/market_forecast";

                filePath = $"{rootPathUpload}/{DateTime.Now.ToString("yyyyMMdd_HHmm")}";

                if (!System.IO.Directory.Exists($"{rootPathUpload}"))
                {
                    System.IO.Directory.CreateDirectory($"{rootPathUpload}");
                }

                /// upload file
                using (var stream = System.IO.File.Create(filePath))
                {
                    await TemplateUpload.CopyToAsync(stream);
                }

                var excelTargetData = new ExcelMapper(filePath)
                {
                    HeaderRow = true
                }.Fetch<ImportTemplateMarketForecastExcelModel>(0);

                if (excelTargetData.ToList().Count == 0)
                    throw new Exception("Data not found.");

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("Import")))
                {
                    var t = unitOfWork.Transaction;

                    var doCompanyList = t.Connection.Query<ClearOldDataParamsModel>(@"
                        SELECT [YEAR] AS [Year], [Company] 
                        FROM TB_ImportTemplateMarketForecast
                        GROUP BY [YEAR], [Company]
                    ", null, t);

                    foreach (var item in doCompanyList)
                    {
                        await t.Connection.ExecuteAsync(
                        @"
                            DELETE FROM TB_ImportTemplateMarketForecast
                            WHERE Year = @Year AND Company = @Company
                        ", new
                        {
                            @Year = item.Year,
                            @Company = item.Company
                        }, t);
                    }

                    foreach (var item in excelTargetData)
                    {
                        await t.Connection.InsertAsync(new ImportTemplateMarketForecast
                        {
                            Year = item.Year,
                            Month = item.Month,
                            Company = item.Company.ToUpper(),
                            Forecast = item.Forecast,
                            SubGroupNew = item.SubGroupNew
                        }, t);
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Upload file success";
                    return Redirect($"/Purchase/Import/MarketForecast");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Purchase/Import/MarketForecast");
            }
        }
    }
}
