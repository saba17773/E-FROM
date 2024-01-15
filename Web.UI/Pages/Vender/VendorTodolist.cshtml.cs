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
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Vender
{
    public class VendorTodolistModel : PageModel
    {
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;

        public VendorTodolistModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IEmailService emailService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }

        [BindProperty]
        public int CreateBy { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(VenderPermissionModel.VIEW_VENDORTODOLIST));


                return Page();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> OnPostGridAsync(string email)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        ID = "ID",
                        REQUESTID = "REQUESTID",
                        APPROVEMASTERID = "APPROVEMASTERID",
                        EMAIL = "EMAIL",
                        SENDEMAILDATE = "SENDEMAILDATE",
                        REQUESTCODE = "REQUESTCODE",
                        REQUESTDATE = "REQUESTDATE",
                        VENDCODE = "VENDCODE",
                        VENDCODE_AX = "VENDCODE_AX",
                        VENDIDNUM = "VENDIDNUM",
                        VENDNAME = "VENDNAME",
                        CREATEBY = "CREATEBY",
                        EMPLOYEEID = "EMPLOYEEID",
                        CREATENAME = "CREATENAME"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<VenderRequestListGridModel>($@"
                          SELECT *
                            FROM
                            (
	                              SELECT VT.ID,VT.REQUESTID,VT.APPROVEMASTERID
	                              ,VT.EMAIL,VT.APPROVELEVEL
                                  ,CONVERT(VARCHAR,VT.SENDEMAILDATE,103)+' '+CONVERT(VARCHAR,VT.SENDEMAILDATE,108) SENDEMAILDATE
	                              ,VT.APPROVEDATE,VT.REJECTDATE,VT.ISDONE
	                              ,VT.REMARK,VT.ISCURRENTAPPROVE,V.REQUESTCODE
	                              ,CASE WHEN VT.PROCESS IS NOT NULL THEN VT.PROCESS
									WHEN VT.PROCESS IS NULL THEN (SELECT TOP 1 VG.DESCRIPTION 
									FROM TB_VenderApproveMapping VM JOIN
									TB_VenderApproveGroup VG ON VM.APPROVEGROUPID = VG.ID
									WHERE VM.APPROVEMASTERID = VT.APPROVEMASTERID)
									END AS PROCESS
                                  ,CONVERT(VARCHAR,V.REQUESTDATE,103)+' '+CONVERT(VARCHAR,V.REQUESTDATE,108) REQUESTDATE
                                  ,V.VENDCODE,V.VENDCODE_AX AS VENDCODEAX,V.VENDIDNUM,V.VENDNAME
	                              ,V.CREATEBY,U.EmployeeId EMPLOYEEID
	                              ,(E.Name + ' ' + E.LastName) CREATENAME
	                              ,N.NONCEKEY,N.EXPIREDATE,N.ISUSED
	                            FROM TB_VenderApproveTrans VT JOIN
	                            TB_VenderTable V ON VT.REQUESTID = V.ID JOIN
	                            TB_User U ON V.CREATEBY = U.Id JOIN
	                            TB_Employee E ON U.EmployeeId = E.EmployeeId JOIN
	                            TB_VenderNonce N ON VT.SENDEMAILDATE = N.CreateDate
	                            WHERE VT.SENDEMAILDATE IS NOT NULL AND VT.APPROVEDATE IS NULL 
	                            AND VT.REJECTDATE IS NULL AND VT.ISDONE = 0
	                            AND N.IsUsed = 0
                                --AND (N.IsUsed = 0 AND N.ExpireDate > GETDATE())
	                            AND VT.EMAIL = @Email
                            )T  ORDER BY REQUESTID
                    ",
                    new
                    {
                        @Email = email
                    }, unitOfWork.Transaction);
                    unitOfWork.Complete();
                    //WHERE " + filter + @"
                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
