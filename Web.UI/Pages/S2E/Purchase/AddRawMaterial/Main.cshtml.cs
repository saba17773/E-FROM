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

namespace Web.UI.Pages.S2E.Purchase.AddRawMaterial
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
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_ADDRAWMATERIAL));

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

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EAddRawMaterialGridModel>(@"
                         SELECT T1.* 
                         FROM (
                            SELECT T.* 
                            FROM (
                                SELECT PS.ID AS PCSAMPLEID,
	                                PS.REQUESTID,
	                                PS.ASSESSMENTID,
	                                PS.LABID,
	                                PS.LABLINEID,
	                                AH.VENDORID,
	                                LEFT(AH.SUPPLIERNAME, 30) + '<br/>' + LEFT(PC.SUPPLIERNAME, 30) AS SUPPLIERNAME,
                                    --AH.SUPPLIERNAME,
	                                LL.ISPURCHASESAMPLE,
	                                PS.APPROVEMASTERID,
	                                PS.APPROVESTATUS AS PCSAMPLESTATUS,
	                                U.Email REQUESTBY,
	                                PC.REQUESTCODE,
	                                LL.PROJECTREFNO,
	                                AH.ID AS ADDRMID,
                                    AL.ID AS ADDRMLINEID,
	                                AL.APPROVESTATUS AS ADDRMAPPROVESTATUS,
	                                AL.APPROVEGROUPID,
                                    AH.ISADDMORE,
                                    AL.REQUESTDATE,
                                    CASE WHEN AL.APPROVESTATUS IS NULL THEN 1 
									WHEN AL.APPROVESTATUS = 8 THEN 2
									WHEN AL.APPROVESTATUS = 4 THEN 3
									WHEN AL.APPROVESTATUS = 3 THEN 4
									WHEN AL.APPROVESTATUS = 5 THEN 4
									ELSE 6 END AS ORDERLIST
                                FROM TB_S2EPurchaseSample PS JOIN
                                TB_User U ON PS.CREATEBY = U.Id JOIN
                                TB_S2ENewRequest PC ON PS.REQUESTID = PC.ID JOIN
                                TB_S2ERMAssessment RM ON PS.ASSESSMENTID = RM.ID JOIN
                                TB_S2ELABTestHead LH ON PS.LABID = LH.ID LEFT JOIN
                                TB_S2ELABTestLine LL ON PS.LABLINEID = LL.ID LEFT JOIN
                                TB_S2EAddRawMaterialHead AH ON AH.PCSAMPLEID = PS.ID LEFT JOIN
                                TB_S2EAddRawMaterialLine AL ON AL.ADDRMID = AH.ID AND AL.ISCURRENTLOGS = 1 
                            )T
                            WHERE T.ADDRMAPPROVESTATUS NOT IN (2) OR T.ADDRMAPPROVESTATUS IS NULL 
                         )T1
                         WHERE " + filter + @" ORDER BY T1.ORDERLIST,T1.REQUESTDATE
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
