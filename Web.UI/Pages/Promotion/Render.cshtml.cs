using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Promotion
{
    public class RenderTransModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public int CCId { get; set; }
        public string RefExp { get; set; }
        public string LevelName1 { get; set; }
        public string LevelName2 { get; set; }
        public string LevelName3 { get; set; }
        public string LevelName4 { get; set; }
        public string LevelName5 { get; set; }
        public string LevelName6 { get; set; }
        public string LevelName7 { get; set; }
        public string PositionName1 { get; set; }
        public string PositionName2 { get; set; }
        public string PositionName3 { get; set; }
        public string PositionName4 { get; set; }
        public string PositionName5 { get; set; }
        public string PositionName6 { get; set; }
        public string PositionName7 { get; set; }
        public string StatusName1 { get; set; }
        public string StatusName2 { get; set; }
        public string StatusName3 { get; set; }
        public string StatusName4 { get; set; }
        public string StatusName5 { get; set; }
        public string StatusName6 { get; set; }
        public string StatusName7 { get; set; }
        public string ReqDateStr { get; set; }
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }
        [BindProperty]
        public PromotionDOMTable Promotion_DOM { get; set; }
        public List<PromotionAttachFileTable> AttachFile { get; set; }
        public List<PromotionApproveTransTable> ApproveTrans { get; set; }
        public List<SelectListItem> PromotionTypeByProductMaster { get; set; }
        public List<SelectListItem> PromotionTypeByMaster { get; set; }
        public List<SelectListItem> PromotionGroupCustomerMaster { get; set; }
        public List<SelectListItem> PromotionTypeFromByMaster { get; set; }
        public List<SelectListItem> PromotionTransMaster { get; set; }
        public List<ApproveFlowTable> ApproveFlows { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public RenderTransModel(
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

        // public void OnGet(int id)
        // {
        //     CCId = id;
        // }

        private async Task InitialDataAsync(int id)
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
                FromDateStr = Convert.ToDateTime(dom.FromDate).ToString("dd/MM/yyyy");
                ToDateStr = Convert.ToDateTime(dom.ToDate).ToString("dd/MM/yyyy");
                ReqDateStr = Convert.ToDateTime(dom.RequestDate).ToString("dd/MM/yyyy");

                if (dom.PromotionRef != 0)
                {
                    var promotionDOMEXPRepo = new GenericRepository<PromotionDOMTable>(unitOfWork.Transaction);
                    var domexp = await promotionDOMEXPRepo.GetAsync(dom.PromotionRef);
                    RefExp = domexp.RequestNumber;
                }
                else
                {
                    RefExp = "";
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
                if (ApproveFlows.Count >= 1)
                {
                    LevelName2 = ApproveFlows[0].Name;
                    PositionName2 = ApproveFlows[0].Position;
                    StatusName2 = ApproveFlows[0].Status;
                }
                if (ApproveFlows.Count >= 2)
                {
                    LevelName3 = ApproveFlows[1].Name;
                    PositionName3 = ApproveFlows[1].Position;
                    StatusName3 = ApproveFlows[0].Status;
                }
                if (ApproveFlows.Count >= 3)
                {
                    LevelName4 = ApproveFlows[2].Name;
                    PositionName4 = ApproveFlows[2].Position;
                    StatusName4 = ApproveFlows[0].Status;
                }
                if (ApproveFlows.Count >= 4)
                {
                    LevelName5 = ApproveFlows[3].Name;
                    PositionName5 = ApproveFlows[3].Position;
                    StatusName5 = ApproveFlows[0].Status;
                }
                if (ApproveFlows.Count >= 5)
                {
                    LevelName6 = ApproveFlows[4].Name;
                    PositionName6 = ApproveFlows[4].Position;
                    StatusName6 = ApproveFlows[0].Status;
                }
                if (ApproveFlows.Count >= 6)
                {
                    LevelName7 = ApproveFlows[5].Name;
                    PositionName7 = ApproveFlows[5].Position;
                    StatusName7 = ApproveFlows[0].Status;
                }

                unitOfWork.Complete();
            }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                await InitialDataAsync(id);

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Promotion");
            }

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