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

namespace Web.UI.Pages.S2E.Purchase.AddRawMaterialSample
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
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_ADDRAWMATERIALSAMPLE));

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
                        REQUESTNO = "PROJECTREFNO",
                        SUPPLIERNAME = "SUPPLIERNAME",
                        VENDORID = "VENDORID",
                        REQUESTBY = "REQUESTBY"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EAddRawMaterialSampleGridModel>(@"
                       SELECT * 
                        FROM
                        ( 
	                        SELECT N.ID AS REQUESTID,
		                        A.ID AS ASSESSMENTID,
		                        S.ID AS ADDRMSAMPLEID,
		                        N.REQUESTCODE,
		                        LL.PROJECTREFNO,
		                        N.VENDORID,
		                        LEFT(N.DEALER, 30) + '<br/>' + LEFT(N.SUPPLIERNAME, 30) AS SUPPLIERNAME,
		                        U.Email AS REQUESTBY,
		                        A.APPROVESTATUS AS ASSESSAPPROVESTATUS,
		                        S.APPROVESTATUS AS ADDRMSAMPLEAPPROVESTATUS,
		                        S.CREATEDATE,
		                        S.APPROVEGROUPID,
		                        CASE WHEN A.APPROVESTATUS = 5 AND S.APPROVESTATUS IS NULL THEN 1 
		                        WHEN A.APPROVESTATUS = 7 AND S.APPROVESTATUS = 8 THEN 2
		                        WHEN A.APPROVESTATUS = 7 AND S.APPROVESTATUS = 4 THEN 3
		                        WHEN A.APPROVESTATUS = 7 AND S.APPROVESTATUS = 3 THEN 4
		                        ELSE 5 END AS ORDERLIST
	                        FROM TB_S2ENewRequest N JOIN
	                        TB_S2ERMAssessment A ON N.ID = A.REQUESTID LEFT JOIN
	                        TB_S2EAddRawMaterialSample S ON N.ID = S.REQUESTID AND A.ID = S.ASSESSMENTID LEFT JOIN
	                        TB_User U ON S.CREATEBY = U.Id  LEFT JOIN
	                        TB_S2ELABTestHead LH ON N.ID = LH.REQUESTID AND A.ID = LH.ASSESSMENTID LEFT JOIN
	                        TB_S2ELABTestLine LL ON LH.ID = LL.LABID AND LL.ISCURRENTLOGS = 1
	                        WHERE (A.APPROVESTATUS = 5 AND (S.APPROVESTATUS NOT IN (5,7) OR S.APPROVESTATUS IS NULL )) OR
	                              (A.APPROVESTATUS = 7 AND LH.ID IS NULL)
                        )T 
                        WHERE " + filter + @" ORDER BY T.ORDERLIST,T.CREATEDATE,T.REQUESTID
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
