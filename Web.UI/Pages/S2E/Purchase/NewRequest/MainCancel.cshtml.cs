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

namespace Web.UI.Pages.S2E.Purchase.NewRequest
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
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_NEWREQUEST));

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
                        REQUESTBY = "REQUESTBY"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ENewRequestGridModel>(@"
                          SELECT T.* 
                            FROM (
                                SELECT R.ID,R.REQUESTCODE,L.PROJECTREFNO AS REQUESTNO,
                                    LEFT(R.DEALER, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
                                    U.Email REQUESTBY,R.APPROVESTATUS,
                                    R.CREATEDATE,R.APPROVEGROUPID,
                                    'Main' AS PAGEBACK,
                                     CASE WHEN R.APPROVESTATUS = 8 THEN 1
									WHEN R.APPROVESTATUS = 3 THEN 2
									ELSE 3 END AS ORDERLIST,
                                    R.CANCELREMARK
                                FROM TB_S2ENewRequest R JOIN
                                TB_User U ON R.CREATEBY = U.Id LEFT JOIN
                                TB_S2ELABTestHead H ON R.ID = H.REQUESTID LEFT JOIN
                                TB_S2ELABTestLine L ON H.ID = L.LABID AND L.ISCURRENTLOGS = 1
                                WHERE R.APPROVESTATUS = 2
                            )T
                        
                        WHERE " + filter + @" ORDER BY T.CREATEDATE 
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
