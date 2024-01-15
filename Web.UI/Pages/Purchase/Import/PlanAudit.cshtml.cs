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
    public class PlanAuditModel : PageModel
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

        public PlanAuditModel(
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
                var rootPathUpload = "wwwroot/files/import/plan_audit";

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

                var planAuditData = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplatePlanAuditExcel>("Plan_Audit");

                if (planAuditData.ToList().Count == 0)
                    throw new Exception("Data not found.");

                var planAuditTargetData = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplatePlanAuditTargetExcel>("Target");

                if (planAuditTargetData.ToList().Count == 0)
                    throw new Exception("Data not found.");

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("Import")))
                {
                    // audit
                    var importPlanAuditTable = new GenericRepository<ImportTemplatePlanAuditTable>(unitOfWork.Transaction);

                    // audit target
                    var importPlanAuditTargetTable = new GenericRepository<ImportTemplatePlanAuditTargetTable>(unitOfWork.Transaction);

                    // check year
                    var checkYear = planAuditData.FirstOrDefault();

                    var checkCurrentData = planAuditData.Any(x => x.Year == checkYear.Year);
                    if (checkCurrentData == true)
                    {
                        var planAuditDataToDelete = importPlanAuditTable.GetAll().Where(x => x.Year == checkYear.Year);

                        foreach (var item in planAuditDataToDelete)
                        {
                            await importPlanAuditTable.DeleteAsync(item);
                        }
                    }

                    // check year target
                    foreach (var item in planAuditTargetData)
                    {
                        if (importPlanAuditTargetTable.GetAll().Any(x => x.Year == item.Year))
                        {
                            var target = importPlanAuditTargetTable.GetAll().Where(x => x.Year == item.Year).FirstOrDefault();
                            await importPlanAuditTargetTable.DeleteAsync(target);
                        }
                    }

                    // insert audit data
                    foreach (var item in planAuditData)
                    {
                        if (item.SubCode != null)
                        {
                            var _tempPlanAudit = new ImportTemplatePlanAuditTable
                            {
                                Year = item.Year,
                                SubCode = item.SubCode,
                                ProductSales = item.ProductSales,
                                CompanyType = item.CompanyType,
                                ActualMonth = item.ActualMonth,
                                PlanMonth = item.PlanMonth
                            };

                            await importPlanAuditTable.InsertAsync(_tempPlanAudit);
                        }

                    }

                    foreach (var item in planAuditTargetData)
                    {
                        var _tempAuditTarget = new ImportTemplatePlanAuditTargetTable { 
                            Year = item.Year,
                            Target = item.Target
                        };

                        await importPlanAuditTargetTable.InsertAsync(_tempAuditTarget);

                    }

                    unitOfWork.Complete();
                }

                AlertSuccess = "Upload file success";
                return Redirect("/Purchase/Import/PlanAudit");
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Purchase/Import/PlanAudit");
            }
        }
    }
}