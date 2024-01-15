using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
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
  public class LoginModel : PageModel
  {
    [TempData]
    public string AlertSuccess { get; set; }

    [TempData]
    public string AlertError { get; set; }

    [BindProperty]
    [Required]
    public string Username { get; set; }

    [BindProperty]
    [Required]
    public string Password { get; set; }
    public string nonceKey { get; set; }
    // DI
    private IConfiguration _configuration;
    private IDatabaseContext _databaseContext;
    private IAuthService _authService;
    private IHttpContextAccessor _httpContextAccessor;

    public LoginModel(
        IDatabaseContext databaseContext,
        IAuthService authService,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
      _databaseContext = databaseContext;
      _authService = authService;
      _configuration = configuration;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> OnGet(int u)
    {
      nonceKey = Guid.NewGuid().ToString();

      if (HttpContext.User.Identity.IsAuthenticated)
      {
        return Redirect("/");
      }
      if (u > 0)
      {
        await LoginDomain(u, nonceKey);
        return Redirect("/");
      }
      
      return Page();
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
          var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);

          var users = await userRepo.GetAllAsync();

          var user = users
              .Where(x =>
                  x.Username == Username.Trim() &&
                  x.Password == _authService.HashPassword(Password, x.Salt) &&
                  x.IsActive == 1)
              .FirstOrDefault();

          if (user == null)
          {
            throw new Exception("Username or Password incorrect.");
          }

          var connString = "EA_APP";
          var projectid = 37;
          var hostName = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
          var Project_Name = "";
          using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection(connString)))
          {

            var testEA = await unitOfWork2.Transaction.Connection.QueryFirstOrDefaultAsync<EAAPP_ProjectMaster_TABLE>($@"
                                    SELECT * 
                                    FROM TB_PROJECT_MASTER
                                    WHERE PROJECT_ID =  @projectid
                                ", new { @projectid = projectid }, unitOfWork2.Transaction);

            Project_Name = testEA.PROJECT_NAME;

            var EmpCode = 0;
            if (user.EmployeeId == null)
            {
              EmpCode = 0;
            }
            else
            {
              EmpCode = Convert.ToInt32(user.EmployeeId);
            }

            var insertLog = unitOfWork2.Transaction.Connection.Execute(@"
                                        INSERT INTO TB_LOG_APP 
                                        (   EMP_CODE,
                                            USER_NAME,
                                            HOST_NAME,
                                            LOGIN_DATE,
                                            PROJECT_NAME
                                        ) 
                                        VALUES
                                        (
                                            @EMP_CODE,
                                            @USER_NAME,
                                            @HOST_NAME,
                                            @LOGIN_DATE,
                                            @PROJECT_NAME
                                        )
                                    ",
                       new
                       {
                         @EMP_CODE = EmpCode,
                         @USER_NAME = user.Username,
                         @HOST_NAME = hostName,
                         @LOGIN_DATE = DateTime.Now,
                         @PROJECT_NAME = Project_Name
                       },
                       unitOfWork2.Transaction
            );


            unitOfWork2.Complete();
          }

          string token = _authService.GenerateToken(user);

          unitOfWork.Complete();

          Response.Cookies.Append(_configuration["Jwt:Name"], token, new CookieOptions()
          {
            Expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:Exp"])),
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
          });

          AlertSuccess = "Login Success.";

          return Redirect("/");

        }
      }
      catch (Exception ex)
      {
        AlertError = ex.Message;
        return Redirect("/Login");
      }
    }

    public async Task LoginDomain(int u, string nonceKey)
    {
      try
      {
        using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
        {
          var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);


          var users = await userRepo.GetAllAsync();

          var user = users
              .Where(x =>
                  x.Id == u &&
                  x.IsActive == 1)
              .FirstOrDefault();

          if (user == null)
          {
            throw new Exception("Username or Password incorrect.");
          }

          var connString = "EA_APP";
          var projectid = 37;
          var hostName = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
          var Project_Name = "";
          using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection(connString)))
          {

            var testEA = await unitOfWork2.Transaction.Connection.QueryFirstOrDefaultAsync<EAAPP_ProjectMaster_TABLE>($@"
                                    SELECT * 
                                    FROM TB_PROJECT_MASTER
                                    WHERE PROJECT_ID =  @projectid
                                ", new { @projectid = projectid }, unitOfWork2.Transaction);

            Project_Name = testEA.PROJECT_NAME;

            var EmpCode = 0;
            if (user.EmployeeId == null)
            {
              EmpCode = 0;
            }
            else
            {
              EmpCode = Convert.ToInt32(user.EmployeeId);
            }

            var insertLog = unitOfWork2.Transaction.Connection.Execute(@"
                                        INSERT INTO TB_LOG_APP 
                                        (   EMP_CODE,
                                            USER_NAME,
                                            HOST_NAME,
                                            LOGIN_DATE,
                                            PROJECT_NAME
                                        ) 
                                        VALUES
                                        (
                                            @EMP_CODE,
                                            @USER_NAME,
                                            @HOST_NAME,
                                            @LOGIN_DATE,
                                            @PROJECT_NAME
                                        )
                                    ",
                       new
                       {
                         @EMP_CODE = EmpCode,
                         @USER_NAME = user.Username,
                         @HOST_NAME = hostName,
                         @LOGIN_DATE = DateTime.Now,
                         @PROJECT_NAME = Project_Name
                       },
                       unitOfWork2.Transaction
            );


            unitOfWork2.Complete();
          }

          string token = _authService.GenerateToken(user);

          unitOfWork.Complete();

          Response.Cookies.Append(_configuration["Jwt:Name"], token, new CookieOptions()
          {
            Expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:Exp"])),
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
          });


          AlertSuccess = "Login Success.";

        }
      }
      catch (Exception ex)
      {
        AlertError = ex.Message;

      }

    }
  }
}