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
    public class MaterialRequestSampleTodolistModel : PageModel
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
        public MaterialRequestSampleTodolistModel(
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


                    var data = await unitOfWork.Transaction.Connection.QueryAsync<MaterialRequestSampleTodolistGridModel>($@"
                        SELECT *
                            FROM
                            (
                                SELECT AT.ID APPROVETRANSID,
                                    CONVERT(VARCHAR,RL.REQUESTDATE,103)+' '+CONVERT(VARCHAR,RL.REQUESTDATE,108) REQUESTDATE,
                                    RL.NO ,
                                    R.ITEMCODE,
                                    R.ITEMNAME,
                                    R.UNIT,
                                    RL.QTY,
                                    RL.DEPARTMENT,
                                    RL.SUPGROUP,
                                    N.NONCEKEY,
                                    R.REQUESTCODE,
                                    AT.RMREQSAMID,
                                    AT.RMREQSAMLINEID,
                                    U.Username AS REQUESTBY,
                                    R.VENDORID,
                                    LEFT(R.DEALER, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME
                                FROM TB_S2EMaterialRequestSampleApproveTrans AT JOIN
                                TB_S2EMaterialRequestSampleNonce N ON AT.SENDEMAILDATE = N.CREATEDATE JOIN
                                TB_S2EMaterialRequestSampleHead RH ON AT.RMREQSAMID = RH.ID JOIN
                                TB_S2EMaterialRequestSampleLine RL ON AT.RMREQSAMLINEID = RL.ID JOIN
                                TB_S2EAddRawMaterialSample S ON RH.ADDRMSAMPLEID = S.ID JOIN
                                TB_S2ENewRequest R ON S.REQUESTID = R.ID JOIN
                                TB_User U ON RL.CREATEBY = U.Id
                                WHERE AT.SENDEMAILDATE IS NOT NULL AND
                                AT.APPROVEDATE IS NULL AND 
                                AT.REJECTDATE IS NULL AND 
                                AT.ISDONE = 0 AND 
                                N.ISUSED = 0  AND 
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
