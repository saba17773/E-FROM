using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels.User;
using Web.UI.Interfaces;

namespace Web.UI.Pages.User
{
    public class EditModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public FormEditUser FormEditUser { get; set; } = new FormEditUser();

        [BindProperty]
        // public string UserDomain { get; set; }

        public List<SelectListItem> RoleMaster { get; set; }
        public List<SelectListItem> CompanyMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IHelperService _helperService;
        private IHttpContextAccessor _httpContextAccessor;

        public EditModel(
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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                await _authService.CanAccess(nameof(UserPermissionModel.EDIT_USER));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var t = unitOfWork.Transaction;

                    var user = await t.Connection.GetAsync<UserTable>(id, t);
                    if (user == null)
                    {
                        throw new Exception("User not found.");
                    }
                    FormEditUser.Id = user.Id;
                    FormEditUser.Username = user.Username;
                    FormEditUser.Email = user.Email;
                    FormEditUser.RoleId = user.RoleId;
                    FormEditUser.IsActive = user.IsActive;
                    FormEditUser.EmployeeId = user.EmployeeId;
                    FormEditUser.UserDomain = user.UserDomain;
                    FormEditUser.CompanyGroup = user.CompanyGroup;

                    var roleAll = t.Connection.GetAll<RoleTable>(t)
                        .Where(x => x.IsActive == 1)
                        .ToList();

                    RoleMaster = roleAll
                        .Where(x => x.IsActive == 1)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.Description
                        })
                        .ToList();
                    
                    var companyAll = t.Connection.GetAll<CompanyGroupTable>(t)
                        .ToList();

                    CompanyMaster = companyAll
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.GroupName
                        })
                        .ToList();

                    unitOfWork.Complete();
                }

                return Page();
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/User");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _authService.CanAccess(nameof(UserPermissionModel.EDIT_USER));

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var t = unitOfWork.Transaction;

                    var checkUserExists = t.Connection.GetAll<UserTable>(t)
                      .Where(x =>
                        x.Username == FormEditUser.Username &&
                        x.Id != FormEditUser.Id)
                      .FirstOrDefault();

                    if (checkUserExists != null)
                    {
                        throw new Exception("This username \"" + FormEditUser.Username + "\" already used.");
                    }

                    var user = await t.Connection.GetAsync<UserTable>(id, t);
                    if (user == null)
                    {
                        throw new Exception("User not found.");
                    }

                    var checkEditEmpUser = 0;
                    if (user.EmployeeId == FormEditUser.EmployeeId && user.Username == FormEditUser.Username)
                    {
                        checkEditEmpUser = 1;
                    }
                    else
                    {
                        checkEditEmpUser = 0;
                    }

                    var checkEditActive = 0;
                    if (user.IsActive == FormEditUser.IsActive)
                    {
                        checkEditActive = 1;
                    }
                    else
                    {
                        checkEditActive = 0;
                    }

                    var connString = "EA_APP";
                    var projectid = 37;
                    var hostName = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    var Project_Name = "";
                    
                    var EmpCode = 0;
                    if (user.EmployeeId == null)
                    {
                        EmpCode = 0;
                    }
                    else
                    {
                        EmpCode = Convert.ToInt32(user.EmployeeId);
                    }


                    using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection(connString)))
                    {
                        var testEA = await unitOfWork2.Transaction.Connection.QueryFirstOrDefaultAsync<EAAPP_ProjectMaster_TABLE>($@"
                                                SELECT * 
                                                FROM TB_PROJECT_MASTER
                                                WHERE PROJECT_ID =  @projectid
                                            ", new { @projectid = projectid }, unitOfWork2.Transaction);

                        Project_Name = testEA.PROJECT_NAME;

                        //แก้ไข Username , EmployeeID
                        if (checkEditEmpUser == 0)
                        {
                            if (checkEditActive == 0)
                            {
                                if (FormEditUser.IsActive == 0)
                                {
                                    //Disable

                                    var updateLog = unitOfWork2.Transaction.Connection.Execute(@"
                                        UPDATE TB_USER_APP SET 
                                            UPDATE_DATE = @UPDATE_DATE, 
                                            STATUS = @STATUS
                                            WHERE EMP_CODE = @EMP_CODE
                                            AND USER_NAME = @USER_NAME
                                            AND PROJECT_NAME = @PROJECT_NAME
                                            AND STATUS = 1
                                                    
                                    ",
                                       new
                                       {
                                           @UPDATE_DATE = DateTime.Now,
                                           @STATUS = 0,
                                           @EMP_CODE = user.EmployeeId,
                                           @USER_NAME = user.Username,
                                           @PROJECT_NAME = Project_Name
                                       },
                                       unitOfWork2.Transaction
                                    );
                                }
                                else
                                {
                                    //Enable
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
                                           @USER_NAME = user.Username,
                                           @PROJECT_NAME = Project_Name,
                                           @CREATE_DATE = DateTime.Now
                                       },
                                       unitOfWork2.Transaction
                                    );
                                }
                            }

                            var updateLog1 = unitOfWork2.Transaction.Connection.Execute(@"
                                        UPDATE TB_USER_APP SET 
                                            EMP_CODE = @EMP_CODE , 
                                            USER_NAME = @USER_NAME
                                        WHERE PROJECT_NAME = @PROJECT_NAME
                                        AND USER_NAME = @USER_NAME_OLD
                                        AND EMP_CODE = @EMP_CODE_OLD 
                                                    
                                    ",
                                       new
                                       {
                                           @EMP_CODE = FormEditUser.EmployeeId,
                                           @USER_NAME = FormEditUser.Username,
                                           @PROJECT_NAME = Project_Name,
                                           @USER_NAME_OLD = user.Username,
                                           @EMP_CODE_OLD = user.EmployeeId
                                       },
                                       unitOfWork2.Transaction
                            );

                            

                        }
                        else
                        {
                            if (checkEditActive == 0)
                            {
                                if (FormEditUser.IsActive == 0)
                                {
                                    //Disable

                                    var updateLog = unitOfWork2.Transaction.Connection.Execute(@"
                                        UPDATE TB_USER_APP SET 
                                            UPDATE_DATE = @UPDATE_DATE, 
                                            STATUS = @STATUS
                                            WHERE EMP_CODE = @EMP_CODE
                                            AND USER_NAME = @USER_NAME
                                            AND PROJECT_NAME = @PROJECT_NAME
                                            AND STATUS = 1
                                                    
                                    ",
                                       new
                                       {
                                           @UPDATE_DATE = DateTime.Now,
                                           @STATUS = 0,
                                           @EMP_CODE = user.EmployeeId,
                                           @USER_NAME = user.Username,
                                           @PROJECT_NAME = Project_Name
                                       },
                                       unitOfWork2.Transaction
                                    );
                                }
                                else
                                {
                                    //Enable
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
                                           @USER_NAME = user.Username,
                                           @PROJECT_NAME = Project_Name,
                                           @CREATE_DATE = DateTime.Now
                                       },
                                       unitOfWork2.Transaction
                                    );
                                }
                            }
                        }

                        unitOfWork2.Complete();
                    }



                    user.Username = FormEditUser.Username;
                    user.Email = FormEditUser.Email;
                    user.RoleId = FormEditUser.RoleId;
                    user.IsActive = FormEditUser.IsActive;
                    user.EmployeeId = FormEditUser.EmployeeId;
                    user.UserDomain = FormEditUser.UserDomain;
                    user.CompanyGroup = FormEditUser.CompanyGroup;

                    await t.Connection.UpdateAsync<UserTable>(user, t);

                    unitOfWork.Complete();

                    AlertSuccess = "Edit User Success.";

                    return Redirect("/User/" + id + "/Edit");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/User/" + id + "/Edit");
            }
        }
    }
}