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
    public class AddModel : PageModel
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
        private IDatatableService _datatablesService;
        private IAuthService _authService;

        public AddModel(
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
            await _authService.CanAccess(nameof(RolePermissionModel.ADD_ROLE));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(RolePermissionModel.ADD_ROLE));

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var roleRepo = new GenericRepository<RoleTable>(unitOfWork.Transaction);
                    await roleRepo.InsertAsync(new RoleTable
                    {
                        Description = Description,
                        IsActive = IsActive
                    });

                    unitOfWork.Complete();

                    AlertSuccess = "Add Role Success.";

                    return Redirect("/Role/Add");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Role/Add");
            }
        }
    }
}