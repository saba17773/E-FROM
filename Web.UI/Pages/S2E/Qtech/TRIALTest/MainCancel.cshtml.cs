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
                        PROJECTREFNO = "PROJECTREFNO",
                        SUPPLIERNAME = "SUPPLIERNAME",
                        ITEMCODE = "ITEMCODE",
                        ITEMNAME = "ITEMNAME"

                    };

                    var filter = _datatablesService.Filter(HttpContext.Request, field);
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<S2ETrialTestGridModel>(@"
                        SELECT * FROM 
                        (
	                        SELECT TH.ID TRIALID,TH.RMREQID,TL.CHEMICALNAME,TL.APPROVESTATUS TRIALAPPROVESTATUS,
                            R.REQUESTCODE,TL.PROJECTREFNO, RH.ITEMCODE,RH.ITEMNAME,
                            LEFT(AH.SUPPLIERNAME, 30) + '<br/>' + LEFT(R.SUPPLIERNAME, 30) AS SUPPLIERNAME,
                            TL.APPROVEGROUPID ,TL.ID TRIALLINEID,TH.CANCELREMARK
                            FROM TB_S2ETrialTestHead TH JOIN
                            TB_S2ETrialTestLine TL ON TH.ID = TL.TRIALID AND TL.ISCURRENTLOGS = 1 JOIN
                            TB_S2EMaterialRequestHead RH ON TH.RMREQID = RH.ID JOIN
                            TB_S2EAddRawMaterialHead AH ON RH.ADDRMID = AH.ID JOIN
                            TB_S2EPurchaseSample PS ON AH.PCSAMPLEID = PS.ID JOIN
                            TB_S2ENewRequest R ON AH.REQUESTID = R.ID
                            WHERE TL.APPROVESTATUS = 2
                        )T4
                        WHERE " + filter + @" ORDER BY TRIALAPPROVESTATUS
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
