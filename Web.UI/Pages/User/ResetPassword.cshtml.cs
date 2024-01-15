using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Web.UI.Pages.User
{
    public class ResetPasswordModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [Required]
        [BindProperty]
        [StringLength(32)]
        [MinLength(8)]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [Required]
        [BindProperty]
        [StringLength(32)]
        [MinLength(8)]
        [DisplayName("Confirm New Password")]
        public string ConfirmNewPassword { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;

        public ResetPasswordModel(
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
            await _authService.CanAccess(nameof(UserPermissionModel.RESET_PASSWORD_USER));
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _authService.CanAccess(nameof(UserPermissionModel.RESET_PASSWORD_USER));

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                if (NewPassword != ConfirmNewPassword)
                {
                    ModelState.AddModelError("ConfirmNewPassword", "The Password didn't match.");

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);

                    var salt = Guid.NewGuid();

                    var user = await userRepo.GetAsync(id);

                    user.Password = _authService.HashPassword(NewPassword, salt.ToString());
                    user.Salt = salt.ToString();

                    await userRepo.UpdateAsync(user);

                    unitOfWork.Complete();

                    AlertSuccess = "Reset Password Success";

                    return Redirect("/User/" + id + "/ResetPassword");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/User/" + id + "/ResetPassword");
            }
        }
    }
}