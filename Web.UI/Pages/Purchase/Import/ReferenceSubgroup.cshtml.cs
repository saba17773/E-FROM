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
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Purchase.Import
{
    public class ReferenceSubgroupModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        public List<SelectListItem> SubGroupIDMaster { get; set; }
        [BindProperty]
        public List<string> SubGroupID { get; set; }
        [BindProperty]
        public List<string> RefSubGroupID { get; set; }
        [BindProperty]
        public List<string> GroupID { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;
        private IConfiguration _configuration;

        public ReferenceSubgroupModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService,
          IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatableService;
            _authService = authService;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(ImportPermissionModel.VIEW_IMPORT_REFERENCE_SUBGROUP));

                SubGroupIDMaster = GetSubGroupIDMaster();
               
                return Page();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public List<SelectListItem> GetSubGroupIDMaster()
        {
            var connString = "AXCust";

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection(connString)))
            {
                var selectax = unitOfWork.Transaction.Connection.Query(@"SELECT DSGSUBGROUPID 
                        FROM DSGSUBGROUP
                        WHERE DSGSUBGROUPID LIKE 'RM%' AND DATAAREAID = 'DV'
                        ORDER BY DSGSUBGROUPID
                    ", null, unitOfWork.Transaction
                );

                return selectax.Select(x => new SelectListItem
                {
                    Value = x.DSGSUBGROUPID,
                    Text = x.DSGSUBGROUPID
                }).ToList();
            }
        }

        public JsonResult OnGetSubGroupID2()
        {
            var connString = "AXCust";

            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection(connString)))
            {
                var selectax = unitOfWork.Transaction.Connection.Query(@"SELECT DSGSUBGROUPID 
                        FROM DSGSUBGROUP
                        WHERE DSGSUBGROUPID LIKE 'RM%' AND DATAAREAID = 'DV'
                        ORDER BY DSGSUBGROUPID
                    ", null, unitOfWork.Transaction
                );

                return new JsonResult(selectax.ToList());
            }

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                SubGroupIDMaster = GetSubGroupIDMaster();

                return Page();
            }

            try
            {
                var connString = "Import";
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection(connString)))
                {
                    int row = SubGroupID.Count();
                    for (int i = 0; i < row ; i++)
                    {
                        if(SubGroupID[i] != null )
                        {

                            var SubGroupRepo = new GenericRepository<ImportReferenceSubGroupTable>(unitOfWork.Transaction);
                            var SubGroupALL = await SubGroupRepo.GetAllAsync();
                            var SubGroup = SubGroupALL.Where(x=>x.SubgroupId == SubGroupID[i]).Count();

                            if (SubGroup == 0)
                            {
                                var newSubGroup = new ImportReferenceSubGroupTable
                                {
                                    SubgroupId = SubGroupID[i],
                                    RefSubgroupId = RefSubGroupID[i],
                                    GroupId = GroupID[i],
                                    isCheck = 1
                                };

                                await SubGroupRepo.InsertAsync(newSubGroup);
                            }
                            else
                            {
                                var SubGroupRepo2 = new GenericRepository<ImportReferenceSubGroupTable>(unitOfWork.Transaction);
                                var SubGroupALL2 = await SubGroupRepo2.GetAllAsync();
                                var SubGroup2 = SubGroupALL2.Where(x => x.SubgroupId == SubGroupID[i]).FirstOrDefault();

                                SubGroup2.RefSubgroupId = RefSubGroupID[i];
                                SubGroup2.GroupId = GroupID[i];
                                SubGroup2.isCheck = 1;

                                await SubGroupRepo2.UpdateAsync(SubGroup2);
                            }
                        }
                        
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Successfully.";

                    return Redirect("/Purchase/Import/ReferenceSubgroup");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/Purchase/Import/ReferenceSubgroup");
            }
        }


    }
}
