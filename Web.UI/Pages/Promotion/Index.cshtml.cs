using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SelectPdf;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Promotion
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int CcId { get; set; }
        [BindProperty]
        public string Remark { get; set; }
        public PromotionDOMTable Promotion_DOM { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public IndexModel(
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
                await _authService.CanAccess(nameof(PromotionPermissionModel.VIEW_PROMOTION));

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Promotion");
            }
        }

        public async Task<IActionResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<PromotionGridViewModel>(@"
                        SELECT 
                            P.Id,
                            P.RequestNumber,
                            P.Pattern,
                            P.RequestType,
                            P.CustomerName,
                            P.TypeOfProduct,
                            TP.ByName,
                            RS.[Description] AS RequestStatus,
                            P.CurrentApproveStep,
                            COUNT(CCAT.Id) AS TotalApproveStep
                        FROM TB_Promotion P
                        LEFT JOIN TB_PromotionByProduct TP ON P.TypeOfProduct = TP.Id
                        LEFT JOIN TB_RequestStatus RS ON RS.Id = P.RequestStatus
                        LEFT JOIN TB_PromotionApproveTrans CCAT ON CCAT.CCId = P.Id
                        WHERE RS.[Description] <> 'Complete Approve' AND RS.[Description] <> 'Cancel'
                        GROUP BY 
                            P.Id,
                            P.RequestNumber,
                            P.Pattern,
                            P.RequestType,
                            P.CustomerName,
                            P.TypeOfProduct,
                            TP.ByName,
                            RS.Description,
                            P.CurrentApproveStep
                        ORDER BY P.RequestNumber,P.Id DESC
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

        public async Task<IActionResult> OnGetDownload(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var attachFileRepo = new GenericRepository<PromotionAttachFileTable>(unitOfWork.Transaction);
                    var file = await attachFileRepo.GetAsync(id);

                    var filePath = $"{file.FilePath}/{file.FileName}";
                    if (!System.IO.File.Exists(filePath))
                    {
                        throw new Exception("File not found.");
                    }

                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/x-msdownload", file.FileName);
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Result");
            }

        }


        public async Task<IActionResult> OnGetReportDOMAsync(int id)
        {
            try
            {
                Promotion_DOM = new PromotionDOMTable();
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var promotionDOMRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                    var dom = await promotionDOMRepo.GetAsync(id);

                    Promotion_DOM = dom;

                    var approveTransRepo = new GenericRepository<PromotionApproveTransTable>(unitOfWork.Transaction);
                    var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

                    unitOfWork.Complete();

                    string basePath = $"wwwroot/files/promotion/dom/report/";
                    var fileName = basePath + "/" + id + ".pdf";
                    HtmlToPdf converter = new HtmlToPdf();
                    PdfDocument doc = converter.ConvertUrl("http://localhost:3001/Promotion/" + id + "/Render");
                    doc.Save($"{basePath}{id + ".pdf" }");
                    doc.Close();

                    // var doc = converter.ConvertHtmlString(_html);

                    // var fileName = "wwwroot/files/credit_control/print_dom.pdf";

                    // doc.Save(fileName);
                    // doc.Close();

                    var stream = new FileStream(fileName, FileMode.Open);
                    return new FileStreamResult(stream, "application/pdf");

                    // return new JsonResult(new { Report = 1 });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var promorepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);  

                    var PomoById = await promorepo.GetAsync(CcId);
                    var status = PomoById.RequestStatus;

                    PomoById.CancelRemark = Remark;
                    PomoById.RequestStatus = RequestStatusModel.Cancel;
                    PomoById.UpdateBy = _authService.GetClaim().UserId;
                    PomoById.UpdateDate = DateTime.Now;

                    if(status == RequestStatusModel.WaitingForApprove)
                    {
                        var transectionpomo = new GenericRepository<PromotionApproveTransTable>(unitOfWork.Transaction);
                        var transAll = await transectionpomo.GetAllAsync();
                        var trans = transAll.Where(x => x.CCId == CcId 
                        && x.SendEmailDate != null && x.ApproveDate == null
                        && x.RejectDate == null && x.IsDone == 0).FirstOrDefault();

                        var SendDate =Convert.ToDateTime(trans.SendEmailDate).ToString("MM/dd/yyyy hh:mm:ss");
                        var noncerpo = new GenericRepository<NonceTable>(unitOfWork.Transaction);
                        var nonceAll = await noncerpo.GetAllAsync();
                        var nonceBysendDate = nonceAll.Where(x => Convert.ToDateTime(x.CreateDate).ToString("MM/dd/yyyy hh:mm:ss") == SendDate && x.IsUsed == 0).FirstOrDefault();
                        if (nonceBysendDate != null)
                        {
                            nonceBysendDate.IsUsed = 1;
                            await noncerpo.UpdateAsync(nonceBysendDate);
                        }
                        else
                        {
                            AlertError = "Please Contact Admin! ";
                            return Redirect($"/Promotion");
                        }
                       
                    }
                  


                    await promorepo.UpdateAsync(PomoById);

                    AlertSuccess = "Cancel Request Success";



                    unitOfWork.Complete();
                    return Redirect($"/Promotion");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Promotion");
            }
        }

    }
}