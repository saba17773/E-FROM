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

namespace Web.UI.Pages.ApproveManage.VenderApproveMapping
{
    public class EditModel : PageModel
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
        
        [BindProperty]
        public string GroupDESC { get; set; }
        [Required]
        [BindProperty]
        public string DataAreaID { get; set; }

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
                        //Text = EmpAll.Where(e => e.EmployeeId == x.EmployeeId).Select(s => s.Name + " " + s.LastName).FirstOrDefault()
                    })
                    .ToList();
            }
        }

        public async Task GetData(int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var VendMapRepo = new GenericRepository<VenderApproveMapping_TB>(unitOfWork.Transaction);
                var VendMapAll = await VendMapRepo.GetAsync(id);
                if (VendMapAll == null)
                {
                    throw new Exception("Approve Mapping not found.");
                }

                ApproveMasterDesc = VendMapAll.APPROVEMASTERID;
                VenderGroupDesc = VendMapAll.APPROVEGROUPID;
                UserCreate = VendMapAll.CreateBy;
                GroupDESC = VendMapAll.DESCRIPTION;
                Step = VendMapAll.STEP;
                DataAreaID = VendMapAll.DATAAREAID;

                var AppMasterRepo = new GenericRepository<ApproveMasterTable>(unitOfWork.Transaction);
                var AppMasterAll = await AppMasterRepo.GetAllAsync();

                var VenderGroupRepo = new GenericRepository<VenderApproveGroupMaster_TB>(unitOfWork.Transaction);
                var VenderGroupAll = await VenderGroupRepo.GetAllAsync();

                var UserRepo = new GenericRepository<UserTable>(unitOfWork.Transaction);
                var UserAll = await UserRepo.GetAllAsync();
                var EmpRepo = new GenericRepository<EmployeeTable>(unitOfWork.Transaction);
                var EmpAll = await EmpRepo.GetAllAsync();

                var AppMaster = AppMasterAll.Where(x => x.IsActive == 1).ToList();
                var VenderGroup = VenderGroupAll.Where(x => x.ISACTIVE == 1).ToList();
                var User = UserAll.Where(x => x.IsActive == 1 && x.EmployeeId != null).ToList();

                ApproveMasterDescMaster = await GetApproveMasterDescMaster();
                VenderGroupDescMaster = await GetVenderGroupDescMaster();
                UserCreateMaster = await GetUserCreateMaster();

                unitOfWork.Complete();
            }
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                //await _authService.CanAccess(nameof(UserPermissionModel.EDIT_USER));
                await _authService.CanAccess(nameof(VenderPermissionModel.APPROVE_MAPPING_VENDER));
                await GetData(id);

                return Page();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
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
                    var VendMapRepo = new GenericRepository<VenderApproveMapping_TB>(unitOfWork.Transaction);
                    var VendMapAll = await VendMapRepo.GetAllAsync();
                    var VendMap = await VendMapRepo.GetAsync(id);
                    if (VendMap == null)
                    {
                        throw new Exception("Approve Mapping not found.");
                    }

                  
                    //var checkUserExists = VendMapAll
                    //  .Where(x =>
                    //    (x.CreateBy == UserCreate &&
                    //    x.APPROVEGROUPID == VenderGroupDesc &&
                    //    x.ID != VendMap.ID) || (x.CreateBy == UserCreate &&
                    //    x.APPROVEMASTERID == ApproveMasterDesc &&
                    //    x.ID != VendMap.ID) )
                    //  .FirstOrDefault();

                    //if (checkUserExists != null)
                    //{
                    //    throw new Exception("Employee already used in Approve Mapping.");
                    //}

                    VendMap.CreateBy = UserCreate;
                    VendMap.DESCRIPTION = GroupDESC;
                    VendMap.APPROVEMASTERID = ApproveMasterDesc;
                    VendMap.APPROVEGROUPID = VenderGroupDesc;
                    VendMap.STEP = Step;
                    VendMap.DATAAREAID = DataAreaID;


                    await VendMapRepo.UpdateAsync(VendMap);

                    unitOfWork.Complete();

                    AlertSuccess = "Edit Flow Success.";

                    return Redirect("/ApproveManage/VenderApproveMapping");
                }
            }
            catch (System.Exception ex)
            {
                AlertError = ex.Message;
                return Redirect("/ApproveManage/VenderApproveMapping/" + id + "/Edit");
            }
        }

    }
}
