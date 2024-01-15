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

namespace Web.UI.Pages.Planing
{
    public class IndexFCModel : PageModel
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
        public IndexFCModel(
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

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(TicketPermissionModel.VIEW_MASTER_FC));

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Planing/IndexFC");
            }
        }

        public async Task<IActionResult> OnPostGridFCAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<BPGridViewModel>(@"
                        SELECT 
                            T.Year,
                            T.Month,
                            T.CustGroup,
                            T.Quarter,
                            T.VersionId,
                            MV.Version,
                            T.Remark,
                            MU.Username AS CreateName,
                            CONVERT(varchar, T.CreateDate, 103) AS CreateDate,
                            MUU.Username AS UpdateName,
                            CONVERT(varchar, T.UpdateDate, 103) AS UpdateDate
                        FROM TB_TicketTemplateForcast T
                        LEFT JOIN TB_TicketVersionMaster MV ON T.VersionId = MV.Id
                        LEFT JOIN TB_User MU ON T.CreateBy = MU.Id
                        LEFT JOIN TB_User MUU ON T.UpdateBy = MUU.Id
                        GROUP BY 
                            T.Year,
                            T.Month,
                            T.CustGroup,
                            T.Quarter,
                            T.VersionId,
                            MV.Version,
                            T.Remark,
                            MU.Username,
                            MUU.Username,
                            CONVERT(varchar, T.CreateDate, 103),
                            CONVERT(varchar, T.UpdateDate, 103)
                        ORDER BY T.Year, T.Month DESC
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnGetDeleteBPAsync(int year, int version, int type)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        DELETE FROM TB_TicketTemplateBP 
                        WHERE Year = {year}
                        AND VersionId = {version}
                        AND Type = {type}
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();
                    AlertSuccess = "Delete Success.";
                    return new JsonResult(new { DeleteBP = 1 });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<IActionResult> OnGetDeleteFCAsync(int year, int quarter, int version, int custgroup, int month)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    string cg = "";
                    if (custgroup == 2)
                    {
                        cg = "OVS";
                        await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                            DELETE FROM TB_TicketTemplateForcast 
                            WHERE Year = {year}
                            AND VersionId = {version}
                            AND CustGroup = '{cg}'
                            AND Quarter = {quarter}
                        ", null, unitOfWork.Transaction);
                    }
                    if (custgroup == 1)
                    {
                        cg = "DOM";
                        await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                            DELETE FROM TB_TicketTemplateForcast 
                            WHERE Year = {year}
                            AND VersionId = {version}
                            AND CustGroup = '{cg}'
                            AND Month = {month}
                        ", null, unitOfWork.Transaction);
                    }
                    if (custgroup == 3)
                    {
                        cg = "CLM";
                        await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                            DELETE FROM TB_TicketTemplateForcast 
                            WHERE Year = {year}
                            AND VersionId = {version}
                            AND CustGroup = '{cg}'
                            AND Month = {month}
                        ", null, unitOfWork.Transaction);
                    }

                    unitOfWork.Complete();
                    AlertSuccess = "Delete Success.";
                    return new JsonResult(new { DeleteFC = 1 });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }

        public async Task<IActionResult> OnGetDownLoadBPAsync(int year, int version, int type)
        {
            try
            {
                FileInfo template = new FileInfo("wwwroot/files/import/template_bp/bp.xlsx");
                using (var package = new ExcelPackage(template))
                {
                    var workbook = package.Workbook;
                    var ws = workbook.Worksheets["Sheet1"];
                    string[] Colums = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

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
                            // ws.Cells["A" + i].Value = item.CustomerCode.ToString();
                            // ws.Cells["B1"].Value = formbalance.YearLeft;
                            Console.WriteLine(item.CustomerCode);
                            i++;
                        }
                        ws.Cells["A3"].Value = "TEST";
                    }
                    package.SaveAs(new FileInfo("wwwroot/files/import/template_bp/bp.xlsx"));
                    var fileName = "bp.xlsx";
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