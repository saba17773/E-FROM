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

namespace Web.UI.Pages.S2E.Qtech.LABTest
{
    public class MainSuccessModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        public MainSuccessModel(
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
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_LABTEST));

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E/Qtech");
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
                        REQUESTDATE = "REQUESTDATE",
                        SUPPLIERNAME = "SUPPLIERNAME",
                        PLANT = "PLANT",
                        REQUESTBY = "REQUESTBY"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ELabTestGridModel>(@"
                       SELECT T.* 
                        FROM (
                            SELECT LH.REQUESTID,
                                LH.ASSESSMENTID,
                                LH.ID AS LABTESTID,
                                LL.ID AS LABTESTLINEID,
                                R.REQUESTCODE,
                                LL.PROJECTREFNO,
                                LEFT(R.DEALER, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
                                U.Email AS REQUESTBY,
                                A.APPROVESTATUS AS ASSESSAPPROVESTATUS,
                                LL.APPROVESTATUS AS LABAPPROVESTATUS,
                                LL.CREATEDATE,
                                LL.APPROVEGROUPID,
                                CASE WHEN A.APPROVESTATUS = 5 AND LL.APPROVESTATUS IS NULL THEN 1 
	                                WHEN A.APPROVESTATUS = 7 AND LL.APPROVESTATUS = 8 THEN 2
	                                WHEN A.APPROVESTATUS = 7 AND LL.APPROVESTATUS = 4 THEN 3
	                                WHEN A.APPROVESTATUS = 7 AND LL.APPROVESTATUS = 3 THEN 4
                                ELSE 5 END AS ORDERLIST,
                                CASE WHEN LL.TESTRESULT = 1 THEN '1'
	                                 WHEN LL.TESTRESULT = 0 THEN '0'
                                ELSE '' END AS TESTRESULT
                            FROM TB_S2ELABTestHead LH JOIN
                            TB_S2ELABTestLine LL ON LL.LABID = LH.ID JOIN
                            TB_User U ON LL.CREATEBY = U.Id JOIN
                            TB_S2ENewRequest R ON R.ID = LH.REQUESTID JOIN
                            TB_S2ERMAssessment A ON A.ID = LH.ASSESSMENTID
                            WHERE LL.APPROVESTATUS IN (5,7)
                        )T
                        WHERE " + filter + @" ORDER BY T.LABAPPROVESTATUS,T.LABTESTID
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
