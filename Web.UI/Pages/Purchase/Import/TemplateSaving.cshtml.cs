using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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
    public class TemplateSavingModel : PageModel
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

        public TemplateSavingModel(
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
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("Import")))
                {
                    var t = unitOfWork.Transaction;

                    var importRepo = new GenericRepository<ImportTable>(unitOfWork.Transaction);
                    var importTemplateSavingRepo = new GenericRepository<ImportTemplateSavingTable>(unitOfWork.Transaction);

                    var filePath = "";
                    var rootPathUpload = "wwwroot/files/import/template_saving";

                    if (TemplateUpload != null)
                    {
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

                        // update exchange rate
                        var exchangeRateData = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplateSaving_ExchangeRateModel>("Rate");

                        List<string> sheetName = new List<string>() {
                            "RM",
                            "FS",
                            "PK",
                            "OS",
                            "SP",
                            "CO"
                        };

                        foreach (var sheet in sheetName)
                        {
                            var templateSaving = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplateSavingModel>(sheet);

                            if (templateSaving.ToList().Count > 0)
                            {
                                // delete old data
                                var current = templateSaving.GroupBy(x => new { x.SubGroupType, x.Year });

                                foreach (var item in current)
                                {
                                    var deleteAffectedRows = await unitOfWork.Import.DeleteImportTemplateSavingByYearAndTypeAsync(item.Key.Year, item.Key.SubGroupType);

                                    Console.WriteLine(deleteAffectedRows + " have been deleted.");
                                }

                                var importId = await importRepo.InsertAsync(new ImportTable
                                {
                                    CreateBy = _authService.GetClaim().UserId,
                                    CreateDate = DateTime.Now,
                                    FileName = TemplateUpload.FileName
                                });

                                foreach (var item in templateSaving.Where(x => x.SubGroupType != null))
                                {
                                    var totalQty = item.Jan + item.Feb + item.Mar + item.Apr + item.May + item.Jun + item.Jul + item.Aug + item.Sep + item.Oct + item.Nov + item.Dec;

                                    var saving = item.LatestPrice - item.NewPrice;

                                    // saving * qty * exchange rate
                                    var exRate_Jan = saving * item.Jan * GetExchangeRate(exchangeRateData, item.Year, 1, item.CUR).Rate;
                                    var exRate_Feb = saving * item.Feb * GetExchangeRate(exchangeRateData, item.Year, 2, item.CUR).Rate;
                                    var exRate_Mar = saving * item.Mar * GetExchangeRate(exchangeRateData, item.Year, 3, item.CUR).Rate;
                                    var exRate_Apr = saving * item.Apr * GetExchangeRate(exchangeRateData, item.Year, 4, item.CUR).Rate;
                                    var exRate_May = saving * item.May * GetExchangeRate(exchangeRateData, item.Year, 5, item.CUR).Rate;
                                    var exRate_Jun = saving * item.Jun * GetExchangeRate(exchangeRateData, item.Year, 6, item.CUR).Rate;
                                    var exRate_Jul = saving * item.Jul * GetExchangeRate(exchangeRateData, item.Year, 7, item.CUR).Rate;
                                    var exRate_Aug = saving * item.Aug * GetExchangeRate(exchangeRateData, item.Year, 8, item.CUR).Rate;
                                    var exRate_Sep = saving * item.Sep * GetExchangeRate(exchangeRateData, item.Year, 9, item.CUR).Rate;
                                    var exRate_Oct = saving * item.Oct * GetExchangeRate(exchangeRateData, item.Year, 10, item.CUR).Rate;
                                    var exRate_Nov = saving * item.Nov * GetExchangeRate(exchangeRateData, item.Year, 11, item.CUR).Rate;
                                    var exRate_Dec = saving * item.Dec * GetExchangeRate(exchangeRateData, item.Year, 12, item.CUR).Rate;

                                    var totalSaving = exRate_Jan + exRate_Feb + exRate_Mar + exRate_Apr + exRate_May + exRate_Jun + exRate_Jul + exRate_Aug + exRate_Sep + exRate_Oct + exRate_Nov + exRate_Dec;

                                    await importTemplateSavingRepo.InsertAsync(new ImportTemplateSavingTable
                                    {
                                        SubGroupType = item.SubGroupType,
                                        Year = item.Year,
                                        Company = item.Company,
                                        ItemGroup = item.ItemGroup,
                                        ItemSubGroup = item.ItemSubGroup,
                                        ItemCode = item.ItemCode,
                                        ItemDescription = item.ItemDescription,
                                        UOM = item.UOM,
                                        CUR = item.CUR,
                                        NewNegotiate = item.NewNegotiate,
                                        Supplier1 = item.Supplier1,
                                        LatestPrice = item.LatestPrice,
                                        NewPrice = item.NewPrice,
                                        Supplier2 = item.Supplier2,
                                        Saving = saving,
                                        Jan = item.Jan,
                                        Feb = item.Feb,
                                        Mar = item.Mar,
                                        Apr = item.Apr,
                                        May = item.May,
                                        Jun = item.Jun,
                                        Jul = item.Jul,
                                        Aug = item.Aug,
                                        Sep = item.Sep,
                                        Oct = item.Oct,
                                        Nov = item.Nov,
                                        Dec = item.Dec,
                                        TotalQTY = totalQty,
                                        CS_Jan = exRate_Jan,
                                        CS_Feb = exRate_Feb,
                                        CS_Mar = exRate_Mar,
                                        CS_Apr = exRate_Apr,
                                        CS_May = exRate_May,
                                        CS_Jun = exRate_Jun,
                                        CS_Jul = exRate_Jul,
                                        CS_Aug = exRate_Aug,
                                        CS_Sep = exRate_Sep,
                                        CS_Oct = exRate_Oct,
                                        CS_Nov = exRate_Nov,
                                        CS_Dec = exRate_Dec,
                                        TotalCostSaving = totalSaving
                                    });
                                }
                            }
                        }

                        unitOfWork.Complete();

                        AlertSuccess = "Import Succesful.";
                        return Redirect("/Purchase/Import/TemplateSaving");
                    }
                    else
                    {
                        throw new Exception("Error : file not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Purchase/Import/TemplateSaving");
            }
        }

        public ImportTemplateSaving_ExchangeRateModel GetExchangeRate(IEnumerable<ImportTemplateSaving_ExchangeRateModel> data, int year, int month, string currency)
        {
            //return new ImportTemplateSaving_ExchangeRateModel { Rate = 1 };
            return data.Where(x => x.Year == year && x.Month == month && x.Currency == currency).FirstOrDefault();
        }
    }
}