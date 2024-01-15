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
    public class AddModel : PageModel
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

        public AddModel(
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

        public void OnGet()
        {
            InitialData();

            ApproveMaster.IsActive = 1;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    InitialData();

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                    var newApproveMaster = new ApproveMasterTable { 
                        GroupDescription = ApproveMaster.GroupDescription,
                        IsActive = ApproveMaster.IsActive
                    };

                    await approveMasterRepo.InsertAsync(newApproveMaster);

                    unitOfWork.Complete();
                }

                AlertSuccess = "Add Successful!";
                return Redirect($@"/CreditControl/ApproveMaster/Add");
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($@"/CreditControl/ApproveMaster/Add");
            }
        }

    }
}