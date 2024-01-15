using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Template
{
    public class ExportAllTransactionModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        public ExportAllTransactionModel(
         IDatabaseContext databaseContext,
         IDatatableService datatablesService,
         IAuthService authService)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _authService = authService;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnGetDownloadAllTrans(string pSearch, string pStatus, string pRequestNo)
        {
            try
            {
                string pathInfo;
                string fileName;
                pathInfo = "wwwroot/files/export/S2E/Template/AllTransactionMain.xlsx";

                FileInfo template = new FileInfo(pathInfo);
                var datenow = System.DateTime.Now.ToString("ddMMyyyyHHmmss");

                fileName = "All_Transaction_" + datenow + ".xlsx";

                string fileName2 = "All_Transaction.xlsx";

                using (var package = new ExcelPackage(template))
                {
                    var workbook = package.Workbook;
                    var ws = workbook.Worksheets["main"];

                    //ws.Column(46).Hidden = true;

                    using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection())) {

                        var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EAllTransactionGridModel>(@"
                            EXEC S2EAllTransactionMain @pSearch,@pStatus,@pRequestNo
                        ", new { @pSearch = pSearch, @pStatus = pStatus, @pRequestNo = pRequestNo }, unitOfWork.Transaction);

                        var currentData = data.ToList();
                        int i = 2;
                        foreach (var subject in currentData)
                        {
                            ws.Cells["A" + i].Value = subject.REQUESTCODE;
                            ws.Cells["B" + i].Value = subject.REQUESTNO;
                            ws.Cells["C" + i].Value = subject.ITEMNAMEREF;
                            ws.Cells["D" + i].Value = subject.ITEMNAME;
                            ws.Cells["E" + i].Value = subject.DEALER;
                            ws.Cells["F" + i].Value = subject.PROCESS;
                            ws.Cells["G" + i].Value = subject.STATUSAPPROVE;
                            ws.Cells["H" + i].Value = subject.ACTIONBY;
                            ws.Cells["I" + i].Value = subject.QTY;
                            ws.Cells["J" + i].Value = subject.UNIT;
                            ws.Cells["K" + i].Value = subject.TESTINGPLANT;
                            ws.Cells["L" + i].Value = subject.REMARKALLTRANS;
                            ws.Cells["M" + i].Value = subject.SAMPLETYPE;
                            ws.Cells["N" + i].Value = subject.SAMPLELOCATION;
                            ws.Cells["O" + i].Value = subject.SAMPLELOCATIONCONTACT;

                            i++;
                        }

                        var YearNow = @DateTime.Now.ToString("yyyy", new CultureInfo("en-US"));
                        var MonthNow = @DateTime.Now.ToString("MM", new CultureInfo("en-US"));

                        var subFolder = MonthNow + YearNow;
                        string basePath = "wwwroot/files/export/S2E/tmpAllTransactionMain";

                        if (!System.IO.Directory.Exists(basePath))
                        {
                            System.IO.Directory.CreateDirectory(basePath);
                        }

                        package.SaveAs(new FileInfo(basePath + "/" + fileName2));

                        string newPath = Path.Combine(Directory.GetCurrentDirectory(), basePath + "/");
                        IFileProvider provider = new PhysicalFileProvider(newPath);
                        IFileInfo fileInfo = provider.GetFileInfo(fileName2);
                        var readStream = fileInfo.CreateReadStream();
                        var mimeType = "application/vnd.ms-excel";

                        unitOfWork.Complete();
                        return File(readStream, mimeType, fileName);

                    }
                }

            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/AllTransaction");
            }
        }
    }
}
