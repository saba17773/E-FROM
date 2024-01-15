using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Master.All
{
    public class AgentTractorModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string AgentCode_Add { get; set; }
        [BindProperty]
        public string AgentDESC_Add { get; set; }
        [BindProperty]
        public int AgentIsActive_Add { get; set; }
        [BindProperty]
        public int AgentId_Edit { get; set; }
        [BindProperty]
        public string AgentCode_Edit { get; set; }
        [BindProperty]
        public string AgentDESC_Edit { get; set; }
        [BindProperty]
        public int AgentIsActive_Edit { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public AgentTractorModel(
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
            await _authService.CanAccess(nameof(QueingPermissionModel.ADMIN_QUEING));
            return Page();
        }
        public async Task<JsonResult> OnPostGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        AGENTCODE = "AGENTCODE"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<QingMaster_AgentTractor_TB>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT * FROM TB_QingMaster_AgentTractor
                        )T
                        WHERE " + filter + @"
                    ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, data.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> OnPostAsync(string add, string edit)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var AgentRepo = new GenericRepository<QingMaster_AgentTractor_TB>(unitOfWork.Transaction);

                    if (!string.IsNullOrEmpty(add))
                    {
                        await AgentRepo.InsertAsync(new QingMaster_AgentTractor_TB
                        {
                            AGENTCODE = AgentCode_Add,
                            AGENTDESCRIPTION = AgentDESC_Add == null ? "" : AgentDESC_Add,
                            ISACTIVE = AgentIsActive_Add == 1 ? 1 : 0
                        });

                        AlertSuccess = "Add Agent Success";
                    }

                    if (!string.IsNullOrEmpty(edit))
                    {
                        var AgentById = await AgentRepo.GetAsync(AgentId_Edit);

                        AgentById.AGENTCODE = AgentCode_Edit;
                        AgentById.AGENTDESCRIPTION = AgentDESC_Edit == null ? "" : AgentDESC_Edit;
                        AgentById.ISACTIVE = AgentIsActive_Edit == 1 ? 1 : 0;

                        await AgentRepo.UpdateAsync(AgentById);

                        AlertSuccess = "Edit Agent Success";
                    }

                    unitOfWork.Complete();
                    return Redirect($"/Queing/Master/All/AgentTractor");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Master/All/AgentTractor");
            }
        }
    }
}
