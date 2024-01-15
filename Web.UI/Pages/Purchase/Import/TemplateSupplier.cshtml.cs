using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class TemplateSupplierModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public IFormFile TemplateUpload { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public TemplateSupplierModel(
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

        public async Task<IActionResult> OnPost()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("Import")))
                {
                    var importRepo = new GenericRepository<ImportTable>(unitOfWork.Transaction);
                    var importTemplateSupplierRepo = new GenericRepository<ImportTemplateSupplierTable>(unitOfWork.Transaction);

                    var filePath = "";
                    var rootPathUpload = $"wwwroot/files/import/template_supplier/";

                    CultureInfo cultureInfo = new CultureInfo("en-US");
                    var ArticleDate = DateTime.Now.ToString("yyyyMMdd_HHmmss", cultureInfo);
                    var newNameFile = ArticleDate + "_" + TemplateUpload.FileName;
                    if (TemplateUpload != null)
                    {
                        filePath = $"{rootPathUpload}/{newNameFile}";

                        if (!System.IO.Directory.Exists(filePath))
                        {
                            System.IO.Directory.CreateDirectory($"{rootPathUpload}");
                        }

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await TemplateUpload.CopyToAsync(stream);
                        }

                        var importId = await importRepo.InsertAsync(new ImportTable
                        {
                            CreateBy = _authService.GetClaim().UserId,
                            CreateDate = DateTime.Now,
                            FileName = TemplateUpload.FileName
                        });

                        await unitOfWork.Transaction.Connection.ExecuteAsync(@"
                            DELETE FROM TB_ImportTemplateSupplier 
                            WHERE YEAR = @YEAR 
                        ", new
                        {
                            @YEAR = 2019
                        }, unitOfWork.Transaction);

                        int sheets = 5;
                        for (int i = 0; i < sheets; i++)
                        {
                            var products = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplateSupplierModel>(i);

                            foreach (var item in products)
                            {
                                await importTemplateSupplierRepo.InsertAsync(new ImportTemplateSupplierTable
                                {
                                    Year = item.Year,
                                    Period = item.Period,
                                    SupCode = item.SupCode,
                                    Quality = item.Quality,
                                    Safety = item.Safety,
                                    Delivery = item.Delivery,
                                    Company = item.Company,
                                    Grade = GetGrade(item.Quality + item.Safety + item.Delivery),
                                    ImportId = (int)importId
                                });
                            }
                        }

                        unitOfWork.Complete();

                        AlertSuccess = "Import Succesful.";
                        return Redirect("/Purchase/Import/TemplateSupplier");
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
                return Redirect("/Purchase/Import/TemplateSupplier");
            }
        }

        private string GetGrade(float score)
        {
            if (score >= 0 && score <= 55.4)
            {
                return "D";
            }
            else if (score >= 55.5 && score <= 75.4)
            {
                return "C";
            }
            else if (score >= 75.5 && score <= 85.4)
            {
                return "B";
            }
            else if (score >= 85.5 && score <= 100)
            {
                return "A";
            }
            else
            {
                return "-";
            }
        }
    }
}