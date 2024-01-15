using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Master.Role
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string AddRoleDesc { get; set; }
        [BindProperty]
        public int AddRoleisActive { get; set; }
        [BindProperty]
        public int EditRoleId { get; set; }
        [BindProperty]
        public string EditRoleDesc { get; set; }
        [BindProperty]
        public int EditRoleisActive { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public IndexModel(
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
            await _authService.CanAccess(nameof(QueingPermissionModel.ADMIN_QUEING));
            return Page();
        }
        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        Description = "description"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<RoleTable>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT *
                            FROM TB_Role
                            WHERE ProjectId = 2	
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
        public async Task<IActionResult> OnPostAsync(string add, string edit)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var RoleRepo = new GenericRepository<RoleTable>(unitOfWork.Transaction);

                    if (!string.IsNullOrEmpty(add))
                    {
                        await RoleRepo.InsertAsync(new RoleTable
                        {
                            Description = AddRoleDesc,
                            ProjectId = ProjectMasterModel.Queing,
                            IsActive = AddRoleisActive == 1 ? 1 : 0 
                        });

                        AlertSuccess = "Add Role Success";
                    }

                    if (!string.IsNullOrEmpty(edit))
                    {
                        var RoleById = await RoleRepo.GetAsync(EditRoleId);

                        RoleById.Description = EditRoleDesc;
                        RoleById.IsActive = EditRoleisActive == 1 ? 1 : 0 ;

                        await RoleRepo.UpdateAsync(RoleById);

                        AlertSuccess = "Edit Role Success";
                    }

                    unitOfWork.Complete();
                    return Redirect($"/Queing/Master/Role");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Master/Role");
            }
        }
        public async Task<IActionResult> OnGetDeleteRoleAsync(int roleid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        DELETE FROM TB_Role 
                        WHERE Id = {roleid}
                    ", null, unitOfWork.Transaction);

                    await unitOfWork.Transaction.Connection.ExecuteAsync($@"
                        DELETE FROM TB_Permission 
                        WHERE RoleId = {roleid}
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    AlertSuccess = "Delete Role Success";
                    return Redirect("/Queing/Master/Role");
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
