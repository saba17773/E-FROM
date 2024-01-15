using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl.ApproveMaster
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public ApproveMasterTable ApproveMaster { get; set; }

        // DI
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;

        public IndexModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
        }

        private void InitialData()
        {
            ApproveMaster = new ApproveMasterTable();
        }

        public void OnGet()
        {
            InitialData();
        }

        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                    var approveMasterAll = await approveMasterRepo.GetAllAsync();

                    return new JsonResult(_datatableService.FormatOnce(approveMasterAll.OrderBy(x => x.Id).ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnGetDeleteAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);
                    var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

                    var approveMaster = await approveMasterRepo.GetAsync(id);
                    var approveFlow = await unitOfWork.CreditControl.GetApproveFlowByApproveMasterIdAsync(id);

                    if (approveMaster.IsActive == 1)
                    {
                        AlertError = "Approve master in use.";
                        return Redirect("/CreditControl/ApproveMaster");
                    }

                    await approveMasterRepo.DeleteAsync(approveMaster);
                    foreach (var item in approveFlow)
                    {
                        await approveFlowRepo.DeleteAsync(item);
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Delete Success.";

                    return Redirect("/CreditControl/ApproveMaster");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/CreditControl/ApproveMaster");
            }
        }
    }
}