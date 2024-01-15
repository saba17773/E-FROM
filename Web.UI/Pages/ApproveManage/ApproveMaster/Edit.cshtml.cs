using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class EditModel : PageModel
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

        public EditModel(
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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                //await _authService.CanAccess(nameof(UserPermissionModel.EDIT_USER));

                await GetData(id);

                return Page();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task GetData(int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var ApproveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                var ApproveMaster = await ApproveMasterRepo.GetAsync(id);
                if (ApproveMasterRepo == null)
                {
                    throw new Exception("Approve master not found.");
                }

                GroupDescription = ApproveMaster.GroupDescription;
                IsActive = ApproveMaster.IsActive;

                unitOfWork.Complete();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                //await _authService.CanAccess(nameof(UserPermissionModel.EDIT_USER));

                if (!ModelState.IsValid)
                {
                    await GetData(id);

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var ApproveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                    var ApproveMasterAll = await ApproveMasterRepo.GetAllAsync();

                    var ApproveMaster = await ApproveMasterRepo.GetAsync(id);
                    if (ApproveMaster == null)
                    {
                        throw new Exception("Approve master not found.");
                    }

                    var checkUserExists = ApproveMasterAll
                       .Where(x =>
                         x.GroupDescription == GroupDescription &&
                         x.Id != ApproveMaster.Id)
                       .FirstOrDefault();

                    if (checkUserExists != null)
                    {
                        throw new Exception("This username \"" + GroupDescription + "\" already used.");
                    }

                    ApproveMaster.GroupDescription = GroupDescription;
                    ApproveMaster.IsActive = IsActive;

                    await ApproveMasterRepo.UpdateAsync(ApproveMaster);

                    unitOfWork.Complete();

                    AlertSuccess = "Edit Approve Master Success.";

                    return Redirect("/ApproveManage/ApproveMaster");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/ApproveManage/ApproveMaster/" + id + "/Edit");
            }
        }

    }
}
