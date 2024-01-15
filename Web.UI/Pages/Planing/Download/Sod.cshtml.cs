using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Planing.Download
{
    public class SodModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public List<TicketTemplateSODTable> SOD { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public SodModel(
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

        public void OnGet(int year, int month)
        {
        }

        public async Task<IActionResult> OnGetDownLoadSOD(int year, int month)
        {
            try
            {
                string pathInfo;
                string fileName;
                pathInfo = "wwwroot/files/import/template_sod/download/sod.xlsx";
                fileName = "SOD_" + year + "_" + month + ".xlsx";
                
                FileInfo template = new FileInfo(pathInfo);

                using (var package = new ExcelPackage(template))
                {
                    var workbook = package.Workbook;
                    var ws = workbook.Worksheets["Sheet1"];
                    var _contextDB = "Ticketblock";
                    using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection(_contextDB)))
                    {
                        var datasod = await unitOfWork.Transaction.Connection.QueryAsync($@"
                            SELECT * FROM SOD 
                            WHERE YEAR(AllocateMonth) = {year}
                            AND MONTH(AllocateMonth) = {month}
                        ", null, unitOfWork.Transaction);
                        unitOfWork.Complete();

                        int i = 2;
                        foreach (var item in datasod)
                        {
                            ws.Cells["A" + i].Value = item.AllocateMonth;
                            ws.Cells["B" + i].Value = item.RequestQTY;
                            ws.Cells["C" + i].Value = item.ConfirmQTY;
                            ws.Cells["D" + i].Value = item.Custcode;
                            ws.Cells["E" + i].Value = item.Itemid;
                            ws.Cells["F" + i].Value = item.Cango;
                            ws.Cells["G" + i].Value = item.Out;
                            i++;
                        }
                    }
                    package.SaveAs(new FileInfo("wwwroot/files/import/template_sod/" + fileName));
                    string newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/import/template_sod/");
                    IFileProvider provider = new PhysicalFileProvider(newPath);
                    IFileInfo fileInfo = provider.GetFileInfo(fileName);
                    var readStream = fileInfo.CreateReadStream();
                    var mimeType = "application/vnd.ms-excel";
                    return File(readStream, mimeType, fileName);
                    // return new JsonResult(new { DownloadBP = 1 });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }
    }
}