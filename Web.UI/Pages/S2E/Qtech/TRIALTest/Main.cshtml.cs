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

namespace Web.UI.Pages.S2E.Qtech.TRIALTest
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
        private IEmailService _emailService;
        public MainModel(
         IDatabaseContext databaseContext,
         IDatatableService datatablesService,
         IAuthService authService,
         IEmailService emailService)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _authService = authService;
            _emailService = emailService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_TRIALTEST));

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
                        REQUESTNO = "PROJECTREFNO",
                        SUPPLIERNAME = "SUPPLIERNAME",
                        ITEMCODE = "ITEMCODE",
                        ITEMNAME = "ITEMNAME"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ETrialTestGridModel>(@"
                        SELECT * FROM 
                        (
	                        SELECT T3.RMREQID,
	                        T3.REQUESTSTATUS,
	                        T3.REQUESTCODE,
	                        T3.VENDORID,
	                        T3.SUPPLIERNAME,
	                        T3.ITEMCODE,
	                        T3.ITEMNAME,
	                        TH.ID TRIALID,
                            TL.ID TRIALLINEID,
                            TL.PROJECTREFNO,
	                        TL.APPROVESTATUS TRIALAPPROVESTATUS,
	                        TL.CREATEBY REQUESTBY,
	                        TL.APPROVEGROUPID,
                            CASE WHEN TL.APPROVESTATUS IS NULL THEN 1 
                            WHEN TL.APPROVESTATUS = 8 THEN 2
                            WHEN TL.APPROVESTATUS = 4 THEN 3
                            WHEN TL.APPROVESTATUS = 3 THEN 4
                            ELSE 5 END AS ORDERLIST
	                        FROM (
		                        SELECT *,
		                        CASE WHEN T2.COUNTROW = CHKAPPSTATUS THEN 1 ELSE 0 END AS ISCANOPEN 
		                        FROM (
			                        SELECT T.RMREQID,
			                        COUNT(T.RMREQLINEID) AS COUNTROW,
			                        SUM(T.CHKAPPROVESTATUS) AS CHKAPPSTATUS,
			                        T.REQUESTSTATUS,
			                        T.REQUESTCODE,
			                        T.VENDORID,
			                        T.SUPPLIERNAME,
			                        T.ITEMCODE,T.ITEMNAME
			                        FROM
			                        (
				                        SELECT RH.ID AS RMREQID ,
				                        RL.ID RMREQLINEID,
				                        RH.ADDRMID,
				                        CASE WHEN RL.APPROVESTATUS = 7 THEN 1 ELSE 0 END AS CHKAPPROVESTATUS,
				                        RH.REQUESTSTATUS,
				                        R.REQUESTCODE,
				                        AH.VENDORID,
				                        LEFT(AH.SUPPLIERNAME, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
				                        RH.ITEMCODE,
				                        RH.ITEMNAME
				                        FROM TB_S2EMaterialRequestHead RH JOIN
				                        TB_S2EMaterialRequestLine RL ON RH.ID = RL.RMREQID JOIN
				                        TB_S2EAddRawMaterialHead AH ON RH.ADDRMID = AH.ID JOIN
				                        TB_S2ENewRequest R ON AH.REQUESTID = R.ID JOIN
				                        TB_S2EPurchaseSample PS ON AH.PCSAMPLEID = PS.ID
				                        WHERE RH.REQUESTSTATUS IN (1,5,7) 
				                        AND  RL.APPROVESTATUS <> 2
			                        )T
			                        GROUP BY T.RMREQID,T.REQUESTSTATUS,T.REQUESTCODE,T.VENDORID,
			                        T.SUPPLIERNAME,T.ITEMCODE,T.ITEMNAME
		                        )T2
	                        )T3 LEFT JOIN 
	                        TB_S2ETrialTestHead TH ON TH.RMREQID = T3.RMREQID LEFT JOIN
	                        TB_S2ETrialTestLine TL ON TH.ID = TL.TRIALID AND TL.ISCURRENTLOGS = 1
	                        WHERE ISCANOPEN = 1
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
