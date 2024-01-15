using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Promotion.ApproveMapping
{
    public class AddModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public PromotionApproveMappingTable ApproveMapping { get; set; }
        public List<SelectListItem> ApproveMappingApproveMaster { get; set; }
        public List<SelectListItem> ApproveMappingUserMaster { get; set; }
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public AddModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IEmailService emailService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }

        private async Task InitialDataAsync()
        {
            ApproveMapping = new PromotionApproveMappingTable();
            ApproveMappingApproveMaster = await GetApproveMappingApproveMasterAsync();
            ApproveMappingUserMaster = await GetApproveMappingUserMasterAsync();
        }

        public async Task OnGetAsync()
        {
            await InitialDataAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approvemappingRepo = new GenericRepository<PromotionApproveMappingTable>(unitOfWork.Transaction);
                    var newMapping = new PromotionApproveMappingTable
                    {
                        CCType = ApproveMapping.CCType,
                        ApproveMasterId = ApproveMapping.ApproveMasterId,
                        CreateBy = ApproveMapping.CreateBy,
                        TypeProduct = ApproveMapping.TypeProduct
                    };
                    var addMapping = await approvemappingRepo.InsertAsync(newMapping);

                    unitOfWork.Complete();

                    AlertSuccess = "Add ApproveMapping Success.";

                    return Redirect("/Promotion/ApproveMapping");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Promotion/ApproveMapping/Add");
            }
        }
        public async Task<List<SelectListItem>> GetApproveMappingApproveMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var ApproveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                var approveMasterAll = await ApproveMasterRepo.GetAllAsync();

                return approveMasterAll
                    .Where(x => x.IsActive == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.GroupDescription,
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetApproveMappingUserMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var UserMasterRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);

                var userMasterAll = await UserMasterRepo.GetAllAsync();

                return userMasterAll
                    .Where(x => x.IsActive == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Username,
                    })
                    .ToList();
            }
        }

    }
}