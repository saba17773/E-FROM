using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganss.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Purchase.Import
{
    public class StockACCModel : PageModel
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


        public StockACCModel(
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
                var rootPathUpload = "wwwroot/files/import/delivery";

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

                // sheets
                var companySheets = new List<string>()
                {
                    "DSL",
                    "DRB",
                    "DSI",
                    "SVO",
                    "STR"
                };

                var month = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

                string Target = "Target";

                // get data from file excel
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("Import")))
                {
                    var importTemplateStockAccTable = new GenericRepository<ImportTemplateStockAccTable>(unitOfWork.Transaction);
                    var importTemplateStockAccTargetTable = new GenericRepository<ImportTemplateStockAccTargetTable>(unitOfWork.Transaction);

                    // target
                    var excelTargetData = new ExcelMapper(filePath)
                    {
                        HeaderRow = true
                    }.Fetch<ImportTemplateStockAccTargetExcelModel>(Target);

                    foreach (var row in excelTargetData)
                    {
                        var target = await unitOfWork.PCDashboard.GetDataTemplateStockAccTargetAsync(row.Year);

                        if (target != null)
                        {
                            await importTemplateStockAccTargetTable.DeleteAsync(target);
                        }

                        await importTemplateStockAccTargetTable.InsertAsync(new ImportTemplateStockAccTargetTable
                        {
                            Year = row.Year,
                            KPI = row.KPI
                        });
                    }

                    foreach (var sheet in companySheets)
                    {
                        var excelData = new ExcelMapper(filePath)
                        {
                            MinRowNumber = 1
                        }
                        .Fetch<ImportTemplateDeliveryExcelModel>(sheet);

                        foreach (var row in excelData)
                        {
                            // check old data
                            var dataByMonth = await unitOfWork.PCDashboard.GetDataTemplateStockAccAsync(row.Year, row.Month, row.Company);
                            if (dataByMonth != null)
                            {
                                // delete current data
                                await importTemplateStockAccTable.DeleteAsync(dataByMonth);
                            }

                            await importTemplateStockAccTable.InsertAsync(new ImportTemplateStockAccTable
                            {
                                Year = row.Year,
                                Month = row.Month,
                                StockAcc = row.Performance,
                                Company = row.Company
                            });
                        }
                    }

                    foreach (var target in excelTargetData)
                    {
                        foreach (var m in month)
                        {
                            var perfomanceByCompany = await unitOfWork.PCDashboard.GetDataTemplateStockAccAsync(target.Year, m);
                            
                            if (perfomanceByCompany.ToList().Count > 0)
                            {
                                var performance = perfomanceByCompany.Average(x => x.StockAcc);

                                await importTemplateStockAccTable.InsertAsync(new ImportTemplateStockAccTable
                                {

                                    Year = target.Year,
                                    Month = m,
                                    StockAcc = performance,
                                    Company = "DSG"
                                });
                            }
                            
                        }
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Upload file success";
                    return Redirect($"/Purchase/Import/Delivery");
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                return Redirect($"/Purchase/Import/Delivery");
            }
        }
    }
}