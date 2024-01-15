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
    public class AddModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        public int _Id { get; set; }

        [BindProperty]
        public S2EApproveFlow_TB ApproveFlowTable { get; set; }

        [BindProperty]
        public bool CanKeyinWhenApprove { get; set; }
        [BindProperty]
        public bool CanApprove { get; set; }
        [BindProperty]
        public bool IsFinalApprove { get; set; }
        [BindProperty]
        public bool ReceiveWhenComplete { get; set; }
        [BindProperty]
        public bool ReceiveWhenFailed { get; set; }

        // DI
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
        public void OnGet(int id)
        {
            _Id = id;
        }
        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                _Id = id;

                ApproveFlowTable.ApproveMasterId = id;

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);

                    //var approveFlow = await unitOfWork.CreditControl.GetApproveFlowByCCIdAsync(id);

                    var newApproveFlow = new S2EApproveFlow_TB
                    {
                        ApproveMasterId = ApproveFlowTable.ApproveMasterId,
                        ApproveLevel = ApproveFlowTable.ApproveLevel,
                        Email = ApproveFlowTable.Email,
                        Company = ApproveFlowTable.Company,
                        BackupEmail = ApproveFlowTable.BackupEmail,
                        EmployeeId = ApproveFlowTable.EmployeeId,
                        Name = ApproveFlowTable.Name,
                        LastName = ApproveFlowTable.LastName,
                        Remark = ApproveFlowTable.Remark,
                        IsActive = ApproveFlowTable.IsActive,
                        CanApprove = CanApprove == true ? 1 : 0,
                        IsFinalApprove = IsFinalApprove == true ? 1 : 0,
                        ReceiveWhenComplete = ReceiveWhenComplete == true ? 1 : 0,
                        ReceiveWhenFailed = ReceiveWhenFailed == true ? 1 : 0,
                        IsKeyinWhenApprove = CanKeyinWhenApprove == true ? 1 : 0
                    };

                    await approveFlowRepo.InsertAsync(newApproveFlow);

                    unitOfWork.Complete();

                    AlertSuccess = "Add Approve Flow Success.";
                    return Redirect($"/S2E/ApproveFlow/{_Id}/View");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                _Id = id;

                return Page();
            }
        }
    }
}
