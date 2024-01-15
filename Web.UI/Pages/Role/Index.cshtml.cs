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

namespace Web.UI.Pages.Role
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
            await _authService.CanAccess(nameof(RolePermissionModel.VIEW_ROLE));
        }

        public async Task<JsonResult> OnPostGridAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var roleRepo = new GenericRepository<RoleTable>(unitOfWork.Transaction);

                var field = new
                {
                    id = "Id",
                    role = "Description",
                    isActive = "IsActive"
                };

                var filter = _datatablesService.Filter(HttpContext.Request, field);

                var role = await unitOfWork.Transaction.Connection.QueryAsync<RoleTable>(@"
                    SELECT *
                    FROM TB_Role
                    WHERE " + filter + @"
                ", null, unitOfWork.Transaction);

                unitOfWork.Complete();

                return new JsonResult(_datatablesService.Format(Request, role.ToList()));
            }
        }
    }
}