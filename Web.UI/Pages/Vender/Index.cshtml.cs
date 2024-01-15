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
using Web.UI.Interfaces;

namespace Web.UI.Pages.Vender
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        public IndexModel(
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
                await _authService.CanAccess(nameof(VenderPermissionModel.VIEW_VENDER));

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Vender");
            }
        }
        public async Task<JsonResult> OnPostGridAsync(string RequestCode, string DataareaID, string VendorAX,
                                                      string RequestDateFrom, string RequestDateTo, string VendorName, 
                                                      string AppStatus, int isFristLoad,string Requestby)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    
                    if (isFristLoad == 1)
                    {
                        var data = await unitOfWork.Transaction.Connection.QueryAsync<VenderTableGridModel>(@"
                            EXEC AVMS_GetVendorList
                        ",null, unitOfWork.Transaction);

                        unitOfWork.Complete();

                        return new JsonResult(_datatablesService.Format(Request, data.ToList()));
                    }
                    else
                    {
                        var data = await unitOfWork.Transaction.Connection.QueryAsync<VenderTableGridModel>(@"
                            EXEC AVMS_GetVendorList @RequestCode,@DataareaID,@VendorAX,@RequestDateFrom,@RequestDateTo,@VendorName,@AppStatus,@Requestby
                        ",
                        new
                        {
                            @RequestCode = RequestCode,
                            @DataareaID = DataareaID,
                            @VendorAX = VendorAX,
                            @RequestDateFrom = RequestDateFrom,
                            @RequestDateTo = RequestDateTo,
                            @VendorName = VendorName,
                            @AppStatus = AppStatus,
                            @Requestby = Requestby
                        }, unitOfWork.Transaction);

                        unitOfWork.Complete();

                        return new JsonResult(_datatablesService.Format(Request, data.ToList()));
                    }
                    
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
