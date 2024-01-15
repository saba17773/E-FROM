using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Process
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public string Plant { get; set; }
        public List<SelectListItem> PlantMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public IndexModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.VIEW_QUEING));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    PlantMaster = await GetPlantByUser();
                }

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Home");
            }
        }

        private async Task<List<SelectListItem>> GetPlantByUser()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var UserID = _authService.GetClaim().UserId;

                var PlantByUserRepo = new GenericRepository<QingMaster_UserMapping_TB>(unitOfWork.Transaction);
                var PlantByUserALL = await PlantByUserRepo.GetAllAsync();

                unitOfWork.Complete();

                return PlantByUserALL
                    .Where(x => x.USERID == UserID)
                    .Select(x => new SelectListItem
                    {
                        Value = x.PLANT.ToString(),
                        Text = x.PLANT.ToString(),
                    }).ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    PlantMaster = await GetPlantByUser();
                }

                return Page();
            }

            try
            {
                return Redirect($"/Queing/Process/{Plant}/Process");
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Queing/Process/Main");
            }
        }
    }
}
