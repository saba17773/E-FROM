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
using Web.UI.Infrastructure.Models.Import.TurnOver;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Purchase.Import
{
    public class TurnOverModel : PageModel
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

        public TurnOverModel(
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
                var rootPathUpload = "wwwroot/files/import/turn_over";

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
                var _dataGroup = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplateTurnOverGroupExcel>("TP_KPI_Group");

                var _dataSubGroup = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplateTurnOverSubGroupExcel>("TP_KPI_Subgroup");

                if (_dataGroup.ToList().Count == 0)
                    throw new Exception("Data not found.");

                if (_dataSubGroup.ToList().Count == 0)
                    throw new Exception("Data not found.");

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("Import")))
                {
                    var groupTable = new GenericRepository<ImportTemplateTurnOverGroupTable>(unitOfWork.Transaction);

                    var subGroupTable = new GenericRepository<ImportTemplateTurnOverSubGroupTable>(unitOfWork.Transaction);

                    foreach (var item in _dataGroup)
                    {
                        var dataInTable = groupTable.GetAll()
                                .Where(x => x.Year == item.Year && x.Group == item.Group)
                                .FirstOrDefault();
                        if (dataInTable != null)
                        {
                            await groupTable.DeleteAsync(dataInTable);
                        }

                        var dataToInsert = _dataGroup.Where(x => x.Year == item.Year && x.Group == item.Group).FirstOrDefault();

                        await groupTable.InsertAsync(new ImportTemplateTurnOverGroupTable
                        {
                            Year = dataToInsert.Year,
                            Group = dataToInsert.Group,
                            KPI = dataToInsert.KPI
                        });
                    }

                    foreach (var item in _dataSubGroup)
                    {
                        var dataInTable = subGroupTable.GetAll()
                                .Where(x => x.Year == item.Year && x.SubGroup == item.SubGroup)
                                .FirstOrDefault();

                        if (dataInTable != null)
                        {
                            await subGroupTable.DeleteAsync(dataInTable);
                        }

                        var dataToInsert = _dataSubGroup.Where(x => x.Year == item.Year && x.SubGroup == item.SubGroup).FirstOrDefault();

                        await subGroupTable.InsertAsync(new ImportTemplateTurnOverSubGroupTable
                        {
                            Year = dataToInsert.Year,
                            SubGroup = dataToInsert.SubGroup,
                            KPI = dataToInsert.KPI
                        });
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Upload file success";
                    return Redirect("/Purchase/Import/TurnOver");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Purchase/Import/TurnOver");
            }
        }
    }
}