using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl.ApproveMaster
{
    public class EditModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public ApproveMasterTable ApproveMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private ICreditControlService _creditControlService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public EditModel(
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

        private void InitialData()
        {
            ApproveMaster = new ApproveMasterTable();
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                InitialData();

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                    var approveMaster = await approveMasterRepo.GetAsync(id);

                    ApproveMaster.GroupDescription = approveMaster.GroupDescription;
                    ApproveMaster.IsActive = approveMaster.IsActive;

                    unitOfWork.Complete();

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($@"/CreditControl/ApproveMaster/{id}/Edit");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    InitialData();

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                    var approveMaster = await approveMasterRepo.GetAsync(id);

                    approveMaster.GroupDescription = ApproveMaster.GroupDescription;
                    approveMaster.IsActive = ApproveMaster.IsActive;

                    await approveMasterRepo.UpdateAsync(approveMaster);

                    unitOfWork.Complete();
                }

                AlertSuccess = "Edit Successful!";
                return Redirect($@"/CreditControl/ApproveMaster/{id}/Edit");
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($@"/CreditControl/ApproveMaster/{id}/Edit");
            }
        }
    }
}