using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.ApproveMapping
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        public S2EApproveMapping_TB ApproveMappingMaster { get; set; }

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
            ApproveMappingMaster = new S2EApproveMapping_TB();
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
                    var approveMappingRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var employeeRepo = new GenericRepository<EmployeeRepository>(unitOfWork.Transaction);
                    var approveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);
                    var S2EGroupRepo = new GenericRepository<S2EApproveGroup_TB>(unitOfWork.Transaction);

                    var approveMappingAll = await approveMappingRepo.GetAllAsync();

                    var approveMappingGrid = new List<ApproveMappingGridModel>();

                    foreach (var item in approveMappingAll.ToList())
                    {
                        var user = await userRepo.GetAsync(item.CreateBy);
                        var approveMaster = await approveMasterRepo.GetAsync(item.APPROVEMASTERID);
                        var S2EGroup = await S2EGroupRepo.GetAsync(item.APPROVEGROUPID);

                        approveMappingGrid.Add(new ApproveMappingGridModel
                        {
                            CreateBy = user.Username,
                            ApproveMaster = approveMaster.GroupDescription,
                            S2EType = S2EGroup.GROUPDESCRIPTION,
                            Id = item.ID
                        });
                    }

                    return new JsonResult(_datatableService.FormatOnce(approveMappingGrid));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> OnGetDeleteAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveMappingRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);

                    var approveMapping = await approveMappingRepo.GetAsync(id);

                    await approveMappingRepo.DeleteAsync(approveMapping);

                    unitOfWork.Complete();

                    AlertSuccess = "Delete Success.";

                    return Redirect($"/S2E/ApproveMapping");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;

                return Redirect($"/S2E/ApproveMapping");
            }
        }
    }
}
