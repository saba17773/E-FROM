using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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
    public class CopyModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        public string CopyFrom { get; set; }

        [Required]
        [BindProperty]
        public string Description { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;

        public CopyModel(
          IDatabaseContext databaseContext,
          IDatatableService datatablesService,
          IAuthService authService)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _authService = authService;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            
            await _authService.CanAccess(nameof(RolePermissionModel.COPY_ROLE));

            await GetData(id);

            return Page();
        }

        public async Task GetData(int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var roleRepo = new GenericRepository<RoleTable>(unitOfWork.Transaction);
                var role = await roleRepo.GetAsync(id);

                CopyFrom = role.Description;

                unitOfWork.Complete();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _authService.CanAccess(nameof(RolePermissionModel.COPY_ROLE));

                if (!ModelState.IsValid)
                {
                    await GetData(id);

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var permissionRepo = new GenericRepository<PermissionTable>(unitOfWork.Transaction);
                    var roleRepo = new GenericRepository<RoleTable>(unitOfWork.Transaction);

                    var newRole = await roleRepo.InsertAsync(new RoleTable
                    {
                        Description = Description,
                        IsActive = 1
                    });

                    var permissionAll = await permissionRepo.GetAllAsync();
                    var permission = permissionAll.Where(x => x.RoleId == id);

                    foreach (var item in permission)
                    {
                        item.RoleId = (int)newRole;
                        await permissionRepo.InsertAsync(item);
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Copy Role Success.";

                    return Redirect("/Role/" + id + "/Copy");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Role/" + id + "/Copy");
            }
        }
    }
}