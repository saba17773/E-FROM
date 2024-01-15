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
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.User
{
    public class AddModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        [Required]
        [StringLength(20)]
        [MinLength(6)]
        public string Username { get; set; }

        [BindProperty]
        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [BindProperty]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [BindProperty]
        public int Role { get; set; }
        
        [Required]
        [BindProperty]
        public int CompanyGroup { get; set; }

        [Required]
        [BindProperty]
        public int IsActive { get; set; }

        [BindProperty]
        public string EmployeeId { get; set; }

        [BindProperty]
        public string UserDomain { get; set; }

        public List<SelectListItem> RoleMaster { get; set; }
        public List<SelectListItem> CompanyMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IHelperService _helperService;
        private IHttpContextAccessor _httpContextAccessor;

        public AddModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IHelperService helperService,
          IHttpContextAccessor httpContextAccessor)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatableService;
            _authService = authService;
            _helperService = helperService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(UserPermissionModel.ADD_USER));

                RoleMaster = await GetRoleMasterAsync();
                CompanyMaster = await GetCompanyMasterAsync();
                return Page();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(UserPermissionModel.ADD_USER));

                if (!ModelState.IsValid)
                {
                    RoleMaster = await GetRoleMasterAsync();
                    CompanyMaster = await GetCompanyMasterAsync();
                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);

                    var userAll = await userRepo.GetAllAsync();
                    var user = userAll.Where(x => x.Username == Username).FirstOrDefault();
                    if (user != null)
                    {
                        throw new Exception("Username already used.");
                    }

                    var connString = "EA_APP";
                    var projectid = 37;
                    var hostName = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    var Project_Name = "";

                    using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection(connString)))
                    {
                        var EmpCode = 0;
                        if (EmployeeId == null)
                        {
                            EmpCode = 0;
                        }
                        else
                        {
                            EmpCode = Convert.ToInt32(EmployeeId);
                        }
                        var testEA = await unitOfWork2.Transaction.Connection.QueryFirstOrDefaultAsync<EAAPP_ProjectMaster_TABLE>($@"
                                                SELECT * 
                                                FROM TB_PROJECT_MASTER
                                                WHERE PROJECT_ID =  @projectid
                                            ", new { @projectid = projectid }, unitOfWork2.Transaction);

                        Project_Name = testEA.PROJECT_NAME;
                        var insertLog = unitOfWork2.Transaction.Connection.Execute(@"
                                                    INSERT INTO TB_USER_APP 
                                                    (   EMP_CODE,
                                                        HOST_NAME,
                                                        USER_NAME,
                                                        PROJECT_NAME,
                                                        CREATE_DATE
                                                    ) 
                                                    VALUES
                                                    (
                                                        @EMP_CODE,
                                                        @HOST_NAME,
                                                        @USER_NAME,
                                                        @PROJECT_NAME,
                                                        @CREATE_DATE
                                                    )
                                                ",
                                   new
                                   {
                                       @EMP_CODE = EmpCode,
                                       @HOST_NAME = hostName,
                                       @USER_NAME = Username,
                                       @PROJECT_NAME = Project_Name,
                                       @CREATE_DATE = DateTime.Now
                                   },
                                   unitOfWork2.Transaction
                        );


                        unitOfWork2.Complete();
                    }

                    var salt = Guid.NewGuid();

                    await userRepo.InsertAsync(new UserTable
                    {
                        Username = Username,
                        Password = _authService.HashPassword(Password, salt.ToString()),
                        Salt = salt.ToString(),
                        Email = Email,
                        RoleId = Role,
                        IsActive = IsActive,
                        UserDomain = UserDomain,
                        EmployeeId = EmployeeId,
                        CompanyGroup = CompanyGroup
                    });

                    unitOfWork.Complete();

                    AlertSuccess = "Add User Success.";

                    return Redirect("/User/Add");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/User/Add");
            }
        }

        public async Task<List<SelectListItem>> GetRoleMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var roleRepo = new GenericRepository<RoleTable>(unitOfWork.Transaction);

                var roleAll = await roleRepo.GetAllAsync();

                return roleAll
                    .Where(x => x.IsActive == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Description
                    })
                    .ToList();
            }
        }

        public async Task<List<SelectListItem>> GetCompanyMasterAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var groupRepo = new GenericRepository<CompanyGroupTable>(unitOfWork.Transaction);

                var groupAll = await groupRepo.GetAllAsync();

                return groupAll
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.GroupName
                    })
                    .ToList();
            }
        }
    }
}