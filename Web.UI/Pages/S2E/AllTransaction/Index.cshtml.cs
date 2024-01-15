using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.Models.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.AllTransaction
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string RequestNo { get; set; }
        public List<SelectListItem> RequestNoMaster { get; set; }
        [BindProperty]
        public string Status { get; set; }
        public List<SelectListItem> RequestStatusMaster { get; set; }

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
                await _authService.CanAccess(nameof(S2EPermissionModel.VIEW_ALLTRANSACTION));

                RequestNoMaster = await GetRequestNoMaster();
                RequestStatusMaster = await GetRequestStatusMaster();

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/S2E");
            }
        }
        public async Task<List<SelectListItem>> GetRequestNoMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var LABTestLineRepo = new GenericRepository<S2ELABTestLine_TB>(unitOfWork.Transaction);

                var LABTestLineALL = await LABTestLineRepo.GetAllAsync();

                return LABTestLineALL
                    .Where(w => w.PROJECTREFNO != null)
                    .Select(x => new SelectListItem
                    {
                        Value = x.PROJECTREFNO,
                        Text = x.PROJECTREFNO
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetRequestStatusMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var StatusALL = await unitOfWork.Transaction.Connection.QueryAsync<S2EAllTransactionGridModel>(@"
                      EXEC S2EAllTransactionMain_Status
                        ", null, unitOfWork.Transaction);

                unitOfWork.Complete();

                return StatusALL
                    .Select(x => new SelectListItem
                    {
                        Value = x.STATUSAPPROVE,
                        Text = x.STATUSAPPROVE
                    })
                    .ToList();
            }
        }
        public async Task<JsonResult> OnPostGridAsync(int isFristLoad,string pSearch, string pStatus, string pRequestNo)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {

                    if (isFristLoad == 0)
                    {
                        var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EAllTransactionGridModel>(@"
                            EXEC S2EAllTransactionMain
                        ", null, unitOfWork.Transaction);

                        unitOfWork.Complete();

                        return new JsonResult(_datatablesService.Format(Request, data.ToList()));
                    }
                    else
                    {
                        var data = await unitOfWork.Transaction.Connection.QueryAsync<S2EAllTransactionGridModel>(@"
                            EXEC S2EAllTransactionMain @pSearch,@pStatus,@pRequestNo
                        ", new { @pSearch = pSearch, @pStatus = pStatus, @pRequestNo = pRequestNo }, unitOfWork.Transaction);

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
