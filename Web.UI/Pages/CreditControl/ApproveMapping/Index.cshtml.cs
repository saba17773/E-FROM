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
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.CreditControl.ApproveMapping
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        public CreditControlApproveMappingTable ApproveMappingMaster { get; set; }

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
            ApproveMappingMaster = new CreditControlApproveMappingTable();
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
                    var approveMappingRepo = new GenericRepository<CreditControlApproveMappingTable>(unitOfWork.Transaction);
                    var userRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                    var employeeRepo = new GenericRepository<EmployeeRepository>(unitOfWork.Transaction);
                    var approveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                    var approveMappingAll = await approveMappingRepo.GetAllAsync();

                    var approveMappingGrid = new List<ApproveMappingGridViewModel>();

                    foreach (var item in approveMappingAll.ToList())
                    {
                        var user = await userRepo.GetAsync(item.CreateBy);
                        var approveMaster = await approveMasterRepo.GetAsync(item.ApproveMasterId);

                        if (user != null)
                        {
                            approveMappingGrid.Add(new ApproveMappingGridViewModel
                            {
                                CreateBy = user.Username,
                                ApproveMaster = approveMaster.GroupDescription,
                                CreditControlType = item.CCType,
                                Id = item.Id
                            });
                        }
                       
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
                    var approveMappingRepo = new GenericRepository<CreditControlApproveMappingTable>(unitOfWork.Transaction);

                    var approveMapping = await approveMappingRepo.GetAsync(id);

                    await approveMappingRepo.DeleteAsync(approveMapping);

                    unitOfWork.Complete();

                    AlertSuccess = "Delete Success.";

                    return Redirect($"/CreditControl/ApproveMapping");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;

                return Redirect($"/CreditControl/ApproveMapping");
            }
        }
    }
}