using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.ApproveFlow
{
    public class ViewModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        public int ApproveMasterId { get; set; }

        // DI
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;

        public ViewModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
        }

        public void OnGet(int id)
        {
            ApproveMasterId = id;
        }
        public async Task<IActionResult> OnPostGridAsync(int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var approveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);

                var approveFlowAll = await approveFlowRepo.GetAllAsync();

                return new JsonResult(_datatableService.FormatOnce(
                    approveFlowAll.Where(x => x.ApproveMasterId == id).OrderBy(x => x.ApproveLevel).ToList()));
            }
        }
        public async Task<IActionResult> OnGetDeleteAsync(int id, int mid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);

                    var approveFlow = await approveFlowRepo.GetAsync(id);

                    await approveFlowRepo.DeleteAsync(approveFlow);

                    unitOfWork.Complete();

                    AlertSuccess = "Delete Success.";
                    return Redirect($"/S2E/ApproveFlow/{mid}/View");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/ApproveFlow/{mid}/View");
            }
        }
    }
}
