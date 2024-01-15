using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Template
{
    public class ExportReportStockCardModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        public ExportReportStockCardModel(
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
        public async Task<IActionResult> OnGetDownloadReportStockCardExcel(string pSearch, string pStatus, string pRequestNo)
        {
            try
            {
                string pathInfo;
                string fileName;
                pathInfo = "wwwroot/files/export/S2E/Template/ReportStockCard.xlsx";

                FileInfo template = new FileInfo(pathInfo);
                var datenow = System.DateTime.Now.ToString("ddMMyyyyHHmmss");

                fileName = "ReportStockCard_" + datenow + ".xlsx";
                string fileName2 = "ReportStockCard.xlsx";
                using (var package = new ExcelPackage(template))
                {
                    var workbook = package.Workbook;
                    var ws = workbook.Worksheets["main"];
                    using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                    {

                        var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EReportStockCardGridModel>(@"
                            EXEC S2EReportStockCard", null , unitOfWork.Transaction);

                        var currentData = data.ToList();
                        int i = 4;
                        decimal tmpQtycal = 0;

                        var val = 0;
                        var color = "1";


                        foreach (var v in currentData)
                        {
                            if (v.MOVEMENT == "IN")
                            {
                                tmpQtycal = v.QTY;
                            }
                            else
                            {
                                tmpQtycal = tmpQtycal - v.QTY;
                            }

                            if (v.NEWREQUESTID != val)
                            {
                                val = v.NEWREQUESTID;
                                if (color == "1")
                                {
                                    color = "0";
                                }
                                else
                                {
                                    color="1";
                                }
                            }

                            ws.Cells["A" + i].Value = v.POSTINGDATE;
                            ws.Cells["B" + i].Value = v.REQUESTCODE;
                            ws.Cells["C" + i].Value = v.DOCNO;
                            ws.Cells["D" + i].Value = v.PLANT;
                            ws.Cells["E" + i].Value = v.REQUESTBY;
                            ws.Cells["F" + i].Value = v.MOVEMENT;
                            ws.Cells["G" + i].Value = v.SAMPLETYPE;
                            ws.Cells["H" + i].Value = v.ITEMCODE;
                            ws.Cells["I" + i].Value = v.ITEMNAME;
                            ws.Cells["J" + i].Value = v.QTY;
                            ws.Cells["K" + i].Value = v.UNIT;
                            ws.Cells["L" + i].Value = tmpQtycal;

                            ws.Row(i).Style.Fill.PatternType = ExcelFillStyle.Solid;
                            if (color == "1")
                            {
                                ws.Cells["A" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                                ws.Cells["B" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                                ws.Cells["C" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                                ws.Cells["D" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                                ws.Cells["E" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                                ws.Cells["F" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                                ws.Cells["G" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                                ws.Cells["H" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                                ws.Cells["I" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                                ws.Cells["J" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                                ws.Cells["K" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                                ws.Cells["L" + i].Style.Fill.BackgroundColor.SetColor(Color.Bisque);
                            }
                            else
                            {
                                ws.Cells["A" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                ws.Cells["B" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                ws.Cells["C" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                ws.Cells["D" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                ws.Cells["E" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                ws.Cells["F" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                ws.Cells["G" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                ws.Cells["H" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                ws.Cells["I" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                ws.Cells["J" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                ws.Cells["K" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                ws.Cells["L" + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                            }

                            i++;
                        }

                        var YearNow = @DateTime.Now.ToString("yyyy", new CultureInfo("en-US"));
                        var MonthNow = @DateTime.Now.ToString("MM", new CultureInfo("en-US"));

                        var subFolder = MonthNow + YearNow;
                        string basePath = "wwwroot/files/export/S2E/tmpReportStockCard";

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
                return Redirect("/S2E/Report/StockCard");
            }
        }
    }
}
