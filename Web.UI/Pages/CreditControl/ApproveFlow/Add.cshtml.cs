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
    public class AddModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        public int _Id { get; set; }

        [BindProperty]
        public ApproveFlowTable ApproveFlowTable { get; set; }

        [BindProperty]
        public bool CanEditCreditLimit { get; set; }


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
                    var approveFlowRepo = new GenericRepository<ApproveFlowTable>(unitOfWork.Transaction);

                    //var approveFlow = await unitOfWork.CreditControl.GetApproveFlowByCCIdAsync(id);

                    var newApproveFlow = new ApproveFlowTable
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
                        EditCreditLimit = CanEditCreditLimit == true ? 1 : 0,
                        IsFile = ApproveFlowTable.IsFile,
                        Position = ApproveFlowTable.Position,
                        Status = ApproveFlowTable.Status
                    };

                    await approveFlowRepo.InsertAsync(newApproveFlow);

                    unitOfWork.Complete();

                    AlertSuccess = "Add Approve Flow Success.";
                    return Redirect($"/CreditControl/ApproveFlow/{_Id}/Add");
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