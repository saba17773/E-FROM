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
    public class EditModel : PageModel
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

        public EditModel(
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

        private async Task<List<SelectListItem>> GetUserMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);

                var userAll = await userRepo.GetAllAsync();

                unitOfWork.Complete();

                return userAll.Where(x => x.IsActive == 1).Select(x => new SelectListItem {
                    Value = x.Id.ToString(),
                    Text = x.Username,
                }).ToList();
            }
        }

        private List<SelectListItem> GetCreditControlType()
        {
            var allType = typeof(CreditControlTypeModel).GetProperties();

            return allType.Select(x => new SelectListItem {
                Value = x.Name,
                Text = x.Name
            }).ToList();
        }

        public async Task OnGetAsync(int id)
        {
            try
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

                    var approveMapping = await approveMappingRepo.GetAsync(id);
                    ApproveMappingMaster = approveMapping;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveMappingRepo = new GenericRepository<CreditControlApproveMappingTable>(unitOfWork.Transaction);

                    var approveMapping = await approveMappingRepo.GetAsync(id);

                    approveMapping.CCType = ApproveMappingMaster.CCType;
                    approveMapping.ApproveMasterId = ApproveMappingMaster.ApproveMasterId;
                    approveMapping.CreateBy = ApproveMappingMaster.CreateBy;

                    await approveMappingRepo.UpdateAsync(approveMapping);

                    unitOfWork.Complete();

                    AlertSuccess = "Edit Success.";
                    return Redirect($"/CreditControl/ApproveMapping/{id}/Edit");
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                return Redirect($"/CreditControl/ApproveMapping/{id}/Edit");
            }
        }

    }
}