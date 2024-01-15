using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Interfaces;

namespace Web.UI.Pages
{
    public class InstallModel : PageModel
    {
        private IDatabaseContext _databaseContext;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public InstallModel(
          IDatabaseContext databaseContext,
          IAuthService authService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _authService = authService;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string initialUsername = "administrator";
            string initialPassword = "administrator";

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);

                    var userAll = await userRepo.GetAllAsync();
                    var user = userAll.Where(x => x.Username == initialUsername).FirstOrDefault();

                    if (user != null)
                    {
                        return NotFound();
                    }

                    var salt = Guid.NewGuid();

                    var addUserAdmin = await userRepo.InsertAsync(new UserTable
                    {
                        Username = initialUsername,
                        Password = _authService.HashPassword(initialPassword, salt.ToString()),
                        Salt = salt.ToString(),
                        RoleId = 1,
                        IsActive = 1
                    });

                    unitOfWork.Complete();

                    return Redirect("/Login");
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}