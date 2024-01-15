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

namespace Web.UI.Pages.FixAssets.ApproveMapping
{
    public class AddModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public AssetsApproveMappingTable ApproveMapping { get; set; }
        public List<SelectListItem> ApproveMappingApproveMaster { get; set; }
        public List<SelectListItem> ApproveMappingUserMaster { get; set; }
        public List<SelectListItem> CompanyMaster { get; set; }
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
            ApproveMapping = new AssetsApproveMappingTable();
            ApproveMappingApproveMaster = await GetApproveMappingApproveMasterAsync();
            ApproveMappingUserMaster = await GetApproveMappingUserMasterAsync();
            CompanyMaster = await GetCompanyMasterAsync();
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
                    var approvemappingRepo = new GenericRepository<AssetsApproveMappingTable>(unitOfWork.Transaction);
                    var newMapping = new AssetsApproveMappingTable
                    {
                        CCType = ApproveMapping.CCType,
                        ApproveMasterId = ApproveMapping.ApproveMasterId,
                        CreateBy = ApproveMapping.CreateBy,
                        TypeProduct = ApproveMapping.TypeProduct,
                        CompanyId = ApproveMapping.CompanyId
                    };
                    var addMapping = await approvemappingRepo.InsertAsync(newMapping);

                    unitOfWork.Complete();

                    AlertSuccess = "Add ApproveMapping Success.";

                    return Redirect("/FixAssets/ApproveMapping");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/FixAssets/ApproveMapping/Add");
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

        public async Task<List<SelectListItem>> GetCompanyMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var companyRepo = new GenericRepository<Company>(unitOfWork.Transaction);

                var companyAll = await companyRepo.GetAllAsync();

                return companyAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CompanyId + " (" + x.CompanyName + ")",
                    })
                    .ToList();
            }
        }

    }
}