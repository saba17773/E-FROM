using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.Queing;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Master.UserMapping
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int UserID { get; set; }
        [BindProperty]
        public int isActive { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        public List<SelectListItem> PlantMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public IndexModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.ADMIN_QUEING));

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Page();
            }
        }
        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        employeeid = "EMPLOYEEID",
                        fullname = "FULLNAME",
                        email = "EMAIL",
                        username = "USERNAME"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<UserMappingGridModel>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT M.USERID
	                        ,U.EmployeeId AS EMPLOYEEID
	                        ,E.Name + ' ' + E.LastName AS FULLNAME
	                        ,U.Email AS EMAIL
	                        ,U.Username AS USERNAME
                            ,M.ISACTIVE
                            ,E.Name  NAME
                            ,E.LastName LASTNAME
                            ,U.UserDomain
	                        FROM TB_QingMaster_UserMapping M
	                        JOIN TB_User U ON M.USERID = U.Id
	                        JOIN TB_Employee E ON U.EmployeeId = E.EmployeeId
	                        GROUP BY M.USERID,U.EmployeeId,E.Name,
	                        E.LastName,U.Email,M.ISACTIVE,U.Username,E.Name,
	                        E.LastName,U.UserDomain
                        )T
                        WHERE " + filter + @"
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnGetActiveUserMappingAsync(int userid, int isactive)
        {
            try
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.ADMIN_QUEING));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var UpdateDate = DateTime.Now;
                    var UpdateBy = _authService.GetClaim().UserId;

                    var UserMappingRepo = new GenericRepository<QingMaster_UserMapping_TB>(unitOfWork.Transaction);
                    var UserMappingALL = await UserMappingRepo.GetAllAsync();
                    var UserMappingByUserID = UserMappingALL.Where(x => x.USERID == userid);

                    foreach (var update in UserMappingByUserID)
                    {
                        update.ISACTIVE = isactive;
                        update.UPDATEBY = UpdateBy;
                        update.UPDATEDATE = UpdateDate;
                        await UserMappingRepo.UpdateAsync(update);
                    }


                    if (isactive == 1)
                    {
                        AlertSuccess = "Enable User Mapping Success";
                    }
                    else
                    {
                        AlertSuccess = "Disable User Mapping Success";
                    }

                    unitOfWork.Complete();
                    return Redirect("/Queing/Master/UserMapping");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }
        
    }
}
