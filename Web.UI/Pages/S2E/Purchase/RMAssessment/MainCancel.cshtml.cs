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

namespace Web.UI.Pages.S2E.Purchase.RMAssessment
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
        public MainCancelModel(
         IDatabaseContext databaseContext,
         IDatatableService datatablesService,
         IAuthService authService)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _authService = authService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_RMASSESSMENT));

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
                        SUPPLIERNAMEOLD = "SUPPLIERNAMEOLD",
                        SUPPLIERNAMENEW = "SUPPLIERNAMENEW",
                        PLANT = "PLANT",
                        REQUESTBY = "REQUESTBY"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ERMAssessmentGridModel>(@"
                         SELECT T.* 
                        FROM (
                            SELECT R.ID,A.ID AS ASSESSMENTID,
								R.REQUESTCODE,L.PROJECTREFNO AS REQUESTNO,
                                A.PLANT,
                                LEFT(R.DEALER, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
                                U.Email REQUESTBY,
                                A.APPROVESTATUS,
                                R.CREATEDATE,
                                A.APPROVEGROUPID,
                                CASE WHEN A.APPROVESTATUS = 8 THEN 1
                                WHEN A.APPROVESTATUS = 4 THEN 2 
                                WHEN A.APPROVESTATUS = 3 THEN 3
                                WHEN A.APPROVESTATUS = 5 THEN 4
                                ELSE 5 END AS ORDERLIST,
                                A.CANCELREMARK
                            FROM TB_S2ENewRequest R JOIN
                            TB_S2ERMAssessment A ON R.ID = A.REQUESTID JOIN
                            TB_User U ON A.CREATEBY = U.Id LEFT JOIN
                            TB_S2ELABTestHead H ON R.ID = H.REQUESTID LEFT JOIN
                            TB_S2ELABTestLine L ON H.ID = L.LABID AND L.ISCURRENTLOGS = 1
                            WHERE A.APPROVESTATUS = 2
                        )T
                        WHERE " + filter + @" ORDER BY T.CREATEDATE ASC
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
