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
    public class TemplateSODModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public TicketTemplateSODTable TicketTamplateSOD { get; set; }

        [BindProperty]
        public IFormFile TemplateUpload { get; set; }
        public List<SelectListItem> YearMaster { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public TemplateSODModel(
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

        // private async Task InitialDataAsync()
        // {
        //     TicketTamplateSOD = new TicketTemplateSODTable();
        // }

        // public async Task OnGetAsync()
        // {
        //     await InitialDataAsync();
        // }
        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection("Ticketblock")))
                {
                    var importTemplateSODRepo = new GenericRepository<TicketTemplateSODTable>(unitOfWork.Transaction);
                    var filePath = "";
                    var rootPathUpload = $"wwwroot/files/import/template_sod";

                    CultureInfo cultureInfo = new CultureInfo("en-US");
                    var ArticleDate = DateTime.Now;

                    if (TemplateUpload != null)
                    {
                        var newNameFile = ArticleDate.ToString("yyyy-MM-dd", new CultureInfo("en-US")) + "_" + TemplateUpload.FileName;
                        filePath = $"{rootPathUpload}/{newNameFile}";

                        if (!System.IO.Directory.Exists(rootPathUpload))
                        {
                            System.IO.Directory.CreateDirectory($"{rootPathUpload}");
                        }

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            TemplateUpload.CopyTo(stream);
                        }

                        var products = new ExcelMapper(filePath) { HeaderRow = true }.Fetch<ImportTemplateSODModel>(0);
                        
                        if(products != null){
                            foreach (var item in products)
                            {
                                // Console.WriteLine(item.Cango);
                                // Console.WriteLine(item.Cango);
                                // Console.WriteLine(TicketTamplateSOD.AllocateMonth);
                                // Console.WriteLine(
                                //     item.RequestQTY+"_"+item.ConfirmQTY+"_"+item.ConfirmQTY+"_"+item.Custcode.Trim()+"_"+item.Itemid.Trim()+"_"+item.Cango+"_"+item.Out+"_"+_authService.GetClaim().UserId+"_"+DateTime.Now
                                // );

                                importTemplateSODRepo.Insert(new TicketTemplateSODTable
                                {
                                    AllocateMonth = TicketTamplateSOD.AllocateMonth,
                                    RequestQTY = item.RequestQTY,
                                    ConfirmQTY = item.ConfirmQTY,
                                    Custcode = item.Custcode,
                                    Itemid = item.Itemid,
                                    Cango = item.Cango,
                                    Out = item.Out,
                                    CreateBy = _authService.GetClaim().UserId,
                                    CreateDate = DateTime.Now
                                });
                                

                            }
                        }else{
                            Console.WriteLine("Read Found!");
                        }
                        
                        
                        unitOfWork.Complete();

                        AlertSuccess = "Import Succesful.";
                        return Redirect("/Planing/Import/TemplateSOD");
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
                return Redirect("/Planing/Import/TemplateSOD");
            }
        }

    }
}