using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.ApproveManage.ApproveMaster
{
    public class AddModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        [Required]
        [StringLength(100)]
        public string GroupDescription { get; set; }

        [Required]
        [BindProperty]
        public int IsActive { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IHelperService _helperService;

        public AddModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IHelperService helperService)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatableService;
            _authService = authService;
            _helperService = helperService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(VenderPermissionModel.VIEW_VENDER));
                return Page();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                /*await _authService.CanAccess(nameof(UserPermissionModel.ADD_USER));*/

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var ApproveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                    await ApproveMasterRepo.InsertAsync(new ApproveMasterTable
                    {
                        GroupDescription = GroupDescription,
                        IsActive = IsActive
                    });

                    unitOfWork.Complete();

                    AlertSuccess = "Add New Approve Master Success.";

                    return Redirect("/ApproveManage/ApproveMaster");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/ApproveManage/ApproveMaster/Add");
            }
        }

    }
}
