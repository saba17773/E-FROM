using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.ApproveManage.VenderApproveMapping
{
    public class IndexModel : PageModel
    {
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        public IndexModel(
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
            await _authService.CanAccess(nameof(VenderPermissionModel.APPROVE_MAPPING_VENDER));
        }

        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        id = "ID",
                        createby = "CreateBy",
                        description = "DESCRIPTION",
                        approvemasterid = "APPROVEMASTERID",
                        approvegroupid = "APPROVEGROUPID",
                        step = "STEP",
                        employeeid = "EmployeeId",
                        approvemaster_desc = "GroupDescription",
                        email = "Email",
                        dataareaid = "DATAAREAID",
                        username = "Username"
                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<ApproveMapGridModel>(@"
                       SELECT *
                        FROM (SELECT VM.ID,CreateBy,
		                    APPROVEMASTERID,
		                    APPROVEGROUPID,
		                    STEP
		                    ,U.EmployeeId
		                    ,U.Email
		                    ,G.DESCRIPTION
		                    ,M.GroupDescription
		                    ,CASE VM.DATAAREAID WHEN 'dv' THEN 'โรงงาน' ELSE 'DSC' END AS DATAAREAID
                            ,U.Username
                    FROM TB_VenderApproveMapping VM 
                    JOIN TB_User U ON VM.CreateBy = U.Id
                    JOIN TB_VenderApproveGroup G ON VM.APPROVEGROUPID = G.ID
                    JOIN TB_ApproveMaster M ON VM.APPROVEMASTERID = M.Id)T
                        
                        WHERE " + filter + @"
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatablesService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
