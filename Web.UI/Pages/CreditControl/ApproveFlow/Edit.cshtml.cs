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

namespace Web.UI.Pages.CreditControl.ApproveFlow
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
        public ApproveFlowTable ApproveFlow { get; set; }

        public int FlowId { get; set; }

        [BindProperty]
        public bool CanEditCreditLimit { get; set; }

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
            ApproveFlow = new ApproveFlowTable();
        }

        public async Task OnGet(int id, int flowId)
        {
            ApproveFlowId = id;
            FlowId = flowId;

            InitialData();

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

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
                ApproveFlow.EditCreditLimit = approveFlow.EditCreditLimit;
                ApproveFlow.IsFile = approveFlow.IsFile;
                ApproveFlow.Position = approveFlow.Position;
                ApproveFlow.Status = approveFlow.Status;
                unitOfWork.Complete();
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
                    var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

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
                    approveFlow.EditCreditLimit = CanEditCreditLimit == true ? 1 : 0;
                    approveFlow.IsFile = ApproveFlow.IsFile;
                    approveFlow.Position = ApproveFlow.Position;
                    approveFlow.Status = ApproveFlow.Status;

                    await approveFlowRepo.UpdateAsync(approveFlow);

                    unitOfWork.Complete();

                    AlertSuccess = "Edit Successful!";
                    return Redirect($"/CreditControl/ApproveFlow/{id}/Edit?flowId={FlowId}");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/CreditControl/ApproveFlow/{id}/Edit?flowId={FlowId}");
            }
        }
    }
}