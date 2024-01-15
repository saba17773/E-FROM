using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.UI.DataAccess.Contexts;
using Web.UI.DataAccess.Entities;
using Web.UI.Infrastructure.Models;
using Web.UI.Interfaces;
using Web.UI.Services.CovidTracker;
using Web.UI.Services.CovidTracker.Interface;
using Web.UI.Services.DataTable.Interface;

namespace Web.UI.Pages.CovidTracker
{
    [Authorize]
    public class VaccineInjectionModel : PageModel
    {
        public List<Covid_VaccineStatus> VaccineStatuses { get; set; }
        public List<SelectListItem> VaccineStatusesSelectListItem { get; set; }
        public List<SelectListItem> VaccineSelectListItem { get; set; }

        [BindProperty]
        public Covid_VaccineInjection Covid_VaccineInjection { get; set; } = new Covid_VaccineInjection();

        private IVaccineService _vaccineService;
        private IAuthService _authService;
        private NewContext _newContext;
        private IDtService _dtService;

        public VaccineInjectionModel(
            IVaccineService vaccineService,
            IAuthService authService,
            NewContext newContext,
            IDtService dtService)
        {
            _vaccineService = vaccineService;
            _authService = authService;
            _newContext = newContext;
            _dtService = dtService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.CanAccess(nameof(CovidTrackerPermission.PROJECT_COVID_TRACKER));

                VaccineStatuses = await _vaccineService.GetVaccineStatuses();

                VaccineStatusesSelectListItem = VaccineStatuses
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    })
                    .ToList();

                var vaccineData = await _vaccineService.GetVaccine(Request);
                VaccineSelectListItem = vaccineData.Data
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    })
                    .ToList();

                return Page();
            }
            catch (Exception)
            {
                return new UnauthorizedResult();
            }
            
        }

        public async Task<IActionResult> OnGetEmployeeGridAsync()
        {
            var employee = await _vaccineService.GetEmployee(Request);

            return new JsonResult(employee);
        }

        public async Task<IActionResult> OnPostVaccineInjectionGridAsync()
        {
            using (var context = _newContext)
            {
                var dtParameters = _dtService.GetParameters(Request);

                var data = await context.VaccineInjections
                    .Include(x => x.Employee)
                    .Include(x => x.Vaccine)
                    .Include(x => x.VaccineStatus)
                    .Where(x =>
                        EF.Functions.Like(x.Employee.EmployeeId, $"%{dtParameters.Search}%") ||
                        EF.Functions.Like(x.Employee.Name, $"%{dtParameters.Search}%") ||
                        EF.Functions.Like(x.Vaccine.Name, $"%{dtParameters.Search}%") ||
                        EF.Functions.Like(x.VaccineStatus.Name, $"%{dtParameters.Search}%") ||
                        EF.Functions.Like(x.Employee.LastName, $"%{dtParameters.Search}%"))
                    .Select(x => new
                    {
                        Id = x.Id,
                        EmployeeId = x.EmployeeId,
                        Employee = new
                        {
                            x.Employee.Name,
                            x.Employee.LastName,
                            x.Employee.Company,
                            x.Employee.DepartmentName,
                            x.Employee.DivisionName
                        },
                        Vaccine = new { 
                            x.Vaccine.Id,
                            x.Vaccine.Name
                        },
                        Status = new {
                            x.VaccineStatus.Id,
                            x.VaccineStatus.Name
                        },
                        VaccineDate = x.VaccineDate
                    })
                    .OrderBy(dtParameters.OrderDir != "" ? $"{dtParameters.OrderColumn} {dtParameters.OrderDir}" : "Id desc")
                    .Take(50)
                    .ToListAsync();

                return new JsonResult(_dtService.GetResult(data, dtParameters));
            }
        }

        public async Task<IActionResult> OnPostSaveVaccineDataAsync()
        {
            var result = await _vaccineService.AddVaccineInjection(Covid_VaccineInjection);
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int rowId)
        {
            var result = await _vaccineService.DeleteVaccineInjection(rowId);
            return new JsonResult(result);
        }
    }
}
