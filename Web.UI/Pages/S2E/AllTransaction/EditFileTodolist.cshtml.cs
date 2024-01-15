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
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Interfaces;


namespace Web.UI.Pages.S2E.AllTransaction
{
    public class EditFileTodolistModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public string EMAIL { get; set; }

        [BindProperty]
        public string ApproveBy { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        public EditFileTodolistModel(
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
        public IActionResult OnGet(string Email)
        {
            try
            {
                EMAIL = Email;
                return Page();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> OnPostGridAsync(string mail)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {


                    var data = await unitOfWork.Transaction.Connection.QueryAsync<LogEditFileTodolistGridModel>($@"
                        SELECT *
                        FROM
                        (
	                        SELECT  H.ID AS REQUESTID,R.REQUESTCODE,
                                CONVERT(VARCHAR,H.CREATEDATE,103)+' '+CONVERT(VARCHAR,H.CREATEDATE,108) REQUESTDATE,
                                LEFT(R.DEALER, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
                                REPLACE(U.Email,'@deestone.com','') AS REQUESTBY,
                                N.NONCEKEY,AT.ID APPROVETRANSID,
                                AG.GROUPDESCRIPTION
                            FROM TB_S2ELogFileEditApproveTrans AT JOIN
                            TB_S2ELogFileEditHead H ON AT.LOGFILEHEADID = H.ID JOIN
                            TB_S2ELogFileEditNonce N ON AT.SENDEMAILDATE = N.CREATEDATE JOIN
                            TB_S2ENewRequest R ON H.REQUESTID = R.ID JOIN
                            TB_User U ON H.CREATEBY = U.Id JOIN
                            TB_S2EApproveGroup AG ON H.APPROVEGROUPID = AG.ID
                            WHERE AT.SENDEMAILDATE IS NOT NULL AND
                            AT.APPROVEDATE IS NULL AND 
                            AT.REJECTDATE IS NULL AND 
                            AT.ISDONE = 0 AND
                            N.ISUSED = 0 AND 
                            AT.EMAIL = @Email
                        )T  ORDER BY REQUESTID
                    ",
                    new
                    {
                        @Email = mail
                    }, unitOfWork.Transaction);

                    unitOfWork.Complete();

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
