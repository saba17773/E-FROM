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
    public class MainModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        public MainModel(
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
                        SELECT 0 AS RMREQSAMID,
                            0 AS REQUESTSTATUS,
                            T.REQUESTCODE,
                            T.SUPPLIERNAME,
                            T.LABTESTID,
                            T.LABTESTLINEID,
                            T.PROJECTREFNO,
                            T.LABAPPROVESTATUS,
                            T.REQUESTBY,
                            T.APPROVEGROUPID,
                            T.ORDERLIST,
                            T.TESTRESULT,
                            T.ASSESSAPPROVESTATUS,
                            T.REQUESTID,
                            T.ASSESSMENTID
                            FROM
                            (
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
	                            TB_S2ELABTestLine LL ON LH.ID = LL.LABID AND LL.ISCURRENTLOGS = 1 JOIN
	                            TB_User U ON U.Id = LL.CREATEBY JOIN
	                            TB_S2ENewRequest R ON R.ID = LH.REQUESTID JOIN
	                            TB_S2ERMAssessment A ON A.ID = LH.ASSESSMENTID 
	                            WHERE LL.APPROVESTATUS NOT IN (5,7)
                            )T
                            UNION ALL
                            SELECT *
                            FROM
                            (
                                SELECT T3.RMREQSAMID,
                                    T3.REQUESTSTATUS,
                                    T3.REQUESTCODE,
                                    T3.SUPPLIERNAME,
                                    LH.ID LABTESTID,
                                    LL.ID LABTESTLINEID,
                                    LL.PROJECTREFNO,
                                    LL.APPROVESTATUS AS LABAPPROVESTATUS,
                                    U.Email AS REQUESTBY,
                                    LL.APPROVEGROUPID,
                                    CASE WHEN LL.APPROVESTATUS IS NULL THEN 1 
                                        WHEN LL.APPROVESTATUS = 8 THEN 2
                                        WHEN LL.APPROVESTATUS = 4 THEN 3
                                        WHEN LL.APPROVESTATUS = 3 THEN 4
                                    ELSE 5 END AS ORDERLIST,
		                            CASE WHEN LL.TESTRESULT = 1 THEN '1'
			                             WHEN LL.TESTRESULT = 0 THEN '0'
		                            ELSE '' END AS TESTRESULT,
		                            T3.ASSESSAPPROVESTATUS,
                                    T3.REQUESTID,
                                    T3.ASSESSMENTID
                                FROM
                                (
                                    SELECT *,
                                    CASE WHEN T2.COUNTROW = CHKAPPSTATUS THEN 1 ELSE 0 END AS ISCANOPEN 
                                    FROM 
                                    (
                                        SELECT T.RMREQSAMID,
                                        COUNT(T.RMREQSAMLINEID) AS COUNTROW,
                                        SUM(T.CHKAPPROVESTATUS) AS CHKAPPSTATUS,
                                        T.REQUESTSTATUS,
                                        T.REQUESTCODE,
                                        T.VENDORID,
                                        T.SUPPLIERNAME,
                                        T.ITEMCODE,
                                        T.ITEMNAME,
                                        T.REQUESTID,
                                        T.ASSESSMENTID,
                                        T.ASSESSAPPROVESTATUS
                                        FROM
                                        (
                                            SELECT RH.ID AS RMREQSAMID ,
                                                RL.ID RMREQSAMLINEID,
                                                RH.ADDRMSAMPLEID,
                                                CASE WHEN RL.APPROVESTATUS = 7 THEN 1 ELSE 0 END AS CHKAPPROVESTATUS,
                                                RH.REQUESTSTATUS,
                                                R.REQUESTCODE,
                                                R.VENDORID,
                                                LEFT(R.DEALER, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
                                                R.ITEMCODE,
                                                R.ITEMNAME,
                                                S.REQUESTID,
                                                S.ASSESSMENTID,
                                                A.APPROVESTATUS AS ASSESSAPPROVESTATUS
                                            FROM TB_S2EMaterialRequestSampleHead RH JOIN
                                            TB_S2EMaterialRequestSampleLine RL ON RH.ID = RL.RMREQSAMID JOIN
                                            TB_S2EAddRawMaterialSample S ON RH.ADDRMSAMPLEID = S.ID JOIN
                                            TB_S2ENewRequest R ON S.REQUESTID = R.ID JOIN
                                            TB_S2ERMAssessment A ON S.ASSESSMENTID = A.ID
                                            WHERE RH.REQUESTSTATUS IN (1,5,7) 
                                            AND  RL.APPROVESTATUS <> 2
                                        )T
                                        GROUP BY T.RMREQSAMID,T.REQUESTSTATUS,T.REQUESTCODE,T.VENDORID,
                                        T.SUPPLIERNAME,T.ITEMCODE,T.ITEMNAME,T.REQUESTID,T.ASSESSMENTID,T.ASSESSAPPROVESTATUS
                                    )T2
                                )T3 LEFT JOIN 
                                TB_S2ELABTestHead LH ON LH.REQUESTID = T3.REQUESTID AND LH.ASSESSMENTID = T3.ASSESSMENTID LEFT JOIN
                                TB_S2ELABTestLine LL ON LH.ID = LL.LABID AND LL.ISCURRENTLOGS = 1 LEFT JOIN
                                TB_User U ON LL.CREATEBY = U.Id 
                                WHERE ISCANOPEN = 1 AND LH.ID IS NULL 
                            )T4

                        WHERE " + filter + @" ORDER BY ORDERLIST
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
