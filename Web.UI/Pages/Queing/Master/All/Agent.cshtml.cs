using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities.Queing;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Queing.Master.All
{
    public class AgentModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }
        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public string AgentName_Add { get; set; }
        [BindProperty]
        public string Company_Add { get; set; }
        public List<SelectListItem> Company_AddMaster { get; set; }
        [BindProperty]
        public int IsActive_Add { get; set; }
        [BindProperty]
        public int IsRemark_Add { get; set; }
        [BindProperty]
        public int AgentId_Edit { get; set; }
        [BindProperty]
        public string AgentName_Edit { get; set; }
        [BindProperty]
        public string Company_Edit { get; set; }
        [BindProperty]
        public int IsActive_Edit { get; set; }
        [BindProperty]
        public int IsRemark_Edit { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public AgentModel(
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
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                Company_AddMaster = await GetCompanyCombo();
            }
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
                        AGENTNAME = "AGENTNAME",
                        COMPANY = "COMPANY"
                    };

                    var filter = _datatableService.Filter(HttpContext.Request, field);

                    var data = await unitOfWork.Transaction.Connection.QueryAsync<QingMaster_Agent_TB>(@"
                      SELECT *
                        FROM
                        (
	                        SELECT * FROM TB_QingMaster_Agent
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
        private async Task<List<SelectListItem>> GetCompanyCombo()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {

                var CompanyRepo = new GenericRepository<QingMaster_Company_TB>(unitOfWork.Transaction);
                var CompanyALL = await CompanyRepo.GetAllAsync();

                unitOfWork.Complete();

                return CompanyALL
                    .Where(x => x.IsQingCompany == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.company,
                        Text = x.company,
                    }).ToList();
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
                    var AgentRepo = new GenericRepository<QingMaster_Agent_TB>(unitOfWork.Transaction);

                    if (!string.IsNullOrEmpty(add))
                    {
                        await AgentRepo.InsertAsync(new QingMaster_Agent_TB
                        {
                            AGENTNAME = AgentName_Add,
                            COMPANY = Company_Add,
                            ISREMARK = IsRemark_Add == 1 ? 1 : 0,
                            ISACTIVE = IsActive_Add == 1 ? 1 : 0
                        });

                        AlertSuccess = "Add Agent Success";
                    }

                    if (!string.IsNullOrEmpty(edit))
                    {
                        var AgentById = await AgentRepo.GetAsync(AgentId_Edit);

                        AgentById.AGENTNAME = AgentName_Edit;
                        AgentById.ISREMARK = IsRemark_Edit == 1 ? 1 : 0;
                        AgentById.ISACTIVE = IsActive_Edit == 1 ? 1 : 0;

                        await AgentRepo.UpdateAsync(AgentById);

                        AlertSuccess = "Edit Agent Success";
                    }

                    unitOfWork.Complete();
                    return Redirect($"/Queing/Master/All/Agent");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Queing/Master/All/Agent");
            }
        }
    }
}
