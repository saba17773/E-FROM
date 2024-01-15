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

namespace Web.UI.Pages.Queing.Master.Role
{
    public class PermissionModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }

        [Required]
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public List<PermissionTable> Permission { get; set; }

        [Required]
        [BindProperty]
        public string[] QueingPermission { get; set; }

        private IDatabaseContext _databaseContext;
        private IAuthService _authService;

        public PermissionModel(
          IDatabaseContext databaseContext,
          IAuthService authService)
        {
            _databaseContext = databaseContext;
            _authService = authService;
        }
        public async Task<IActionResult> OnGetAsync(int roleid)
        {
            await GetData(roleid);

            return Page();
        }
        public async Task GetData(int roleid)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var roleRepo = new GenericRepository<RoleTable>(unitOfWork.Transaction);
                var permissionRepo = new GenericRepository<PermissionTable>(unitOfWork.Transaction);

                var permissionAll = await permissionRepo.GetAllAsync();

                Permission = permissionAll.Where(x => x.RoleId == roleid).ToList();

                var role = await roleRepo.GetAsync(roleid);
                RoleName = role.Description;
                RoleId = role.Id;

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnPostAsync(int roleid)
        {
            await GetData(roleid);

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var permissionRepo = new GenericRepository<PermissionTable>(unitOfWork.Transaction);

                    var permissionAll = await permissionRepo.GetAllAsync();

                    foreach (var item in QueingPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.QUEING)
                            });
                        }
                    }

                    foreach (var item in permissionAll)
                    {
                        if (!QueingPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.QUEING) && item.RoleId == roleid)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Set Permission Success.";

                    return Redirect("/Queing/Master/Role");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Master/Role/{roleid}/Permission");
            }
        }
    }
}
