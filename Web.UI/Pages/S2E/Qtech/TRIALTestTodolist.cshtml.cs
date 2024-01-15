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

namespace Web.UI.Pages.S2E.Qtech
{
    public class TRIALTestTodolistModel : PageModel
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
        public TRIALTestTodolistModel(
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


                    var data = await unitOfWork.Transaction.Connection.QueryAsync<TrialTestTodolistGridModel>($@"
                         SELECT *
                            FROM
                            (
	                            SELECT AT.TRIALID,AT.TRIALLINEID,
                                    CONVERT(VARCHAR,TH.REQUESTDATE,103)+' '+CONVERT(VARCHAR,TH.REQUESTDATE,108) REQUESTDATE,
                                    TL.CHEMICALNAME ,
                                    U.Email AS REQUESTBY,
                                    N.NONCEKEY,
                                    AT.ID AS APPROVETRANSID,
                                    TL.PROJECTREFNO,
                                    R.REQUESTCODE,
                                    LEFT(AH.SUPPLIERNAME, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME
                                FROM TB_S2ETrialTestApproveTrans AT JOIN
                                TB_S2ETrialTestNonce N ON AT.SENDEMAILDATE = N.CREATEDATE JOIN
                                TB_S2ETrialTestHead TH ON AT.TRIALID = TH.ID JOIN
                                TB_S2ETrialTestLine TL ON AT.TRIALLINEID = TL.ID JOIN
                                TB_S2EMaterialRequestHead RH ON TH.RMREQID = RH.ID JOIN
                                TB_S2EAddRawMaterialHead AH ON RH.ADDRMID = AH.ID JOIN
                                TB_S2ENewRequest R ON AH.REQUESTID = R.ID JOIN
                                TB_User U ON TL.CREATEBY = U.Id
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
