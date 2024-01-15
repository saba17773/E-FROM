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
    public class EditModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [Required]
        [StringLength(100)]
        [BindProperty]
        public string Description { get; set; }

        [Required]
        [BindProperty]
        public int IsActive { get; set; }

        private IDatabaseContext _databaseContext;
        private IAuthService _authService;

        public EditModel(
          IDatabaseContext databaseContext,
          IAuthService authService)
        {
            _databaseContext = databaseContext;
            _authService = authService;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            try
            {
                await _authService.CanAccess(nameof(RolePermissionModel.EDIT_ROLE));

                await GetData(id);

                return Page();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _authService.CanAccess(nameof(RolePermissionModel.EDIT_ROLE));

                if (!ModelState.IsValid)
                {
                    await GetData(id);

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var roleRepo = new GenericRepository<RoleTable>(unitOfWork.Transaction);

                    var role = await roleRepo.GetAsync(id);
                    role.Description = Description;
                    role.IsActive = IsActive;

                    await roleRepo.UpdateAsync(role);

                    unitOfWork.Complete();

                    AlertSuccess = "Edit Role Success.";

                    return Redirect($"/Role/{id}/Edit");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Role/{id}/Edit");
            }
        }

        public async Task GetData(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var roleRepo = new GenericRepository<RoleTable>(unitOfWork.Transaction);
                    var role = await roleRepo.GetAsync(id);

                    Description = role.Description;
                    IsActive = role.IsActive;

                    unitOfWork.Complete();
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}