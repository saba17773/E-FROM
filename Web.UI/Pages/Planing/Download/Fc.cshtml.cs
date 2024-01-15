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
    public class FcModel : PageModel
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

        public FcModel(
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

        public void OnGet(int year, int version, string custgroup, int quarter)
        {
        }

        public async Task<IActionResult> OnGetDownLoadFC(int year, int version, string custgroup, int quarter, int month)
        {
            try
            {
                string pathInfo;
                string fileName;
                if (custgroup == "OVS")
                {
                    pathInfo = "wwwroot/files/import/template_fc/download/fcovs.xlsx";
                    fileName = "FC_" + custgroup + "_year" + year + "_quarter" + quarter + "_version" + version + ".xlsx";
                }
                else
                {
                    pathInfo = "wwwroot/files/import/template_fc/download/fcdom.xlsx";
                    fileName = "FC_" + custgroup + "_year" + year + "_month" + month + "_version" + version + ".xlsx";
                }
                FileInfo template = new FileInfo(pathInfo);

                using (var package = new ExcelPackage(template))
                {
                    if (custgroup == "OVS")
                    {
                        var workbook = package.Workbook;
                        var ws = workbook.Worksheets["Sheet1"];

                        int[] quarterlist = new int[] { };
                        string[] columnlistqty = new string[] { "I", "K", "M" };
                        string[] columnlistamount = new string[] { "J", "L", "N" };

                        if (quarter == 1)
                        {
                            quarterlist = new int[] { 1, 2, 3 };
                        }
                        if (quarter == 2)
                        {
                            quarterlist = new int[] { 4, 5, 6 };
                        }
                        if (quarter == 3)
                        {
                            quarterlist = new int[] { 7, 8, 9 };
                        }
                        if (quarter == 4)
                        {
                            quarterlist = new int[] { 10, 11, 12 };
                        }

                        for (int i = 0; i < quarterlist.Length; i++)
                        {
                            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                            {
                                var monthtemp = quarterlist[i];

                                var datafc = await unitOfWork.Transaction.Connection.QueryAsync($@"
                                    SELECT * FROM TB_TicketTemplateForcast 
                                    WHERE Year = {year}
                                    AND VersionId = {version}
                                    AND CustGroup = '{custgroup}'
                                    AND Quarter = {quarter}
                                    AND Month = {monthtemp}
                                ", null, unitOfWork.Transaction);
                                unitOfWork.Complete();

                                int ovs = 2;
                                foreach (var item in datafc)
                                {
                                    if (i == 0)
                                    {
                                        ws.Cells["A" + ovs].Value = "FC";
                                        ws.Cells["B" + ovs].Value = item.CustGroup;
                                        ws.Cells["C" + ovs].Value = item.Year;
                                        ws.Cells["D" + ovs].Value = item.ItemId;
                                        ws.Cells["E" + ovs].Value = item.ItemName;
                                        ws.Cells["F" + ovs].Value = item.ProductGroup;
                                        ws.Cells["G" + ovs].Value = item.SubGroup;
                                        ws.Cells["H" + ovs].Value = item.ProductType;
                                        ws.Cells[columnlistqty[i] + ovs].Value = item.Qty;
                                        ws.Cells[columnlistamount[i] + ovs].Value = item.Amount;
                                    }
                                    if (i == 1)
                                    {
                                        ws.Cells[columnlistqty[i] + ovs].Value = item.Qty;
                                        ws.Cells[columnlistamount[i] + ovs].Value = item.Amount;
                                    }
                                    if (i == 2)
                                    {
                                        ws.Cells[columnlistqty[i] + ovs].Value = item.Qty;
                                        ws.Cells[columnlistamount[i] + ovs].Value = item.Amount;
                                    }
                                    ovs++;
                                }
                            }
                        }
                    }
                    else
                    {
                        var workbook = package.Workbook;
                        var ws = workbook.Worksheets["Sheet1"];

                        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                        {
                            var datafcdom = await unitOfWork.Transaction.Connection.QueryAsync($@"
                                SELECT * FROM TB_TicketTemplateForcast 
                                WHERE Year = {year}
                                AND VersionId = {version}
                                AND CustGroup = '{custgroup}'
                                AND Month = {month}
                            ", null, unitOfWork.Transaction);
                            unitOfWork.Complete();

                            int dom = 2;
                            foreach (var item in datafcdom)
                            {

                                ws.Cells["A" + dom].Value = "FC";
                                ws.Cells["B" + dom].Value = item.CustGroup;
                                ws.Cells["C" + dom].Value = item.Year;
                                ws.Cells["D" + dom].Value = item.ItemId;
                                ws.Cells["E" + dom].Value = item.ItemName;
                                ws.Cells["F" + dom].Value = item.ProductGroup;
                                ws.Cells["G" + dom].Value = item.SubGroup;
                                ws.Cells["H" + dom].Value = item.ProductType;
                                ws.Cells["I" + dom].Value = item.Qty;
                                ws.Cells["J" + dom].Value = item.Amount;
                                ws.Cells["K" + dom].Value = item.QtyNext;
                                ws.Cells["L" + dom].Value = item.AmountNext;
                                dom++;
                            }

                        }
                    }

                    package.SaveAs(new FileInfo("wwwroot/files/import/template_fc/" + fileName));
                    string newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/import/template_fc/");
                    IFileProvider provider = new PhysicalFileProvider(newPath);
                    IFileInfo fileInfo = provider.GetFileInfo(fileName);
                    var readStream = fileInfo.CreateReadStream();
                    var mimeType = "application/vnd.ms-excel";
                    return File(readStream, mimeType, fileName);
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