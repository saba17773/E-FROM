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
    public class TemplateFCOVSModel : PageModel
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

        public TemplateFCOVSModel(
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

                        var products = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplateFCModel>(0);
                        DateTime updateD = DateTime.Now;
                        string updateDate = updateD.ToString("yyyy-MM-dd HH:mm:ss");
                        int CreateByOld = 0;
                        DateTime? CreateDateOld = null;
                        string OVSText = "OVS";
                        var dataBP = await unitOfWork.Transaction.Connection.QueryFirstOrDefaultAsync($@"
                            SELECT TOP 5 *
                            FROM TB_TicketTemplateForcast
                            WHERE Year = {TicketTamplateFC.Year}
                            AND VersionId = {TicketTamplateFC.VersionId}
                            AND CustGroup = '{OVSText}'
                        ", null, unitOfWork.Transaction);
                        if (dataBP != null)
                        {
                            CreateByOld = dataBP.CreateBy;
                            CreateDateOld = dataBP.CreateDate;
                        }

                        int[] quarterlist = new int[] { };

                        if (TicketTamplateFC.Quarter == "1")
                        {
                            quarterlist = new int[] { 1, 2, 3 };
                        }
                        if (TicketTamplateFC.Quarter == "2")
                        {
                            quarterlist = new int[] { 4, 5, 6 };
                        }
                        if (TicketTamplateFC.Quarter == "3")
                        {
                            quarterlist = new int[] { 7, 8, 9 };
                        }
                        if (TicketTamplateFC.Quarter == "4")
                        {
                            quarterlist = new int[] { 10, 11, 12 };
                        }

                        double QTYTemp = 0;
                        double AmountTemp = 0;

                        // await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        //         DELETE FROM TB_TicketTemplateForcast 
                        //         WHERE Year = {TicketTamplateFC.Year}
                        //         AND VersionId = {TicketTamplateFC.VersionId}
                        //         AND CustGroup = '{OVSText}'
                        //     ", null, unitOfWork.Transaction);

                        for (int i = 0; i < quarterlist.Length; i++)
                        {
                            foreach (var item in products)
                            {
                                if (i == 0)
                                {
                                    QTYTemp = item.QTY_01;
                                    AmountTemp = item.Amount_01;
                                }
                                else if (i == 1)
                                {
                                    QTYTemp = item.QTY_02;
                                    AmountTemp = item.Amount_02;
                                }
                                else
                                {
                                    QTYTemp = item.QTY_03;
                                    AmountTemp = item.Amount_03;
                                }
                                await importTemplateFCRepo.InsertAsync(new TicketTemplateFCTable
                                {
                                    GroupId = item.GroupId,
                                    CustGroup = item.CustGroup,
                                    Year = item.Year,
                                    VersionId = TicketTamplateFC.VersionId,
                                    Quarter = TicketTamplateFC.Quarter,
                                    ItemId = item.ItemId,
                                    ItemName = item.ItemName,
                                    ProductGroup = item.ProductGroup,
                                    SubGroup = item.SubGroup,
                                    ProductType = item.ProductType,
                                    QTY = QTYTemp,
                                    Amount = AmountTemp,
                                    Month = quarterlist[i],
                                    // Status = TicketTamplateFC.Status,
                                    Remark = TicketTamplateFC.Remark,
                                    UpdateBy = _authService.GetClaim().UserId,
                                    UpdateDate = DateTime.Now,
                                    CreateBy = _authService.GetClaim().UserId,
                                    CreateDate = DateTime.Now
                                });
                            }
                        }

                        unitOfWork.Complete();

                        AlertSuccess = "Import Succesful.";
                        return Redirect("/Planing/Import/TemplateFCOVS");
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
                return Redirect("/Planing/Import/TemplateFCOVS");
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