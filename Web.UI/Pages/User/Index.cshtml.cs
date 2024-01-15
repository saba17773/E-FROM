using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.User
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
            await _authService.CanAccess(nameof(UserPermissionModel.VIEW_USER));
        }

        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        id = "U.Id",
                        username = "U.Username",
                        email = "U.Email",
                        role = "R.Description",
                        isActive = " U.IsActive",
                        EmployeeId = " U.EmployeeId",
                        UserDomain = " U.UserDomain"
                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<UserGridModel>(@"
                        SELECT
                        U.Id,
                        U.Username,
                        U.Email,
                        R.Description AS Role,
                        U.IsActive,
                        U.EmployeeId,
                        U.UserDomain
                        FROM TB_User U
                        LEFT JOIN TB_Role R ON R.Id = U.RoleId
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
    }
}