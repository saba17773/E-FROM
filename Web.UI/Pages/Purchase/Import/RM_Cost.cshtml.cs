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
using Web.UI.Infrastructure.Models.Import.RM_Cost;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Purchase.Import
{
    public class RM_CostModel : PageModel
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

        public RM_CostModel(
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
                var rootPathUpload = "wwwroot/files/import/rm_cost";

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
                var rmCostData = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplateRMCostExcel>("Template");

                if (rmCostData.ToList().Count == 0)
                    throw new Exception("Data not found.");

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("Import")))
                {
                    var rmCostTable = new GenericRepository<ImportTemplateRMCostTable>(unitOfWork.Transaction);

                    foreach (var item in rmCostData)
                    {
                        var dataInTable = rmCostTable.GetAll().Where(x => x.Year == item.Year).FirstOrDefault();
                        if (dataInTable != null)
                        {
                            await rmCostTable.DeleteAsync(dataInTable);
                        }

                        var dataToInsert = rmCostData.Where(x => x.Year == item.Year).FirstOrDefault();

                        await rmCostTable.InsertAsync(new ImportTemplateRMCostTable
                        {
                            Year = dataToInsert.Year,
                            Target = dataToInsert.Target
                        });
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Upload file success";
                    return Redirect("/Purchase/Import/RM_Cost");
                }

            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Purchase/Import/RM_Cost");
            }
        }
    }
}