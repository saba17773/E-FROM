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
using Web.UI.Interfaces;

namespace Web.UI.Pages
{
    public class ChangePasswordModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [Required]
        [BindProperty]
        [DisplayName("Old Password")]
        public string OldPassword { get; set; }

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
        private IAuthService _authService;

        public ChangePasswordModel(
          IDatabaseContext databaseContext,
          IAuthService authService)
        {
            _databaseContext = databaseContext;
            _authService = authService;
        }

        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Login");
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
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

                    var user = await userRepo.GetAsync(_authService.GetClaim().UserId);

                    if (_authService.HashPassword(OldPassword, user.Salt) != user.Password)
                    {
                        AlertError = "Old Password Incorrect.";
                        return Redirect("/ChangePassword");
                    }

                    var salt = Guid.NewGuid();

                    user.Password = _authService.HashPassword(NewPassword, salt.ToString());
                    user.Salt = salt.ToString();

                    await userRepo.UpdateAsync(user);

                    unitOfWork.Complete();

                    AlertSuccess = "Change Password Success.";
                    return Redirect("/ChangePassword");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/ChangePassword");
            }
        }

    }
}