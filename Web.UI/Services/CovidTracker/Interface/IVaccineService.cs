using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.DataAccess.Entities;
using Web.UI.Services.Common.Dto;
using Web.UI.Services.DataTable.Dto;

namespace Web.UI.Services.CovidTracker.Interface
{
    public interface IVaccineService
    {
        Task<List<Covid_VaccineStatus>> GetVaccineStatuses();
        Task<DtResult<Covid_Vaccine>> GetVaccine(HttpRequest request);
        Task<DtResult<Covid_VaccineInjection>> GetVaccineInjection(HttpRequest request);
        Task<ResponseResult> AddVaccineInjection(Covid_VaccineInjection vaccineInjectionData);
        Task<ResponseResult> DeleteVaccineInjection(int rowId);
        Task<DtResult<Covid_VaccineInjection>> GetVaccineHistory(HttpRequest request, string employeeId);
        Task<DtResult<HRMS_Employee>> GetEmployee(HttpRequest request);
    }
}
