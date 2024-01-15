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
    public class AddRawMaterialSampleTodolistModel : PageModel
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
        public AddRawMaterialSampleTodolistModel(
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


                    var data = await unitOfWork.Transaction.Connection.QueryAsync<AddRawMaterialSampleTodolistGridModel>($@"
                        SELECT *
                        FROM
                        (
	                        SELECT AT.ADDRMSAMPLEID,
		                        R.REQUESTCODE,
		                        CASE WHEN S.UPDATEDATE IS NULL THEN CONVERT(VARCHAR,S.CREATEDATE,103)+' '+CONVERT(VARCHAR,S.CREATEDATE,108)
			                         ELSE CONVERT(VARCHAR,S.UPDATEDATE,103)+' '+CONVERT(VARCHAR,S.UPDATEDATE,108)
		                        END AS REQUESTDATE,
		                        LEFT(R.DEALER, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
		                        U.Email AS REQUESTBY,
		                        N.NONCEKEY,AT.ID APPROVETRANSID,
		                        AT.ISKEYINWHENAPPROVE
	                        FROM TB_S2EAddRawMaterialSampleApproveTrans AT JOIN
	                        TB_S2EAddRawMaterialSample S ON AT.ADDRMSAMPLEID = S.ID JOIN 
	                        TB_S2ENewRequest R ON S.REQUESTID = R.ID JOIN
	                        TB_S2EAddRawMaterialSampleNonce N ON AT.SENDEMAILDATE = N.CREATEDATE JOIN
	                        TB_User U ON S.CREATEBY = U.Id
	                        WHERE AT.SENDEMAILDATE IS NOT NULL AND
	                        AT.APPROVEDATE IS NULL AND 
	                        AT.REJECTDATE IS NULL AND 
	                        AT.ISDONE = 0 AND
	                        N.ISUSED = 0 AND 
	                        AT.EMAIL = @Email
                        )T
                        ORDER BY REQUESTDATE
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
