using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SelectPdf;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Infrastructure.ViewModels.CreditControl;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl.Export
{
    [AllowAnonymous]
    public class OVSModel : PageModel
    {
        [TempData]
        public string AlertError { get; set; }

        public ViewOVSViewModel OvsData { get; set; }
        public string RequestDate { get; set; }
        public List<CreditControl_ApproveRemarkModel> ApproveRemark { get; set; }
        public List<ApprovalRemarkViewModel> ApprovalRemark { get; set; }
        public RequesterInfoViewModel RequesterInfo { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public OVSModel(
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

        public async Task<IActionResult> OnGet(int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var tx = unitOfWork.Transaction;

                var OVSDATA = await unitOfWork.CreditControl.GetOVSDataByCCIdAsync(id);

                var reqDate = Convert.ToDateTime(OVSDATA.RequestDate).ToString("dd/MM/yyyy");

                if (OVSDATA == null)
                {
                    AlertError = @"ไม่พยข้อมูล";
                    return Page();
                }

                OvsData = OVSDATA;
                RequestDate = reqDate;

                // remark
                var approveTrans = await unitOfWork.CreditControl.GetApproveTransByCCId(id);
                var approveTransByLevel = approveTrans.OrderBy(x => x.ApproveLevel).ToList();

                List<CreditControl_ApproveRemarkModel> approveRemark = new List<CreditControl_ApproveRemarkModel>();

                foreach (var item in approveTransByLevel)
                {
                    var approveName = await tx.Connection.GetAsync<ApproveFlowTable>(item.ApproveFlowId, tx);
                    approveRemark.Add(new CreditControl_ApproveRemarkModel
                    {
                        Remark = item.Remark,
                        Name = approveName?.Name ?? ""
                    });
                }

                ApproveRemark = approveRemark;

                var _approvalRemark = await unitOfWork.CreditControl.GetApprovalRemarkAsync(id);
                ApprovalRemark = _approvalRemark.ToList();

                RequesterInfo = await unitOfWork.CreditControl.GetRequesterInfoAsync(id);

                unitOfWork.Complete();

                return Page();
            }
        }

        public IActionResult OnGetPDF(int id)
        {
            var converter = new HtmlToPdf();

            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 5;
            converter.Options.MarginRight = 5;
            converter.Options.MarginTop = 5;
            converter.Options.MarginBottom = 5;

            var doc = converter.ConvertUrl($"{_configuration["Config:BaseUrl"]}/CreditControl/Export/OVS/{id}");

            var fileName = "wwwroot/files/credit_control/print_ovs.pdf";

            doc.Save(fileName);
            doc.Close();

            var stream = new FileStream(fileName, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf");
        }
    }
}
