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

namespace Web.UI.Pages.S2E.Qtech.MaterialRequest
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
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_RAWMATERIALREQUEST));

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
                        PROJECTREFNO = "PROJECTREFNO",
                        SUPPLIERNAME = "SUPPLIERNAME",
                        ITEMCODE = "ITEMCODE",
                        ITEMNAME = "ITEMNAME"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EMaterialRequestGridModel>(@"
                       SELECT *
                        FROM
                        (
	                        SELECT T4.RMREQID,T4.ADDRMID,T4.APPROVESTATUS,R.REQUESTCODE,LL.PROJECTREFNO,
	                        AH.VENDORID,LEFT(AH.SUPPLIERNAME, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
                            PS.ITEMCODE,PS.ITEMNAME,T4.REQUESTSTATUS,T4.CANCELREMARK
	                        FROM
	                        (
		                        SELECT T3.RMREQID,T3.APPROVESTATUS,H.ADDRMID,H.REQUESTSTATUS,T3.CANCELREMARK
		                        FROM
		                        (
			                        SELECT * 
			                        FROM (
				                        SELECT T1.RMREQID,T1.Total_ApproveStatus,L.APPROVESTATUS,
				                        COUNT(APPROVESTATUS) OVER (PARTITION BY L.RMREQID) AS Total_ApproveReject
                                        ,L.CANCELREMARK
				                        FROM (
					                        SELECT *
					                        FROM
					                        (
						                        SELECT RMREQID,
						                        COUNT(APPROVESTATUS) OVER (PARTITION BY RMREQID) AS Total_ApproveStatus
						                        FROM TB_S2EMaterialRequestLine 
					                        )T
					                        GROUP BY RMREQID,Total_ApproveStatus
				                        )T1 JOIN TB_S2EMaterialRequestLine L ON T1.RMREQID = L.RMREQID AND L.APPROVESTATUS = 2
			                        )T2
			                        WHERE T2.Total_ApproveStatus = T2.Total_ApproveReject
			                        GROUP BY T2.RMREQID,T2.Total_ApproveStatus,T2.Total_ApproveReject,T2.APPROVESTATUS,T2.CANCELREMARK
		                        )T3 JOIN TB_S2EMaterialRequestHead H ON T3.RMREQID = H.ID AND REQUESTSTATUS = 7
	                        )T4 JOIN
	                        TB_S2EAddRawMaterialHead AH ON T4.ADDRMID = AH.ID JOIN
	                        TB_S2ENewRequest R ON AH.REQUESTID = R.ID JOIN
	                        TB_S2EPurchaseSample PS ON AH.PCSAMPLEID = PS.ID JOIN
	                        TB_S2ELABTestHead LH ON AH.LABID = LH.ID JOIN
	                        TB_S2ELABTestLine LL ON AH.LABLINEID = LL.ID
                        )T5
                        WHERE " + filter + @" ORDER BY T5.RMREQID
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
