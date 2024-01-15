using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using SelectPdf;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Memo.Export
{
    [AllowAnonymous]
    public class ReportModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public string requestDate { get; set; }
        public string txtAttn { get; set; }
        public string txtSubject { get; set; }
        public string txtCustomername { get; set; }
        public string txtDescription { get; set; }
        public string txtRemark { get; set; }
        public string txtCreateBy { get; set; }
        public int CountItem { get; set; }

        [BindProperty]

        public MemoTable Memo { get; set; }
        public List<MemoAttachFileTable> AttachFile { get; set; }
        public List<MemoItemTable> MemoItem { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public ReportModel(
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
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Memo = new MemoTable();
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var memoRepo = new GenericRepository<MemoTable>(unitOfWork.Transaction);
                var memoAttnRepo = new GenericRepository<MemoAttnTable>(unitOfWork.Transaction);
                var memoSubjectRepo = new GenericRepository<MemoSubjectTable>(unitOfWork.Transaction);
                var memoCustomerRepo = new GenericRepository<MemoCustomerTable>(unitOfWork.Transaction);
                var memo = await memoRepo.GetAsync(id);
                var memoAttn = await memoAttnRepo.GetAsync(memo.AttnId);
                var memoSubject = await memoSubjectRepo.GetAsync(memo.SubjectId);
                var custcode = await unitOfWork.Memo.GetCustomerByCodeAsync(memo.CustomerCode);

                Memo = memo;
                requestDate = Convert.ToDateTime(memo.MemoDate).ToString("dd/MM/yyyy");
                txtAttn = memoAttn.MemoAttn;
                txtSubject = memoSubject.MemoSubject;
                txtCustomername = custcode.CustomerName;
                txtDescription = memo.Description.Replace("\n", "<br>");
                txtRemark = memo.Remark == null ? "" : memo.Remark.Replace("\n", "<br>");

                AttachFile = new List<MemoAttachFileTable>();
                var attachFile = await unitOfWork.Memo.GetFileByCCIdAsync(id);
                AttachFile = attachFile.ToList();

                var createBy = await unitOfWork.Memo.GetCreateBy(id);
                txtCreateBy = createBy.Name + " " + createBy.LastName;

                MemoItem = new List<MemoItemTable>();
                var memoItem = await unitOfWork.Memo.GetItemByCCIdAsync(id);
                MemoItem = memoItem.ToList();
                CountItem = MemoItem.Count();

                unitOfWork.Complete();
                return Page();
            }
        }
        public async Task<IActionResult> OnGetPDF(int id)
        {
            Memo = new MemoTable();
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var memoRepo = new GenericRepository<MemoTable>(unitOfWork.Transaction);
                var memoAttnRepo = new GenericRepository<MemoAttnTable>(unitOfWork.Transaction);
                var memoSubjectRepo = new GenericRepository<MemoSubjectTable>(unitOfWork.Transaction);
                var memoCustomerRepo = new GenericRepository<MemoCustomerTable>(unitOfWork.Transaction);
                var memo = await memoRepo.GetAsync(id);
                var memoAttn = await memoAttnRepo.GetAsync(memo.AttnId);
                var memoSubject = await memoSubjectRepo.GetAsync(memo.SubjectId);
                var custcode = await unitOfWork.Memo.GetCustomerByCodeAsync(memo.CustomerCode);

                Memo = memo;
                requestDate = Convert.ToDateTime(memo.MemoDate).ToString("dd/MM/yyyy");
                txtAttn = memoAttn.MemoAttn;
                txtSubject = memoSubject.MemoSubject;
                txtCustomername = custcode.CustomerName;
                txtDescription = memo.Description.Replace("\n", "<br>");
                txtRemark = memo.Remark == null ? "": memo.Remark.Replace("\n", "<br>");

                AttachFile = new List<MemoAttachFileTable>();
                var attachFile = await unitOfWork.Memo.GetFileByCCIdAsync(id);
                AttachFile = attachFile.ToList();

                var createBy = await unitOfWork.Memo.GetCreateBy(id);
                txtCreateBy = createBy.Name + " " + createBy.LastName;

                MemoItem = new List<MemoItemTable>();
                var memoItem = await unitOfWork.Memo.GetItemByCCIdAsync(id);
                MemoItem = memoItem.ToList();
                CountItem = MemoItem.Count();

                unitOfWork.Complete();
            }

            var converter = new HtmlToPdf();

            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 30;
            converter.Options.MarginRight = 30;
            converter.Options.MarginTop = 2;
            converter.Options.MarginBottom = 7;

            using (var conn = _databaseContext.GetConnection())
            {
                var _table = conn.Query<MemoGridViewModel>(@"
                        SELECT 
                        E.Name,E.LastName
                        FROM TB_Memo T
                        LEFT JOIN TB_User U ON T.CreateBy = U.Id
                        LEFT JOIN TB_Employee E ON U.EmployeeId = E.EmployeeId
                        WHERE T.Id=@Id
                    ", new { @Id = id }).ToList();
                foreach (var item in _table)
                {
                    txtCreateBy = item.Name + " " + item.LastName;
                }

                var _tableItem = conn.Query<MemoItemTable>(@"
                        SELECT 
                        *
                        FROM TB_MemoItem
                        WHERE MemoId=@Id
                        AND Cancel=1
                    ", new { @Id = id }).ToList();
                CountItem = _tableItem.Count();
            }

            // headder settings
            converter.Options.DisplayHeader = true;
            converter.Header.DisplayOnOddPages = true;
            converter.Header.DisplayOnEvenPages = true;
            converter.Header.DisplayOnFirstPage = true;
            converter.Header.Height = 210;

            PdfTextSection text = new PdfTextSection(0, 10, "Page {page_number} /{total_pages}  ", new Font("Arial", 8));
            text.HorizontalAlign = PdfTextHorizontalAlign.Right;
            converter.Header.Add(text);

            var body_headder = "";

            body_headder += " <table width='100%' cellpadding='7' style='line-height: 0.8;'> ";
            body_headder += " <tr><td valign='top' width='100%' align='center'><img src='D:/DEV/Memo/wwwroot/images/DSC.jpg' width='900px;' height='110px;'></td></tr> ";
            body_headder += " </table> ";
            body_headder += " <div style='font-size:24px;text-align:center';><b>INTERNAL MEMO</b></div>";
            body_headder += " <table width='100%' cellpadding='7' style='line-height: 0.8;'> ";
            body_headder += " <tr><td valign='top' style='font-family:Browallia New; font-size:30px;'><b>No : </b>" + Memo.MemoNumber + "</td></tr> ";
            body_headder += " <tr><td valign='top' style='font-family:Browallia New; font-size:30px;'><b>Date : </b>" + requestDate + "</td></tr> ";
            body_headder += " <tr><td valign='top' style='font-family:Browallia New; font-size:30px;'><b>Attn : </b>" + txtAttn + "</td></tr> ";
            body_headder += " <tr><td valign='top' style='font-family:Browallia New; font-size:30px;'><b>Subject : </b>" + txtSubject + "</td></tr> ";
            body_headder += " <tr><td valign='top' style='font-family:Browallia New; font-size:30px;'><b>Customer Code : </b>" + Memo.CustomerCode + "</td></tr> ";
            body_headder += " <tr><td valign='top' style='font-family:Browallia New; font-size:30px;'><b>Customer Name : </b>" + txtCustomername + "</td></tr> ";
            body_headder += " </table> ";

            //body_headder += "<table  width='100%' cellpadding='5' style='border-collapse: collapse;'>";
            //body_headder += " <tr style='white-space:nowrap; background-color:#E0E0E0;' > ";
            //body_headder += " <td style='width:10%; font-family:Cordia New; font-size:28px;' > Item Number</td> ";
            //body_headder += " <td style='width:60%; font-family:Cordia New; font-size:28px;' > Item Name</td> ";
            //body_headder += " <td style='width:10%; font-family:Cordia New; font-size:28px;' > Quantity</td> ";
            //body_headder += " <td style='width:5%; font-family:Cordia New; font-size:28px;' > Unit</td> ";
            //body_headder += " <td style='width:5%; font-family:Cordia New; font-size:28px;' > ยกเลิก</td> ";
            //body_headder += " <td style='width:5%; font-family:Cordia New; font-size:28px;' > ผลิตแล้ว</td> ";
            //body_headder += " <td style='width:5%; font-family:Cordia New; font-size:28px;' > ยังไม่ผลิต</td> ";
            //body_headder += " </tr> ";
            //body_headder += " </table> ";

            PdfHtmlSection customHtml_signature = new PdfHtmlSection(body_headder, string.Empty);
            converter.Header.Add(customHtml_signature);

            //footer settings
            converter.Options.DisplayFooter = true;
            converter.Footer.Height = 330;

            var body_signature = "";
            body_signature += "<table width='100%' cellpadding='8' style='text-align: center; font-size: 30px; font-family:Browallia New;'>";
            body_signature += "<tr><td width='50%'>Requested By</td><td>Approved By</td></tr>";
            body_signature += "<tr><td><br><br><div style='margin-left: 120px; width: 50%; border-bottom: 1px solid #333333; font-size: 30px;'></div>Global Sales</td><td><br><br><div style='margin-left: 120px; width: 50%; border-bottom: 1px solid #333333; font-size: 10px;'></div>Global Regional, Sales Manager</td></tr>";
            body_signature += "<tr><td>Approved By</td><td>Approved By</td></tr>";
            body_signature += "<tr><td><br><br><div style='margin-left: 120px; width: 50%; border-bottom: 1px solid #333333; font-size: 30px;'></div>Global Sales Administration Manager</td><td><br><br><div style='margin-left: 120px; width: 50%; border-bottom: 1px solid #333333; font-size: 10px;'></div>Production Planning Manager</td></tr>";
            body_signature += "<tr><td>Approved By</td><td>Approved By</td></tr>";
            body_signature += "<tr><td><br><br><div style='margin-left: 120px; width: 50%; border-bottom: 1px solid #333333; font-size: 30px;'></div> Commercial Control Sr.Manager</td><td><br><br><div style='margin-left: 120px; width: 50%; border-bottom: 1px solid #333333; font-size: 10px;'></div>Chief Marketing Officer</td></tr>";
            body_signature += "</table>";

            body_signature += "<table width='100%' cellpadding='8' style='text-align: center; font-family:Browallia New; font-size: 30px;'><tr><td width='30%' style='text-align: left;'>Created by " + txtCreateBy + "<div class='mb-3' style='margin-left: 110px;width: 20%; border-bottom: 1px solid #333333; font-size: 10px;'></div></td></tr></table>";

            PdfHtmlSection customHtml = new PdfHtmlSection(body_signature, string.Empty);
            converter.Footer.Add(customHtml);

            if (Memo.Id == 5)
            {
                converter.Footer.DisplayOnEvenPages = true;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnFirstPage = false;
            }
            else if (CountItem > 0 && CountItem < 3 && Memo.Description.Length > 450)
            {
                converter.Footer.DisplayOnEvenPages = true;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnFirstPage = false;   
            }
            else if (CountItem == 0  && Memo.Description.Length > 450 || Memo.Remark.Length > 450)
            {
                converter.Footer.DisplayOnEvenPages = true;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnFirstPage = false;
            }
            else if (CountItem > 0 && CountItem < 3 && Memo.Description.Length < 450)
            {
                converter.Footer.DisplayOnEvenPages = true;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnFirstPage = true;
            }
            else if (CountItem > 2 && CountItem < 32)
            {
                converter.Footer.DisplayOnEvenPages = true;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnFirstPage = false;
            }
            else if (CountItem > 31 && CountItem < 39)
            {
                converter.Footer.DisplayOnEvenPages = false;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnFirstPage = false;
            }
            else if (CountItem > 38 && CountItem < 50)
            {
                converter.Footer.DisplayOnEvenPages = false;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnFirstPage = false;
            }
            else
            {
                converter.Footer.DisplayOnEvenPages = false;
                converter.Footer.DisplayOnOddPages = false;
                converter.Footer.DisplayOnFirstPage = true;
            }

            var doc = converter.ConvertUrl($"{_configuration["Config:BaseUrl"]}/Memo/Export/Report/{id}");

            var fileName = "wwwroot/files/memo/print_memo.pdf";

            doc.Save(fileName);
            doc.Close();

            var stream = new FileStream(fileName, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf");
        }
    }
}