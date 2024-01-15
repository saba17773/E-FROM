using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Promotion.ApproveMapping
{
    public class EditModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [BindProperty]
        public PromotionApproveMappingTable ApproveMappingMaster { get; set; }

        public List<SelectListItem> ApproveMaster { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;

        public EditModel(
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

        public async Task OnGetAsync(int id)
        {
            try
            {
                InitialData();

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveMappingRepo = new GenericRepository<PromotionApproveMappingTable>(unitOfWork.Transaction);
                    var approveMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                    ApproveMaster = approveMasterRepo.GetAll()
                        .Where(x => x.IsActive == 1)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.GroupDescription
                        }).ToList();

                    var approveMapping = await approveMappingRepo.GetAsync(id);
                    ApproveMappingMaster = approveMapping;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveMappingRepo = new GenericRepository<PromotionApproveMappingTable>(unitOfWork.Transaction);

                    var approveMapping = await approveMappingRepo.GetAsync(id);

                    approveMapping.CCType = ApproveMappingMaster.CCType;
                    approveMapping.ApproveMasterId = ApproveMappingMaster.ApproveMasterId;

                    await approveMappingRepo.UpdateAsync(approveMapping);

                    unitOfWork.Complete();

                    AlertSuccess = "Edit Success.";
                    return Redirect($"/Promotion/ApproveMapping/{id}/Edit");
                }
            }
            catch (Exception ex)
            {
                AlertSuccess = ex.Message;
                return Redirect($"/Promotion/ApproveMapping/{id}/Edit");
            }
        }

        public async Task<IActionResult> OnGetDeleteMappingAsync(int idMap)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var attachFileRepo = new GenericRepository<PromotionAttachFileTable>(unitOfWork.Transaction);

                    var attachFile = await attachFileRepo.GetAsync(idMap);

                    // if (!System.IO.Directory.Exists(basePath))
                    // {
                    //     System.IO.File.Delete(basePath);
                    //     await attachFileRepo.DeleteAsync(new PromotionAttachFileTable
                    //     {
                    //         Id = IdFile
                    //     });
                    // }

                    unitOfWork.Complete();
                    AlertSuccess = "Delete Success.";
                    return new JsonResult(new { DeleteFile = 1 });
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