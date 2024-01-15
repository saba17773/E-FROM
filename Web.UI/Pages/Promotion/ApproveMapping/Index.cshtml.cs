using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Promotion.ApproveMapping
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        public PromotionApproveMappingTable ApproveMappingMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;

        public IndexModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
        }

        private void InitialData()
        {
            ApproveMappingMaster = new PromotionApproveMappingTable();
        }

        public void OnGet()
        {
            InitialData();
        }

        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    // var approveMappingRepo = new GenericRepository<PromotionApproveMappingTable>(unitOfWork.Transaction);

                    // var approveMappingAll = await approveMappingRepo.GetAllAsync();

                    // return new JsonResult(_datatableService.FormatOnce(approveMappingAll.ToList()));
                    var data = await unitOfWork.Transaction.Connection.QueryAsync<PromotionApproveMappingGridViewModel>(@"
                        SELECT 
                            M.Id,
                            M.CCType,
                            M.ApproveMasterId,
                            A.GroupDescription,
                            M.CreateBy,
                            U.Username,
                            M.TypeProduct
                        FROM TB_PromotionApproveMapping M
                        LEFT JOIN TB_User U ON M.CreateBy = U.Id
                        LEFT JOIN TB_ApproveMaster A ON M.ApproveMasterId = A.Id
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.FormatOnce(data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnGetDeleteMapAsync(int idmap)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approvemappingRepo = new GenericRepository<PromotionApproveMappingTable>(unitOfWork.Transaction);

                    await approvemappingRepo.DeleteAsync(new PromotionApproveMappingTable
                    {
                        Id = idmap
                    });

                    unitOfWork.Complete();
                    AlertSuccess = "Delete Success.";
                    return new JsonResult(new { DeleteMapping = 1 });
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                throw;
            }
        }
    }
}