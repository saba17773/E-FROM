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

namespace Web.UI.Pages.S2E.Purchase
{
    public class NewRequestTodolistModel : PageModel
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
        public NewRequestTodolistModel(
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


                    var data = await unitOfWork.Transaction.Connection.QueryAsync<NewRequestTodolistGridModel>($@"
                        SELECT *
                        FROM
                        (
	                        SELECT AT.REQUESTID,R.REQUESTCODE,
                              CONVERT(VARCHAR,R.REQUESTDATE,103)+' '+CONVERT(VARCHAR,R.REQUESTDATE,108) REQUESTDATE,
                              LEFT(R.DEALER, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
                              U.Email AS REQUESTBY,
                              N.NONCEKEY,AT.ID APPROVETRANSID,
                              AT.ISKEYINWHENAPPROVE
                              FROM TB_S2ENewRequestApproveTrans AT JOIN
                              TB_S2ENewRequest R ON AT.REQUESTID = R.ID JOIN
                              TB_S2ENewRequestNonce N ON AT.SENDEMAILDATE = N.CREATEDATE JOIN
                              TB_User U ON R.CREATEBY = U.Id
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
