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
    public class BpModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public List<TicketTemplateBPTable> BP { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public BpModel(
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

        public void OnGet(int year, int version, int type)
        {
        }

        public async Task<IActionResult> OnGetDownLoadBP(int year, int version, int type)
        {
            try
            {
                string pathInfo;
                string fileName;
                pathInfo = "wwwroot/files/import/template_bp/download/bp.xlsx";
                if (type == 1)
                {
                    fileName = "BP_" + year + "_V" + version + "_ถอดยางSET.xlsx";
                }
                else
                {
                    fileName = "BP_" + year + "_V" + version + "_ไม่ถอดยางSET.xlsx";
                }
                FileInfo template = new FileInfo(pathInfo);

                using (var package = new ExcelPackage(template))
                {
                    var workbook = package.Workbook;
                    var ws = workbook.Worksheets["Sheet1"];

                    using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                    {
                        var databp = await unitOfWork.Transaction.Connection.QueryAsync($@"
                            SELECT * FROM TB_TicketTemplateBP 
                            WHERE Year = {year}
                            AND VersionId = {version}
                            AND Type = {type}
                        ", null, unitOfWork.Transaction);
                        unitOfWork.Complete();

                        int i = 2;
                        foreach (var item in databp)
                        {
                            ws.Cells["A" + i].Value = "BP";
                            ws.Cells["B" + i].Value = item.Year;
                            ws.Cells["C" + i].Value = item.Month;
                            ws.Cells["D" + i].Value = item.CustomerCode;
                            ws.Cells["E" + i].Value = item.CustomerName;
                            ws.Cells["F" + i].Value = item.ItemId;
                            ws.Cells["G" + i].Value = item.ItemName;
                            ws.Cells["H" + i].Value = item.ProductGroup;
                            ws.Cells["I" + i].Value = item.SubGroup;
                            ws.Cells["J" + i].Value = item.ProductType;
                            ws.Cells["K" + i].Value = item.QTY;
                            ws.Cells["L" + i].Value = item.Amount;
                            if (item.Type == 1)
                            {
                                ws.Cells["M" + i].Value = "ถอดยาง SET";
                            }
                            else
                            {
                                ws.Cells["M" + i].Value = "ไม่ถอดยาง SET";
                            }
                            i++;
                        }
                    }
                    package.SaveAs(new FileInfo("wwwroot/files/import/template_bp/" + fileName));
                    string newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/import/template_bp/");
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