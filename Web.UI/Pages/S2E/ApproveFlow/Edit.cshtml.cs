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
    public class EditModel : PageModel
    {
        [TempData]
        public string AlertError { get; set; }

        [TempData]
        public string AlertSuccess { get; set; }

        [BindProperty]
        public int ApproveFlowId { get; set; }

        [BindProperty]
        public S2EApproveFlow_TB ApproveFlow { get; set; }

        public int FlowId { get; set; }

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

        public EditModel(
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
            ApproveFlow = new S2EApproveFlow_TB();
        }
        public async Task OnGet(int id, int flowId)
        {
            ApproveFlowId = id;
            FlowId = flowId;

            InitialData();

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var approveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);

                var approveFlow = await approveFlowRepo.GetAsync(id);

                ApproveFlow.Email = approveFlow.Email;
                ApproveFlow.BackupEmail = approveFlow.BackupEmail;
                ApproveFlow.IsActive = approveFlow.IsActive;
                ApproveFlow.ApproveLevel = approveFlow.ApproveLevel;
                ApproveFlow.Name = approveFlow.Name;
                ApproveFlow.LastName = approveFlow.LastName;
                ApproveFlow.BackupEmail = approveFlow.BackupEmail;
                ApproveFlow.Company = approveFlow.Company;
                ApproveFlow.EmployeeId = approveFlow.EmployeeId;
                ApproveFlow.Remark = approveFlow.Remark;
                ApproveFlow.CanApprove = approveFlow.CanApprove;
                ApproveFlow.IsFinalApprove = approveFlow.IsFinalApprove;
                ApproveFlow.ReceiveWhenFailed = approveFlow.ReceiveWhenFailed;
                ApproveFlow.ReceiveWhenComplete = approveFlow.ReceiveWhenComplete;
                ApproveFlow.IsKeyinWhenApprove = approveFlow.IsKeyinWhenApprove;

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostAsync(int id,int FlowId)
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
                    var approveFlowRepo = new GenericRepository<S2EApproveFlow_TB>(unitOfWork.Transaction);

                    var approveFlow = await approveFlowRepo.GetAsync(id);

                    approveFlow.ApproveLevel = ApproveFlow.ApproveLevel;
                    approveFlow.Email = ApproveFlow.Email;
                    approveFlow.BackupEmail = ApproveFlow.BackupEmail;
                    approveFlow.IsActive = ApproveFlow.IsActive;
                    approveFlow.Name = ApproveFlow.Name;
                    approveFlow.Company = ApproveFlow.Company;
                    approveFlow.EmployeeId = ApproveFlow.EmployeeId;
                    approveFlow.LastName = ApproveFlow.LastName;
                    approveFlow.Remark = ApproveFlow.Remark;
                    approveFlow.IsKeyinWhenApprove = CanKeyinWhenApprove == true ? 1 : 0;
                    approveFlow.CanApprove = CanApprove == true ? 1 : 0;
                    approveFlow.IsFinalApprove = IsFinalApprove == true ? 1 : 0;
                    approveFlow.ReceiveWhenComplete = ReceiveWhenComplete == true ? 1 : 0;
                    approveFlow.ReceiveWhenFailed = ReceiveWhenFailed == true ? 1 : 0;

                    await approveFlowRepo.UpdateAsync(approveFlow);

                    unitOfWork.Complete();

                    AlertSuccess = "Edit Successful!";
                    return Redirect($"/S2E/ApproveFlow/{FlowId}/View");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/S2E/ApproveFlow/{id}/Edit?flowId={FlowId}");
            }
        }
    }
}
