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
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.Purchase.PurchaseSample
{
    public class MainCancelModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IEmailService _emailService;
        public MainCancelModel(
         IDatabaseContext databaseContext,
         IDatatableService datatablesService,
         IEmailService emailService,
         IAuthService authService)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _emailService = emailService;
            _authService = authService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_PURCHASESAMPLE));

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Purchase");
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
                        REQUESTCODE = "REQUESTCODE",
                        REQUESTNO = "REQUESTNO",
                        SUPPLIERNAME = "SUPPLIERNAME",
                        VENDORID = "VENDORID",
                        REQUESTBY = "REQUESTBY"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EPurchaseSampleGridModel>(@"
                           SELECT T.* 
                            FROM (
                                SELECT LH.ID LABID
	                                ,LH.REQUESTID
	                                ,LH.ASSESSMENTID
	                                ,LL.PROJECTREFNO REQUESTNO
                                    ,LL.ID LABLINEID
	                                ,R.REQUESTCODE
	                                ,CASE WHEN PS.APPROVESTATUS IS NULL THEN R.VENDORID
	                                ELSE PS.VENDORID END AS VENDORID
	                                ,CASE WHEN PS.APPROVESTATUS IS NULL THEN LEFT(R.DEALER, 30) + '|' + LEFT(R.SUPPLIERNAME, 30)
	                                ELSE LEFT(PS.SUPPLIERNAME, 30)  + '<br/>' + LEFT(R.SUPPLIERNAME, 30)  END AS SUPPLIERNAME
	                                ,U.Email AS REQUESTBY
	                                ,LL.APPROVESTATUS LABSTATUS
	                                ,PS.ID PCSAMPLEID
	                                ,PS.APPROVESTATUS PCSAMPLESTATUS
                                    ,LL.ISPURCHASESAMPLE
	                                ,AH.ID ADDRMID
                                FROM TB_S2ELABTestHead LH JOIN
                                TB_S2ELABTestLine LL ON LH.ID = LL.LABID AND LL.ISCURRENTLOGS = 1 JOIN
                                TB_S2ENewRequest R ON LH.REQUESTID = R.ID JOIN
                                TB_User U ON LL.CREATEBY = U.Id LEFT JOIN
                                TB_S2EPurchaseSample PS ON LH.ID = PS.LABID LEFT JOIN
                                TB_S2EAddRawMaterialHead AH ON PS.ID = AH.PCSAMPLEID
                                WHERE LL.APPROVESTATUS = 2 OR 
                                (LL.APPROVESTATUS = 2 AND PS.APPROVESTATUS = 2) 
                            )T
                        WHERE " + filter + @" ORDER BY T.REQUESTID
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
