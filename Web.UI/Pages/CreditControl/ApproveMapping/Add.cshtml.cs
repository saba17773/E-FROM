using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl.ApproveMapping
{
    public class AddModel : PageModel
    {

        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public CreditControlApproveMappingTable ApproveMappingMaster { get; set; }
        public List<SelectListItem> UserMaster { get; set; }
        public List<SelectListItem> ApproveMaster { get; set; }
        public List<SelectListItem> CreditControlTypeMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;

        public AddModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
        }

        private async Task InitialData()
        {
            ApproveMappingMaster = new CreditControlApproveMappingTable();
            UserMaster = await GetUserMasterAsync();
            CreditControlTypeMaster = GetCreditControlType();
        }

        public async Task OnGetAsync()
        {
            await InitialData();

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var approveMappingRepo = new GenericRepository<CreditControlApproveMappingTable>(unitOfWork.Transaction);
                var approveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                ApproveMaster = approveMasterRepo.GetAll()
                    .Where(x => x.IsActive == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.GroupDescription
                    }).ToList();
            }
        }

        private async Task<List<SelectListItem>> GetUserMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);

                var userAll = await userRepo.GetAllAsync();

                unitOfWork.Complete();

                return userAll.Where(x => x.IsActive == 1).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Username,
                }).ToList();
            }
        }

        private List<SelectListItem> GetCreditControlType()
        {
            var allType = typeof(CreditControlTypeModel).GetProperties();

            return allType.Select(x => new SelectListItem
            {
                Value = x.Name,
                Text = x.Name
            }).ToList();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    await InitialData();

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveMappingRepo = new GenericRepository<CreditControlApproveMappingTable>(unitOfWork.Transaction);

                    var newApproveMapping = new CreditControlApproveMappingTable { 
                        ApproveMasterId = ApproveMappingMaster.ApproveMasterId,
                        CCType = ApproveMappingMaster.CCType,
                        CreateBy = ApproveMappingMaster.CreateBy
                    };

                    await approveMappingRepo.InsertAsync(newApproveMapping);

                    unitOfWork.Complete();

                    AlertSuccess = "Add Success.";
                    return Redirect($"/CreditControl/ApproveMapping/Add");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }
    }
}