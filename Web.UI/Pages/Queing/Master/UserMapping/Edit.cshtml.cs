using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    public class EditModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int UserID { get; set; }
        [BindProperty]
        public List<string> PlantResult { get; set; }
        [BindProperty]
        public string EmployeeID { get; set; }
        [BindProperty]
        public string UserDomain { get; set; }
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string FirstName { get; set; }
        [BindProperty]
        public string LastName { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public List<string> PlantResultLogsID { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public EditModel(
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
        public async Task<IActionResult> OnGetAsync(int userid)
        {
            try
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.ADMIN_QUEING));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var UserByID = await UserRepo.GetAsync(userid);

                    var EmployeeRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                    var EmployeeALL = await EmployeeRepo.GetAllAsync();
                    var EmployeeByUserID = EmployeeALL.Where(x => x.EmployeeId == UserByID.EmployeeId).FirstOrDefault();

                    UserID = userid;
                    EmployeeID = EmployeeByUserID.EmployeeId;
                    UserDomain = UserByID.UserDomain;
                    Username = UserByID.Username;
                    FirstName = EmployeeByUserID.Name;
                    LastName = EmployeeByUserID.LastName;
                    Email = UserByID.Email;

                    return Page();
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Page();
            }
        }
        public async Task<JsonResult> OnPostPlantGridAsync(int userid)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<CompanyGridModel>(@"
                       SELECT *
                        FROM
                        (
	                        SELECT C.ID
		                        ,C.COMPANY + '  : ' +C.FullName_EN AS FULLNAME
		                        ,C.COMPANY
		                        ,CASE WHEN M.ID IS NULL THEN 0 ELSE 1 END AS ISCHECK
	                        FROM TB_QingMaster_Company C 
	                        LEFT JOIN
	                        TB_QingMaster_UserMapping M ON C.company = M.PLANT AND M.USERID = @userid
	                        WHERE C.IsQingCompany = 1 
                        )T ",
                        new
                        {
                            @userid = userid
                        }
                        , unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<IActionResult> OnPostAsync(int userid)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var CreateDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;

                    var UserMappingRepo = new GenericRepository<QingMaster_UserMapping_TB>(unitOfWork.Transaction);

                    var rowcount = 0;
                    int totalrow = PlantResult.Count();
                    for (int i = 0; i < totalrow; i++)
                    {
                        var Plant = PlantResult[i];
                        if (Plant != null)
                        {
                            rowcount += 1;
                        }
                    }
                    if (rowcount == 0)
                    {
                        AlertError = "Please Active Plant !!";
                        return Redirect($"/Queing/Master/UserMapping/{UserID}/Edit");
                    }

                    //delete all
                    using (var unitOfWork2 = new UnitOfWork(_databaseContext.GetConnection()))
                    {
                        var deleteOLDLogsDoc = unitOfWork2.Transaction.Connection.Execute(@"DELETE 
                                FROM TB_QingMaster_UserMapping
                                WHERE USERID = @userid",
                            new
                            {
                                @userid = userid
                            },
                            unitOfWork2.Transaction
                        );
                        unitOfWork2.Complete();
                    }

                    for (int i = 0; i < totalrow; i++)
                    {
                        var Plant = PlantResult[i];
                        if (Plant != null)
                        {
                            await UserMappingRepo.InsertAsync(new QingMaster_UserMapping_TB
                            {
                                USERID = UserID,
                                PLANT = Plant,
                                ISACTIVE = 1,
                                CREATEBY = CreateBy,
                                CREATEDATE = CreateDate
                            });
                        }
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Edit User Mapping Success";
                    return Redirect("/Queing/Master/UserMapping");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Master/UserMapping/{UserID}/Edit");
            }
        }
    }
}
