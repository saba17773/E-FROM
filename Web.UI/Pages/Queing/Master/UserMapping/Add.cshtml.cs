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
    public class AddModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int UserID { get; set; }
        [BindProperty]
        public List<string> PlantResult { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public AddModel(
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
        public async Task<JsonResult> OnPostEmployeeGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        USERNAME = "USERNAME",
                        EMPLOYEEID = "EMPLOYEEID",
                        EMAIL = "EMAIL",
                        NAME = "NAME",
                        FULLNAME = "FULLNAME"
                    };

                    var filter = _datatableService.Filter(Request, field);

                    var ItemSample = await unitOfWork.Transaction.Connection.QueryAsync<UserMappingGridModel>(@"
                        SELECT *
                        FROM
                        (
	                        SELECT U.Id		USERID
		                        ,U.EmployeeId	EMPLOYEEID
		                        ,U.Username		USERNAME
		                        ,U.Email		EMAIL
		                        ,U.UserDomain	USERDOMAIN
		                        ,U.IsActive		ISACTIVE
		                        ,E.Name			NAME
		                        ,E.LastName		LASTNAME
                                ,E.Name + ' ' + E.LastName AS FULLNAME
	                        FROM TB_User U JOIN
	                        TB_Employee E ON U.EmployeeId = E.EmployeeId  LEFT JOIN
	                        (
		                        SELECT USERID FROM TB_QingMaster_UserMapping GROUP BY USERID
	                        ) M ON U.Id = M.USERID
	                        WHERE U.IsActive = 1 AND M.USERID IS NULL
                        )T
                        WHERE " + filter + @" 
                        ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, ItemSample.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<JsonResult> OnPostPlantGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<CompanyGridModel>(@"
                       SELECT ID
                        ,COMPANY + '  : ' +FullName_EN AS FULLNAME
                        ,COMPANY
                       FROM TB_QingMaster_Company
                       WHERE IsQingCompany = 1 ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

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
                    if (UserID == 0)
                    {
                        AlertError = "Please input Employee !!";
                        return Redirect("/Queing/Master/UserMapping/Add");
                    }

                    var CreateDate = DateTime.Now;
                    var CreateBy = _authService.GetClaim().UserId;

                    var UserMappingRepo = new GenericRepository<QingMaster_UserMapping_TB>(unitOfWork.Transaction);

                    var CompanyRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                    var CompanyALL = await CompanyRepo.GetAllAsync();
                    var CompanyIsQing = CompanyALL.Where(x => x.IsQingCompany == 1);

                    var rowcount = 0;
                    int totalrow = PlantResult.Count();
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

                            rowcount += 1;
                        }

                    }

                    if (rowcount == 0)
                    {
                        AlertError = "Please Active Plant !!";
                        return Redirect("/Queing/Master/UserMapping/Add");
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Add User Mapping Success";
                    return Redirect("/Queing/Master/UserMapping");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Master/UserMapping/Add");
            }
        }
    }
}
