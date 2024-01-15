using System;
using System.Collections.Generic;
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
    public class LogoutModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        private IConfiguration _configuration;
        private IDatabaseContext _databaseContext;
        private IAuthService _authService;
        private IHttpContextAccessor _httpContextAccessor;

        public LogoutModel(
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

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var userid = _authService.GetClaim().UserId;
                    var user = await userRepo.GetAsync(userid);

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
                                            LOGOUT_DATE,
                                            PROJECT_NAME
                                        ) 
                                        VALUES
                                        (
                                            @EMP_CODE,
                                            @USER_NAME,
                                            @HOST_NAME,
                                            @LOGOUT_DATE,
                                            @PROJECT_NAME
                                        )
                                    ",
                                   new
                                   {
                                       @EMP_CODE = EmpCode,
                                       @USER_NAME = user.Username,
                                       @HOST_NAME = hostName,
                                       LOGOUT_DATE = DateTime.Now,
                                       @PROJECT_NAME = Project_Name
                                   },
                                   unitOfWork2.Transaction
                        );


                        unitOfWork2.Complete();
                    }


                    unitOfWork.Complete();

                    Response.Cookies.Delete(_configuration["Jwt:Name"]);

                    AlertSuccess = "Logout Success.";

                    return Redirect("/Login?u=0");

                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/");
            }
        }

    }
}