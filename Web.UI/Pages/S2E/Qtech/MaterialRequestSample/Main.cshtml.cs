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

namespace Web.UI.Pages.S2E.Qtech.MaterialRequestSample
{
    public class MainModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int isPurchase { get; set; }

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
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_RAWMATERIALREQUESTSAMPLE));

                if (await _authService.CanDisplay(nameof(S2EPermissionModel.VIEW_PURCHASE)))
                {
                    isPurchase = 1;
                }
                else
                {
                    isPurchase = 0;
                }
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
                        SUPPLIERNAME = "SUPPLIERNAME",
                        ITEMCODE = "ITEMCODE",
                        ITEMNAME = "ITEMNAME"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EMaterialRequestSampleGridModel>(@"
                        SELECT T2.*,
                            CASE 
                                WHEN (T2.SUMSUCCESSSTATUS = T2.COUNTTOTALREQLINE) AND (T2.ADDRMSAMPLEQTY = T2.RMREQQTYTOTAL)  
                                THEN 1 
                                ELSE 0 
                            END AS ISCOMPLETE 
                            FROM 
                            (
	                            SELECT T1.ADDRMSAMPLEID,
	                            T1.REQUESTCODE,
	                            T1.VENDORID,
	                            T1.SUPPLIERNAME,
	                            T1.ITEMCODE,
	                            T1.ITEMNAME,
	                            T1.ADDRMSAMPLEAPPROVESTATUS,
	                            T1.RMREQSAMID,
	                            T1.REQUESTSTATUS,
	                            SUM(T1.RMREQSAMPLEQTY)RMREQQTYTOTAL,
	                            T1.ADDRMSAMPLEQTY,
	                            SUM(REQSAMPLEAPPROVESTATUS)AS SUMSUCCESSSTATUS,
	                            COUNT(RMREQSAMLINEID) AS COUNTTOTALREQLINE
	                            FROM 
	                            (
		                            SELECT T.ADDRMSAMPLEID,
		                            T.REQUESTCODE,
		                            T.VENDORID,
		                            T.SUPPLIERNAME,
		                            T.ITEMCODE,
		                            T.ITEMNAME,
		                            T.APPROVESTATUS ADDRMSAMPLEAPPROVESTATUS,
		                            MR.ID RMREQSAMID,
		                            MR.REQUESTSTATUS,
		                            ML.ID RMREQSAMLINEID,
		                            CASE WHEN ML.APPROVESTATUS = 7 THEN 1 ELSE 0 END AS REQSAMPLEAPPROVESTATUS,
		                            ML.QTY AS RMREQSAMPLEQTY,
		                            T.QTY ADDRMSAMPLEQTY 
		                            FROM 
		                            (
			                            SELECT S.ID AS ADDRMSAMPLEID,
			                            S.REQUESTID,
			                            S.ASSESSMENTID,
			                            S.PLANT,
			                            S.APPROVESTATUS,
			                            R.QTY,
			                            R.VENDORID,
			                            LEFT(R.DEALER, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
			                            R.REQUESTCODE,
			                            R.ITEMCODE,
			                            R.ITEMNAME
			                            FROM TB_S2EAddRawMaterialSample S JOIN
			                            TB_S2ENewRequest R ON S.REQUESTID = R.ID
			                            WHERE S.APPROVESTATUS IN (5,7)
		                            )T LEFT JOIN
		                            TB_S2EMaterialRequestSampleHead MR ON T.ADDRMSAMPLEID = MR.ADDRMSAMPLEID LEFT JOIN
		                            TB_S2EMaterialRequestSampleLine ML ON MR.ID = ML.RMREQSAMID AND ML.APPROVESTATUS <> 2
	                            )T1
	                            GROUP BY T1.ADDRMSAMPLEID,
	                            T1.REQUESTCODE,
	                            T1.VENDORID,
	                            T1.SUPPLIERNAME,
	                            T1.ITEMCODE,
	                            T1.ITEMNAME,
	                            T1.ADDRMSAMPLEAPPROVESTATUS,
	                            T1.RMREQSAMID,
	                            T1.REQUESTSTATUS,
	                            T1.ADDRMSAMPLEQTY
                            )T2
                        WHERE " + filter + @" ORDER BY T2.REQUESTSTATUS
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
