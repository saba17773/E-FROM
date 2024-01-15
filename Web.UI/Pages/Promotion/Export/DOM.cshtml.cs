using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

namespace Web.UI.Pages.Promotion.Export
{
    [AllowAnonymous]
    public class DOMModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public int CCId { get; set; }
        public string RefExp { get; set; }
        public string HeaderType { get; set; }
        public string ReqDateStr { get; set; }
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }
        public string LevelName1 { get; set; }
        public string LevelName2 { get; set; }
        public string LevelName3 { get; set; }
        public string LevelName4 { get; set; }
        public string LevelName5 { get; set; }
        public string LevelName6 { get; set; }
        public string LevelName7 { get; set; }
        public string LevelName8 { get; set; }
        public string PositionName1 { get; set; }
        public string PositionName2 { get; set; }
        public string PositionName3 { get; set; }
        public string PositionName4 { get; set; }
        public string PositionName5 { get; set; }
        public string PositionName6 { get; set; }
        public string PositionName7 { get; set; }
        public string PositionName8 { get; set; }
        public string StatusName1 { get; set; }
        public string StatusName2 { get; set; }
        public string StatusName3 { get; set; }
        public string StatusName4 { get; set; }
        public string StatusName5 { get; set; }
        public string StatusName6 { get; set; }
        public string StatusName7 { get; set; }
        public string StatusName8 { get; set; }
        public string remarkTrans1 { get; set; }
        public string remarkTrans2 { get; set; }
        public string remarkTrans3 { get; set; }
        public string remarkTrans4 { get; set; }
        public string remarkTrans5 { get; set; }
        public string remarkTrans6 { get; set; }
        public string remarkTrans7 { get; set; }
        public string remarkTrans8 { get; set; }
        public string txtPromotionConditions { get; set; }
        public string txtObjective { get; set; }
        [BindProperty]
        public PromotionDOMTable Promotion_DOM { get; set; }
        public List<PromotionAttachFileTable> AttachFile { get; set; }
        public List<PromotionApproveTransTable> ApproveTrans { get; set; }
        public List<ApproveFlowTable> ApproveFlows { get; set; }
        public List<SelectListItem> PromotionTypeByProductMaster { get; set; }
        public int isExpDate { get; set; }
        public List<SelectListItem> PromotionTypeByMaster { get; set; }
        public List<SelectListItem> PromotionGroupCustomerMaster { get; set; }
        public List<SelectListItem> PromotionTypeFromByMaster { get; set; }
        public List<SelectListItem> PromotionTransMaster { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public DOMModel(
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
        // private async Task InitialDataAsync(int id)
        {
            Promotion_DOM = new PromotionDOMTable();
            PromotionTypeByProductMaster = await GetPromotionTypeByProductMasterAsync();
            PromotionTypeByMaster = await GetPromotionTypeByMasterAsync();
            PromotionGroupCustomerMaster = await GetPromotionGroupCustomerMasterAsync();
            PromotionTypeFromByMaster = await GetPromotionTypeFromByMasterAsync();

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var promotionDOMRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                var dom = await promotionDOMRepo.GetAsync(id);

                Promotion_DOM = dom;
                txtPromotionConditions = dom.PromotionConditions.Replace("\n","<br>");
                txtObjective = dom.Objective.Replace("\n","<br>");
                
                if (dom.PromotionRef != 0)
                {
                    HeaderType = "Exceptional Promotion Request";
                    var promotionDOMEXPRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                    var domexp = await promotionDOMEXPRepo.GetAsync(dom.PromotionRef);
                    RefExp = domexp.RequestNumber;
                }
                else
                {
                    HeaderType = "Promotion Request";
                    RefExp = "";
                }

                FromDateStr = Convert.ToDateTime(dom.FromDate).ToString("dd/MM/yyyy");
                ToDateStr = Convert.ToDateTime(dom.ToDate).ToString("dd/MM/yyyy");
                ReqDateStr = Convert.ToDateTime(dom.RequestDate).ToString("dd/MM/yyyy");

                DateTime date1 = Convert.ToDateTime(dom.RequestDate);
                DateTime date2 = new DateTime(2021, 10, 7, 0, 0, 0);
                int result = DateTime.Compare(date1, date2);
                if (result < 0)
                {
                    isExpDate = 1;
                }
                else
                {
                    isExpDate = 0;
                }

                var approveTransRepo = new GenericRepository<PromotionApproveTransTable>(unitOfWork.Transaction);
                var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

                AttachFile = new List<PromotionAttachFileTable>();
                var attachFile = await unitOfWork.Promotion.GetFileByCCIdAsync(id, Promotion_DOM.RequestType);
                AttachFile = attachFile.ToList();

                ApproveTrans = new List<PromotionApproveTransTable>();
                var approveTrans = await unitOfWork.Promotion.GetApproveTransByCCId(id);
                ApproveTrans = approveTrans.ToList();

                ApproveFlows = new List<ApproveFlowTable>();
                var approveFlows = await unitOfWork.Promotion.GetApproveTransFlowByCCId(id);
                ApproveFlows = approveFlows.ToList();

                var createBy = await unitOfWork.Promotion.GetCreateBy(id);

                if (ApproveFlows.Count >= 1)
                {
                    LevelName1 = createBy.Name + " " + createBy.LastName;
                    PositionName1 = "Proposed by";
                    StatusName1 = "Proposed by";
                }
                if (ApproveTrans.Count >= 1)
                {
                    LevelName2 = ApproveTrans[0].Name;
                    PositionName2 = ApproveTrans[0].Position;
                    StatusName2 = ApproveTrans[0].Status;
                    if (!String.IsNullOrEmpty(ApproveTrans[0].Remark)){
                        remarkTrans1 = ApproveTrans[0].Remark.Replace("\n","<br>");
                    }
                }
                if (ApproveTrans.Count >= 2)
                {
                    LevelName3 = ApproveTrans[1].Name;
                    PositionName3 = ApproveTrans[1].Position;
                    StatusName3 = ApproveTrans[1].Status;
                    if (!String.IsNullOrEmpty(ApproveTrans[1].Remark)){
                        remarkTrans2 = ApproveTrans[1].Remark.Replace("\n","<br>");
                    }
                }
                if (ApproveTrans.Count >= 3)
                {
                    LevelName4 = ApproveTrans[2].Name;
                    PositionName4 = ApproveTrans[2].Position;
                    StatusName4 = ApproveTrans[2].Status;
                    if (!String.IsNullOrEmpty(ApproveTrans[2].Remark)){
                        remarkTrans3 = ApproveTrans[2].Remark.Replace("\n","<br>");
                    }
                }
                if (ApproveTrans.Count >= 4)
                {
                    LevelName5 = ApproveTrans[3].Name;
                    PositionName5 = ApproveTrans[3].Position;
                    StatusName5 = ApproveTrans[3].Status;
                    if (!String.IsNullOrEmpty(ApproveTrans[3].Remark)){
                        remarkTrans4 = ApproveTrans[3].Remark.Replace("\n","<br>");
                    }
                }
                if (ApproveTrans.Count >= 5)
                {
                    LevelName6 = ApproveTrans[4].Name;
                    PositionName6 = ApproveTrans[4].Position;
                    StatusName6 = ApproveTrans[4].Status;
                    if (!String.IsNullOrEmpty(ApproveTrans[4].Remark)){
                        remarkTrans5 = ApproveTrans[4].Remark.Replace("\n","<br>");
                    }
                }
                if (ApproveTrans.Count >= 6)
                {
                    LevelName7 = ApproveTrans[5].Name;
                    PositionName7 = ApproveTrans[5].Position;
                    StatusName7 = ApproveTrans[5].Status;
                    if (!String.IsNullOrEmpty(ApproveTrans[5].Remark)){
                        remarkTrans6 = ApproveTrans[5].Remark.Replace("\n","<br>");
                    }
                }
                if (ApproveTrans.Count >= 7)
                {
                    LevelName8 = ApproveTrans[6].Name;
                    PositionName8 = ApproveTrans[6].Position;
                    StatusName8 = ApproveTrans[6].Status;
                    if (!String.IsNullOrEmpty(ApproveTrans[6].Remark))
                    {
                        remarkTrans7 = ApproveTrans[6].Remark.Replace("\n", "<br>");
                    }
                }


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

            var doc = converter.ConvertUrl($"{_configuration["Config:BaseUrl"]}/Promotion/Export/DOM/{id}");

            var fileName = "wwwroot/files/promotion/print_dom.pdf";

            doc.Save(fileName);
            doc.Close();

            var stream = new FileStream(fileName, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf");
        }

        public async Task<List<SelectListItem>> GetPromotionTypeByProductMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var customerTypeByProductRepo = new GenericRepository<PromotionByProductTable>(unitOfWork.Transaction);

                var customerTypeByProdictAll = await customerTypeByProductRepo.GetAllAsync();

                unitOfWork.Complete();

                return customerTypeByProdictAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.ByCode + " (" + x.ByName + ")"
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetPromotionTypeByMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var promotionTypeRepo = new GenericRepository<PromotionTypeTable>(unitOfWork.Transaction);

                var promotionTypeAll = await promotionTypeRepo.GetAllAsync();

                return promotionTypeAll
                    .Where(x => x.ByType == "DOM")
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.PromotionType
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetPromotionGroupCustomerMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var promotionGroupCustomerRepo = new GenericRepository<PromotionGroupCustomerTable>(unitOfWork.Transaction);

                var promotionGroupCustomerAll = await promotionGroupCustomerRepo.GetAllAsync();

                return promotionGroupCustomerAll
                    .Where(x => x.ByType == "DOM")
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.GroupName
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetPromotionTypeFromByMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var promotionTypeFromRepo = new GenericRepository<PromotionTypeFromTable>(unitOfWork.Transaction);

                var promotionFromTypeAll = await promotionTypeFromRepo.GetAllAsync();

                return promotionFromTypeAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.PromotionFrom
                    })
                    .ToList();
            }
        }

    }
}