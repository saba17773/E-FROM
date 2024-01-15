using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.S2E;
using Web.UI.Infrastructure.ViewModels.S2E;
using Web.UI.Interfaces;

namespace Web.UI.Pages.S2E.ApproveMapping
{
    public class AddModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }
        [BindProperty]
        public int ApproveGroupID { get; set; }
        public List<SelectListItem> ApproveGroupIDMaster { get; set; }
        [BindProperty]
        public int CreateBy { get; set; }
        [BindProperty]
        public int ApproveMasterID { get; set; }
        [BindProperty]
        public string DescriptionMapping { get; set; }
        [BindProperty]
        public int Step { get; set; }
        [BindProperty]
        public bool IsNewRequest { get; set; }
        [BindProperty]
        public bool IsRMAssessment { get; set; }
        [BindProperty]
        public bool IsLabTest { get; set; }
        [BindProperty]
        public bool IsPurchaseSample { get; set; }
        [BindProperty]
        public bool IsAddRM { get; set; }
        [BindProperty]
        public bool IsRequestRM { get; set; }
        [BindProperty]
        public bool IsTrialtest { get; set; }
        [BindProperty]
        public bool IsAddMoreRM { get; set; }
        [BindProperty]
        public string Plant { get; set; }
        [BindProperty]
        public bool IsAddRMSample { get; set; }
        [BindProperty]
        public bool IsRequestRMSample { get; set; }

        private IDatabaseContext _databaseContext;
        private IDatatableService _datatableService;
        private IAuthService _authService;

        public AddModel(
          IDatabaseContext databaseContext,
          IDatatableService datatableService,
          IAuthService authService)
        {
            _databaseContext = databaseContext;
            _datatableService = datatableService;
            _authService = authService;
        }

    
        public async Task OnGetAsync()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                ApproveGroupIDMaster = await GetS2EApproveGroup();
            }
        }
        private async Task<List<SelectListItem>> GetS2EApproveGroup()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var S2EAppGroupRepo = new GenericRepository<S2EApproveGroup_TB>(unitOfWork.Transaction);

                var S2EAppGroupALL = await S2EAppGroupRepo.GetAllAsync();

                unitOfWork.Complete();

                return S2EAppGroupALL.Where(x => x.ISACTIVE == 1).Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GROUPDESCRIPTION,
                }).ToList();
            }
        }
        public async Task<JsonResult> OnPostUserGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        USERNAME = "USERNAME",
                        EMPLOYEEID = "EMPLOYEEID",
                        EMAIL = "EMAIL"
                    };

                    var filter = _datatableService.Filter(Request, field);

                    var ItemSample = await unitOfWork.Transaction.Connection.QueryAsync<S2EUserGridViewModel>(@"
                       SELECT * 
                        FROM (
	                        SELECT U.ID CREATEBY,U.USERNAME,
	                        U.EMPLOYEEID,U.EMAIL,
	                        U.USERDOMAIN
	                        FROM TB_User U 
	                        WHERE IsActive = 1
                        )T
                        WHERE " + filter + @" 
                        ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, ItemSample.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<JsonResult> OnPostApproveMasterGridAsync()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var field = new
                    {
                        GROUPDESCRIPTION = "GROUPDESCRIPTION"
                    };

                    var filter = _datatableService.Filter(Request, field);

                    var ItemSample = await unitOfWork.Transaction.Connection.QueryAsync<ApproveMasterTable>(@"
                       SELECT * 
                        FROM (
	                        SELECT Id,GroupDescription
                            FROM TB_ApproveMaster
                            WHERE isS2EProject = 1 AND IsActive = 1
                        )T
                        WHERE " + filter + @" 
                        ", null, unitOfWork.Transaction);

                    unitOfWork.Complete();

                    return new JsonResult(_datatableService.Format(Request, ItemSample.ToList()));
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<IActionResult> OnGetCheackGroupID(int GroupID)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var ApproveGroupRepo = new GenericRepository<S2EApproveGroup_TB>(unitOfWork.Transaction);
                    var ApproveGroupByID = await ApproveGroupRepo.GetAsync(GroupID);

                    unitOfWork.Complete();

                    if (ApproveGroupByID.ISPLANT == 1)
                    {
                        return new JsonResult(false);
                    }
                    else
                    {
                        return new JsonResult(true);
                    }

                }
            }
            catch (Exception)
            {
                return new JsonResult(false);
            }

        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ApproveGroupIDMaster = await GetS2EApproveGroup();

                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var approveMappingRepo = new GenericRepository<S2EApproveMapping_TB>(unitOfWork.Transaction);
                    if (Plant == "")
                    {
                        var approveMappingALL = await approveMappingRepo.GetAllAsync();
                        var Check = approveMappingALL.Where(x => x.CreateBy == CreateBy &&
                                                            x.APPROVEGROUPID == ApproveGroupID).FirstOrDefault();
                        if (Check != null)
                        {
                            AlertError = "User นี้ Mapping กับ Group นี้ไปแล้้ว";
                            return Redirect($"/S2E/ApproveMapping/Add");
                        }
                    }
                    else
                    {
                        var approveMappingALL = await approveMappingRepo.GetAllAsync();
                        var Check = approveMappingALL.Where(x => x.CreateBy == CreateBy &&
                                                            x.APPROVEGROUPID == ApproveGroupID &&
                                                            x.PLANT == Plant).FirstOrDefault();
                        if (Check != null)
                        {
                            AlertError = "User นี้ Mapping กับ Group นี้ไปแล้้ว";
                            return Redirect($"/S2E/ApproveMapping/Add");
                        }
                    }
                
                    var newApproveMapping = new S2EApproveMapping_TB
                    {
                        APPROVEMASTERID = ApproveMasterID,
                        APPROVEGROUPID = ApproveGroupID,
                        CreateBy = CreateBy,
                        STEP = Step,
                        PLANT = Plant,
                        DESCRIPTION = DescriptionMapping,
                        ISNEWREQUEST = IsNewRequest == true ? 1 : 0,
                        ISRMASSESSMENT = IsRMAssessment == true ? 1 : 0,
                        ISLABTEST = IsLabTest == true ? 1 : 0,
                        ISPURCHASESAMPLE = IsPurchaseSample == true ? 1 : 0,
                        ISADDRM = IsAddRM == true ? 1 : 0,
                        ISREQUESTRM = IsRequestRM == true ? 1 : 0,
                        ISTRIALTEST = IsTrialtest == true ? 1 : 0,
                        ISADDMORERM = IsAddMoreRM == true ? 1 : 0,
                        ISADDRMSAMPLE = IsAddRMSample == true ? 1 : 0,
                        ISREQUESTRMSAMPLE = IsRequestRMSample == true ? 1 : 0
                    };

                    await approveMappingRepo.InsertAsync(newApproveMapping);

                    unitOfWork.Complete();

                    AlertSuccess = "Add Success.";
                    return Redirect($"/S2E/ApproveMapping");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }

    }
}
