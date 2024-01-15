using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.ApproveManage.ApproveMaster
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public ApproveMasterTable ApproveMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        public IndexModel(
         IDatabaseContext databaseContext,
         IDatatableService datatablesService,
         IAuthService authService)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _authService = authService;
        }

        public async Task OnGet()
        {
            await _authService.CanAccess(nameof(ApproveMasterPermissionModel.VIEW_APPROVE_MASTER));
        }

        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        id = "Id",
                        groupDesc = "GroupDescription",
                        isActive = "IsActive"
                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<ApproveMasterGridModel>(@"
                        SELECT
                        Id,
                        GroupDescription,
                        IsActive
                        FROM TB_ApproveMaster
                        WHERE " + filter + @"
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.Format(Request, data.ToList()));
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
                    var approveFlowRepo = new GenericRepository<ApproveFlow_TB>(unitOfWork.Transaction);

                    var approveMaster = await approveMasterRepo.GetAsync(id);
                    var approveFlow = await unitOfWork.VenderControl.GetApproveFlowByApproveMasterIdAsync(id);

                    if (approveMaster.IsActive == 1)
                    {
                        AlertError = "Approve master in use.";
                        return Redirect("/ApproveManage/ApproveMaster");
                    }

                    await approveMasterRepo.DeleteAsync(approveMaster);

                    foreach (var item in approveFlow)
                    {
                        await approveFlowRepo.DeleteAsync(item);
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Delete Success.";

                    return Redirect("/ApproveManage/ApproveMaster");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/ApproveManage/ApproveMaster");
            }
        }
    }
}
