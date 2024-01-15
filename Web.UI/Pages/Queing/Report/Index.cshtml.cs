using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Report
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public string PlantView { get; set; }

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
        public async Task<IActionResult> OnGetAsync(string plant)
        {
            try
            {
                await _authService.CanAccess(nameof(QueingPermissionModel.VIEW_QUEING_REPORT));

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    Plant = plant;

                    var PlantRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                    var PlantALL = await PlantRepo.GetAllAsync();
                    var PlantbyPlantCode = PlantALL.Where(x => x.company == plant).FirstOrDefault();

                    PlantView = PlantbyPlantCode.FullName_EN;

                }

                return Page();
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Process/{plant}/Process");
            }
        }
    }
}
