using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.ApproveManage.ApproveFlow
{
    public class IndexModel : PageModel
    {
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
                        id = "F.Id",
                        groupDescription = "M.GroupDescription",
                        approveMasterId = "ApproveMasterId",
                        approveLevel = "ApproveLevel",
                        employeeId = "EmployeeId",
                        name = "Name",
                        company = "Company",
                        email = "Email",
                        backupEmail = "BackupEmail",
                        canApprove = "CanApprove",
                        isFinalApprove = "IsFinalApprove",
                        receiveWhenComplete = "ReceiveWhenComplete",
                        receiveWhenFailed = "ReceiveWhenFailed",
                        remark = "Remark",
                        isActive = "F.IsActive"
                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<ApproveFlowGridModel>(@"
                        SELECT F.Id,M.GroupDescription
                          ,ApproveMasterId
                          ,ApproveLevel
                          ,EmployeeId
                          ,Name
                          ,Company
                          ,Email
                          ,BackupEmail
                          ,CanApprove
                          ,IsFinalApprove
                          ,ReceiveWhenComplete
                          ,ReceiveWhenFailed
                          ,Remark
                          ,F.IsActive
                          ,F.IsSkipAlert
                        FROM TB_ApproveFlow F JOIN
                        TB_ApproveMaster M ON F.ApproveMasterId = M.Id
                        WHERE " + filter + @" ORDER BY ApproveMasterId,ApproveLevel
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
    }
}
