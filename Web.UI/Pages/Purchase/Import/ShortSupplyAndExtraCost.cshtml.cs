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
using Web.UI.Infrastructure.Models.Import.ShortSupplyAndExtraCost;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Purchase.Import
{
    public class ShortSupplyAndExtraCostModel : PageModel
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

        public ShortSupplyAndExtraCostModel(
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
                // initial file and folder
                if (TemplateUpload == null)
                {
                    throw new Exception("File not found.");
                }

                var filePath = "";
                var rootPathUpload = "wwwroot/files/import/short_supply_and_extra_cost";

                filePath = $"{rootPathUpload}/{DateTime.Now.ToString("yyyyMMdd_HHmm")}";

                if (!System.IO.Directory.Exists($"{rootPathUpload}"))
                {
                    System.IO.Directory.CreateDirectory($"{rootPathUpload}");
                }

                // upload file
                using (var stream = System.IO.File.Create(filePath))
                {
                    await TemplateUpload.CopyToAsync(stream);
                }

                // fetch data
                var _data = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplateShortSupplyAndExtraCostExcel>("Template");

                if (_data.ToList().Count == 0)
                    throw new Exception("Data not found.");

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("Import")))
                {
                    var ssesTable = new GenericRepository<ImportTemplateShortSupplyAndExtraCostTable>(unitOfWork.Transaction);

                    foreach (var item in _data)
                    {
                        var dataInTable = ssesTable.GetAll()
                                .Where(x => 
                                    x.Year == item.Year && 
                                    x.Month == item.Month && 
                                    x.Date == item.Date)
                                .FirstOrDefault();
                        if (dataInTable != null)
                        {
                            await ssesTable.DeleteAsync(dataInTable);
                        }

                        var dataToInsert = _data.Where(x => 
                            x.Year == item.Year &&
                            x.Month == item.Month &&
                            x.Date == item.Date).FirstOrDefault();

                        await ssesTable.InsertAsync(new ImportTemplateShortSupplyAndExtraCostTable
                        {
                            Year = dataToInsert.Year,
                            Month = dataToInsert.Month,
                            Date = dataToInsert.Date,
                            Action = dataToInsert.Action,
                            Company = dataToInsert.Company,
                            ExtraCost = dataToInsert.ExtraCost,
                            Group = dataToInsert.Group,
                            Issue = dataToInsert.Issue,
                            Item = dataToInsert.Item,
                            ShortDays = dataToInsert.ShortDays,
                            ShortQtyMT = dataToInsert.ShortQtyMT,
                            Supplier = dataToInsert.Supplier
                        });
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Upload file success";
                    return Redirect("/Purchase/Import/ShortSupplyAndExtraCost");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Purchase/Import/ShortSupplyAndExtraCost");
            }
        }
    }
}