using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;

namespace Web.UI.Pages.Permission
{
    public class IndexModel : PageModel
    {
        public CovidTrackerPermission CovidTracker_Permission { get; set; }


        [TempData]
        public string AlertSuccess { get; set; }

        [TempData]
        public string AlertError { get; set; }

        [Required]
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        [Required]
        [BindProperty]
        public string[] UserPermission { get; set; }

        [Required]
        [BindProperty]
        public string[] RolePermission { get; set; }

        [Required]
        [BindProperty]
        public string[] GeneralPermission { get; set; }

        [Required]
        [BindProperty]
        public string[] CreditControlPermission { get; set; }

        [Required]
        [BindProperty]
        public string[] VenderPermission { get; set; }

        [Required]
        [BindProperty]
        public string[] ImportPermission { get; set; }

        [Required]
        [BindProperty]
        public string[] ApproveMasterPermission { get; set; }

        [Required]
        [BindProperty]
        public string[] PromotionPermission { get; set; }
        
        [Required]
        [BindProperty]
        public string[] TicketPermission { get; set; }

        public List<PermissionTable> Permission { get; set; }
        [Required]
        [BindProperty]
        public string[] S2EPermission { get; set; }

        [Required]
        [BindProperty]
        public string[] MemoPermission { get; set; }

        [Required]
        [BindProperty]
        public string[] AssetsPermission { get; set; }

        [Required]
        [BindProperty]
        public string[] QueingPermission { get; set; }

        [Required]
        [BindProperty]
        public string[] CovidTrackerPermission { get; set; }

        private IDatabaseContext _databaseContext;
        private IAuthService _authService;

        public IndexModel(
          IDatabaseContext databaseContext,
          IAuthService authService)
        {
            _databaseContext = databaseContext;
            _authService = authService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            await GetData(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await GetData(id);

            try
            {
                using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
                {
                    var permissionRepo = new GenericRepository<PermissionTable>(unitOfWork.Transaction);

                    var permissionAll = await permissionRepo.GetAllAsync();

                    foreach (var item in GeneralPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == id && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = id,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.GENERAL)
                            });
                        }
                    }

                    foreach (var item in UserPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == id && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = id,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.USER)
                            });
                        }
                    }

                    foreach (var item in RolePermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.ROLE)
                            });
                        }
                    }

                    foreach (var item in CreditControlPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.CREDIT_CONTROL)
                            });
                        }
                    }

                    foreach (var item in VenderPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.VENDER)
                            });
                        }
                    }

                    foreach (var item in ImportPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.IMPORT)
                            });
                        }
                    }

                    foreach (var item in ApproveMasterPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.APPROVEMASTER)
                            });
                        }
                    }

                    foreach (var item in PromotionPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.PROMOTION)
                            });
                        }
                    }

                    foreach (var item in TicketPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.TICKET)
                            });
                        }
                    }

                    foreach (var item in S2EPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.S2E)
                            });
                        }
                    }

                    foreach (var item in MemoPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.MEMO)
                            });
                        }
                    }

                    foreach (var item in AssetsPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.ASSETS)
                            });
                        }
                    }

                    foreach (var item in QueingPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.QUEING)
                            });
                        }
                    }

                    foreach (var item in CovidTrackerPermission)
                    {
                        if (!permissionAll.Any(x => x.RoleId == RoleId && x.CapabilityId == item))
                        {
                            await permissionRepo.InsertAsync(new PermissionTable
                            {
                                RoleId = RoleId,
                                CapabilityId = item,
                                PermissionType = nameof(PermissionTypeModel.COVID_TRACKER)
                            });
                        }
                    }

                    foreach (var item in permissionAll)
                    {
                        if (!GeneralPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.GENERAL) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!UserPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.USER) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!RolePermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.ROLE) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!CreditControlPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.CREDIT_CONTROL) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!VenderPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.VENDER) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!ApproveMasterPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.APPROVEMASTER) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!ImportPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.IMPORT) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!PromotionPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.PROMOTION) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!TicketPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.TICKET) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!S2EPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.S2E) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!MemoPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.MEMO) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!AssetsPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.ASSETS) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                         if (!QueingPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.QUEING) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }

                        if (!CovidTrackerPermission.Any(x => x == item.CapabilityId) && item.PermissionType == nameof(PermissionTypeModel.COVID_TRACKER) && item.RoleId == id)
                        {
                            var permissionId = await permissionRepo.GetAsync(item.Id);
                            await permissionRepo.DeleteAsync(permissionId);
                        }
                    }

                    unitOfWork.Complete();

                    AlertSuccess = "Update Permission Success.";

                    return Redirect($"/Role/{id}/Permission");
                }
            }
            catch (Exception ex)
            {
                AlertError = ex.Message;
                return Redirect($"/Role/{id}/Permission");
            }
        }

        public async Task GetData(int id)
        {
            using (var unitOfWork = new UnitOfWork(_databaseContext.GetConnection()))
            {
                var roleRepo = new GenericRepository<RoleTable>(unitOfWork.Transaction);
                var permissionRepo = new GenericRepository<PermissionTable>(unitOfWork.Transaction);

                var permissionAll = await permissionRepo.GetAllAsync();

                Permission = permissionAll.Where(x => x.RoleId == id).ToList();

                var role = await roleRepo.GetAsync(id);
                RoleName = role.Description;
                RoleId = role.Id;

                unitOfWork.Complete();
            }
        }
    }
}