using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Web.UI.Infrastructure.Models;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Interfaces;

namespace Web.UI.Pages.ApproveManage.VenderApproveMapping
{
    public class AddModel : PageModel
    {
        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [Required]
        [BindProperty]
        public int ApproveMasterDesc { get; set; }
        [Required]
        public List<SelectListItem> ApproveMasterDescMaster { get; set; }

        [Required]
        [BindProperty]
        public int VenderGroupDesc { get; set; }
        [Required]
        public List<SelectListItem> VenderGroupDescMaster { get; set; }

        [Required]
        [BindProperty]
        public int UserCreate { get; set; }
        [Required]
        public List<SelectListItem> UserCreateMaster { get; set; }
        [Required]
        [BindProperty]
        public int Step { get; set; }
        [Required]
        [BindProperty]
        public string DataAreaID { get; set; }

        [BindProperty]
        public string GroupDESC { get; set; }

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
                await _authService.CanAccess(nameof(VenderPermissionModel.APPROVE_MAPPING_VENDER));

                ApproveMasterDescMaster = await GetApproveMasterDescMaster();
                VenderGroupDescMaster = await GetVenderGroupDescMaster();
                UserCreateMaster = await GetUserCreateMaster();
                return Page();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<List<SelectListItem>> GetApproveMasterDescMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var AppMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);

                var AppMasterAll = await AppMasterRepo.GetAllAsync();

                return AppMasterAll
                    .Where(x => x.IsActive == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.GroupDescription
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetVenderGroupDescMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var VenderGroupRepo = new GenericRepository<VenderApproveGroupMaster_TB>(unitOfWork.Transaction);

                var VenderGroupAll = await VenderGroupRepo.GetAllAsync();

                return VenderGroupAll
                    .Where(x => x.ISACTIVE == 1)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.DESCRIPTION
                    })
                    .ToList();
            }
        }
        public async Task<List<SelectListItem>> GetUserCreateMaster()
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                var UserAll = await UserRepo.GetAllAsync();
                var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                var EmpAll = await EmpRepo.GetAllAsync();
                return UserAll
                    .Where(x => x.IsActive == 1 && x.EmployeeId != null)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Username
                        //Text = EmpAll.Where(e=>e.EmployeeId == x.EmployeeId).Select(s=>s.Name + " " +s.LastName).FirstOrDefault()
                    })
                    .ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                /*await _authService.CanAccess(nameof(UserPermissionModel.ADD_USER));*/

                if (!ModelState.IsValid)
                {
                    ApproveMasterDescMaster = await GetApproveMasterDescMaster();
                    VenderGroupDescMaster = await GetVenderGroupDescMaster();
                    UserCreateMaster = await GetUserCreateMaster();
                    return Page();
                }

                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var vendMapRepo = new GenericRepository<VenderApproveMapping_TB>(unitOfWork.Transaction);
                    var vendMapAll = await vendMapRepo.GetAllAsync();
                   // var checkData = vendMapAll.Where(x => x.CreateBy == UserCreate &&  x.APPROVEGROUPID == VenderGroupDesc).FirstOrDefault();
                   // var checkData2 = vendMapAll.Where(x => x.CreateBy == UserCreate && x.APPROVEMASTERID == ApproveMasterDesc ).FirstOrDefault();
                   //// var checkData3 = vendMapAll.Where(x => x.CreateBy == UserCreate && x.STEP == Step).FirstOrDefault();
                   // //var checkData2 = vendMapAll.Where(x => x.CreateBy == UserCreate && x.APPROVEMASTERID == ApproveMasterDesc && x.APPROVEGROUPID == VenderGroupDesc).FirstOrDefault();
                   // if (checkData != null || checkData2 != null )
                   // {
                   //     throw new Exception("Employee already used in Approve Mapping");
                   // }

                    await vendMapRepo.InsertAsync(new VenderApproveMapping_TB
                    {
                        CreateBy = UserCreate,
                        DESCRIPTION = GroupDESC,
                        APPROVEMASTERID = ApproveMasterDesc,
                        APPROVEGROUPID = VenderGroupDesc,
                        STEP = Step,
                        DATAAREAID = DataAreaID

                    });

                    unitOfWork.Complete();

                    AlertSuccess = "Add Flow Success.";

                    return Redirect("/ApproveManage/VenderApproveMapping");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/ApproveManage/VenderApproveMapping/Add");
            }
        }
    }
}
