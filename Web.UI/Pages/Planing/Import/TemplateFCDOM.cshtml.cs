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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Planing.Import
{
    public class TemplateFCDOMModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public TicketTemplateFCTable TicketTamplateFC { get; set; }

        [BindProperty]
        public IFormFile TemplateUpload { get; set; }
        public List<SelectListItem> YearMaster { get; set; }
        public List<SelectListItem> VersionMaster { get; set; }
        public List<SelectListItem> SetTypeMaster { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public TemplateFCDOMModel(
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

        private async Task InitialDataAsync()
        {
            TicketTamplateFC = new TicketTemplateFCTable();
            YearMaster = await GetYearMasterAsync();
            VersionMaster = await GetVersionMasterAsync();
            SetTypeMaster = await GetSetTypeMasterAsync();
        }

        public async Task OnGetAsync()
        {
            await InitialDataAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await InitialDataAsync();

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var importRepo = new GenericRepository<ImportTable>(unitOfWork.Transaction);
                    var importTemplateFCRepo = new GenericRepository<TicketTemplateFCTable>(unitOfWork.Transaction);

                    var filePath = "";
                    var rootPathUpload = $"wwwroot/files/import/template_fc/";

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

                        var products = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplateFCDOMModel>(0);
                        DateTime updateD = DateTime.Now;
                        string updateDate = updateD.ToString("yyyy-MM-dd HH:mm:ss");

                        // double QTYTemp = 0;
                        // double AmountTemp = 0;
                        // int MonthTemp = 0;
                        // int YearTemp = 0;

                        // if (TicketTamplateFC.Month == 12)
                        // {

                        //     for (int i = 0; i < 2; i++)
                        //     {
                        //         if (i == 0)
                        //         {
                        //             await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        //                 DELETE FROM TB_TicketTemplateForcast 
                        //                 WHERE Year = {TicketTamplateFC.Year}
                        //                 AND VersionId = {TicketTamplateFC.VersionId}
                        //                 AND CustGroup IN ('DOM','CLM') 
                        //                 AND Month = {TicketTamplateFC.Month}
                        //             ", null, unitOfWork.Transaction);
                        //         }
                        //         else if (i == 1)
                        //         {
                        //             await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        //                 DELETE FROM TB_TicketTemplateForcast 
                        //                 WHERE Year = {TicketTamplateFC.Year + i}
                        //                 AND VersionId = {TicketTamplateFC.VersionId}
                        //                 AND CustGroup IN ('DOM','CLM') 
                        //                 AND Month = {1}
                        //             ", null, unitOfWork.Transaction);
                        //         }

                        //         foreach (var item in products)
                        //         {
                        //             if (i == 0)
                        //             {
                        //                 QTYTemp = item.Qty_Current;
                        //                 AmountTemp = item.Amount_Current;
                        //                 MonthTemp = TicketTamplateFC.Month;
                        //                 YearTemp = TicketTamplateFC.Year;
                        //             }
                        //             else if (i == 1)
                        //             {
                        //                 QTYTemp = item.Qty_Next;
                        //                 AmountTemp = item.Amount_Next;
                        //                 MonthTemp = 1;
                        //                 YearTemp = TicketTamplateFC.Year + 1;
                        //             }
                        //             await importTemplateFCRepo.InsertAsync(new TicketTemplateFCTable
                        //             {
                        //                 GroupId = item.GroupId,
                        //                 CustGroup = item.CustGroup,
                        //                 Year = YearTemp,
                        //                 Month = MonthTemp,
                        //                 VersionId = TicketTamplateFC.VersionId,
                        //                 ItemId = item.ItemId,
                        //                 ItemName = item.ItemName,
                        //                 ProductGroup = item.ProductGroup,
                        //                 SubGroup = item.SubGroup,
                        //                 ProductType = item.ProductType,
                        //                 QTY = QTYTemp,
                        //                 Amount = AmountTemp,
                        //                 Status = TicketTamplateFC.Status,
                        //                 Remark = TicketTamplateFC.Remark,
                        //                 CreateBy = _authService.GetClaim().UserId,
                        //                 CreateDate = DateTime.Now
                        //             });
                        //         }
                        //     }

                        // }
                        // else
                        // {

                        // for (int i = 0; i < 2; i++)
                        // {
                        // await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        //     DELETE FROM TB_TicketTemplateForcast 
                        //     WHERE Year = {TicketTamplateFC.Year}
                        //     AND VersionId = {TicketTamplateFC.VersionId}
                        //     AND CustGroup IN ('DOM','CLM') 
                        //     AND Month = {TicketTamplateFC.Month + i}
                        // ", null, unitOfWork.Transaction);

                        foreach (var item in products)
                        {
                            // if (i == 0)
                            // {
                            //     QTYTemp = item.Qty_Current;
                            //     AmountTemp = item.Amount_Current;
                            //     MonthTemp = TicketTamplateFC.Month;
                            // }
                            // else if (i == 1)
                            // {
                            //     QTYTemp = item.Qty_Next;
                            //     AmountTemp = item.Amount_Next;
                            //     MonthTemp = TicketTamplateFC.Month + i;
                            // }
                            await importTemplateFCRepo.InsertAsync(new TicketTemplateFCTable
                            {
                                GroupId = item.GroupId,
                                CustGroup = item.CustGroup,
                                Year = item.Year,
                                Month = TicketTamplateFC.Month,
                                VersionId = TicketTamplateFC.VersionId,
                                ItemId = item.ItemId,
                                ItemName = item.ItemName,
                                ProductGroup = item.ProductGroup,
                                SubGroup = item.SubGroup,
                                ProductType = item.ProductType,
                                QTY = item.Qty_Current,
                                Amount = item.Amount_Current,
                                QTYNext = item.Qty_Next,
                                AmountNext = item.Amount_Next,
                                Status = TicketTamplateFC.Status,
                                Remark = TicketTamplateFC.Remark,
                                CreateBy = _authService.GetClaim().UserId,
                                CreateDate = DateTime.Now
                            });
                        }
                        // }
                        // }

                        unitOfWork.Complete();

                        AlertSuccess = "Import Succesful.";
                        return Redirect("/Planing/Import/TemplateFCDOM");
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
                return Redirect("/Planing/Import/TemplateFCDOM");
            }
        }

        public async Task<List<SelectListItem>> GetYearMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var yearRepo = new GenericRepository<TicketYearMasterTable>(unitOfWork.Transaction);

                var yearAll = await yearRepo.GetAllAsync();

                return yearAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Year.ToString(),
                        Text = x.Year.ToString()
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetVersionMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var versionRepo = new GenericRepository<TicketVersionMasterTable>(unitOfWork.Transaction);

                var versionAll = await versionRepo.GetAllAsync();

                return versionAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Version.ToString()
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetSetTypeMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var settypeRepo = new GenericRepository<TicketSetTypeMasterTable>(unitOfWork.Transaction);

                var settypeAll = await settypeRepo.GetAllAsync();

                return settypeAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.SetType.ToString()
                    })
                    .ToList();
            }
        }

    }
}