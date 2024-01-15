using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.Queing;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Master.All
{
    public class UserPermissionModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int UserId { get; set; }
        [BindProperty]
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public UserPermissionModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.ADD_QUEING));

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Master/All/UserPermission");
            }
        }
        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        ROLEDESC = "ROLEDESC",
                        USERNAME = "USERNAME",
                        EMPID = "EMPID",
                        EMAIL = "EMAIL",
                        FULLNAME = "FULLNAME"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<UserPermissionGridModel>(@"
                      SELECT *
                        FROM
                        (
	                       SELECT R.Id ROLEID
                                ,R.Description ROLEDESC
                                ,U.Id USERID 
                                ,U.Username USERNAME
                                ,U.EmployeeId EMPID
                                ,U.Email EMAIL
                                ,E.Name + ' ' + E.LastName AS FULLNAME
                            FROM TB_Role R JOIN
                            TB_User U ON R.Id = U.RoleId JOIN
                            TB_Employee E ON U.EmployeeId = E.EmployeeId
                            WHERE R.ProjectId = 2 AND U.IsActive = 1
                        )T
                        WHERE " + filter + @"
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<JsonResult> OnPostRoleGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        id = "id",
                        description = "description"
                    };

                    var filter = _datatableService.Filter(Request, field);

                    var ItemSample = await unitOfWork.Transaction.Connection.QueryAsync<RoleTable>(@"
                      SELECT *
                        FROM
                        (
	                       SELECT * FROM TB_Role WHERE ProjectId = 2
                        )T
                        WHERE " + filter + @" 
                        ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, ItemSample.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);

                    var UserById = await UserRepo.GetAsync(UserId);

                    UserById.RoleId = RoleId;

                    await UserRepo.UpdateAsync(UserById);

                    unitOfWork.Complete();

                    AlertSuccess = "Change User Permission Success";
                    return Redirect("/Queing/Master/All/UserPermission");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Master/All/UserPermission");
            }
        }
    }
}
