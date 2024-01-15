using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Web.UI.DataAccess.Contexts;
using Web.UI.DataAccess.Entities;
using Web.UI.Services.Common.Dto;
using Web.UI.Services.CovidTracker.Interface;
using Web.UI.Services.DataTable.Dto;
using Web.UI.Services.DataTable.Interface;

namespace Web.UI.Services.CovidTracker.Handler
{
    public class VaccineService : IVaccineService
    {
        private IDtService _dtService;
        private NewContext _newContext;

        public VaccineService(
            IDtService dtService,
            NewContext newContext)
        {
            _dtService = dtService;
            _newContext = newContext;
        }

        public async Task<ResponseResult> AddVaccineInjection(Covid_VaccineInjection vaccineInjectionData)
        {
            using (var context = _newContext)
            {
                try
                {
                    if (vaccineInjectionData.Id == 0)
                    {
                        using (var trans = await context.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                var prevData = context.VaccineInjections
                                .Where(x => x.EmployeeId == vaccineInjectionData.EmployeeId)
                                .ToList();

                                foreach (var item in prevData)
                                {
                                    item.IsLatest = 0;
                                }

                                context.UpdateRange(prevData);
                                await context.SaveChangesAsync();

                                context.VaccineInjections.Add(vaccineInjectionData);
                                await context.SaveChangesAsync();

                                await trans.CommitAsync();
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }
                    else
                    {
                        var tempData = context.VaccineInjections.Find(vaccineInjectionData.Id);
                        tempData.EmployeeId = vaccineInjectionData.EmployeeId;
                        tempData.VaccineId = vaccineInjectionData.VaccineId;
                        tempData.VaccineDate = vaccineInjectionData.VaccineDate;
                        tempData.VaccineStatusId = vaccineInjectionData.VaccineStatusId;

                        context.Update(tempData);
                        await context.SaveChangesAsync();
                    }


                    return new ResponseResult
                    {
                        Result = true,
                        Message = "Save success",
                        Data = new
                        {
                            vaccineInjectionData
                        }
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseResult
                    {
                        Result = false,
                        Message = "Save failed",
                        Data = new
                        {
                            FormData = ex.Message
                        }
                    };
                }
            }
        }

        public async Task<ResponseResult> DeleteVaccineInjection(int rowId)
        {
            using (var context = _newContext)
            {
                try
                {
                    var vaccineInjection = await context.VaccineInjections.FindAsync(rowId);
                    context.Remove(vaccineInjection);
                    await context.SaveChangesAsync();

                    return new ResponseResult
                    {
                        Result = true,
                        Message = "Delete Success",
                        Data = new
                        {
                            vaccineInjection
                        }
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseResult
                    {
                        Result = false,
                        Message = ex.Message
                    };
                }

            }
        }

        public async Task<DtResult<Covid_Vaccine>> GetVaccine(HttpRequest request)
        {
            using (var context = _newContext)
            {
                var dtParameters = _dtService.GetParameters(request);

                var result = await context.Vaccine
                    .Where(x => EF.Functions.Like(x.Name, $"%{dtParameters.Search}%"))
                    .Take(50)
                    .Select(x => new Covid_Vaccine
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                    .ToListAsync();

                return _dtService.GetResult(result, dtParameters);
            }
        }

        public async Task<DtResult<Covid_VaccineInjection>> GetVaccineHistory(HttpRequest request, string employeeId)
        {
            using (var context = _newContext)
            {
                var dtParameters = _dtService.GetParameters(request);

                var employee = await context.VaccineInjections
                    .Where(x => x.EmployeeId == employeeId)
                    .OrderBy(x => x.VaccineDate)
                    .ToListAsync();

                return _dtService.GetResult(employee, dtParameters);
            }
        }

        public async Task<DtResult<Covid_VaccineInjection>> GetVaccineInjection(HttpRequest request)
        {
            using (var context = _newContext)
            {
                var dtParameters = _dtService.GetParameters(request);

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
                    .Take(50)
                    .OrderBy(dtParameters.OrderDir != "" ? $"{dtParameters.OrderColumn} {dtParameters.OrderDir}" : "Id desc")
                    .ToListAsync();

                return _dtService.GetResult(data, dtParameters);
            }
        }

        public async Task<List<Covid_VaccineStatus>> GetVaccineStatuses()
        {
            using (var context = _newContext)
            {
                var vaccineStatus = await context.VaccineStatuses
                    .Select(x => new Covid_VaccineStatus
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                    .ToListAsync();

                return vaccineStatus;
            }
        }

        public async Task<DtResult<HRMS_Employee>> GetEmployee(HttpRequest request)
        {
            using (var context = _newContext)
            {
                var dtParameters = _dtService.GetParameters(request);

                var result = await context.Employees
                    .Where(x =>
                        EF.Functions.Like(x.EmployeeId, $"%{dtParameters.Search}%") ||
                        EF.Functions.Like(x.Name, $"%{dtParameters.Search}%") ||
                        EF.Functions.Like(x.LastName, $"%{dtParameters.Search}%") ||
                        EF.Functions.Like(x.DepartmentName, $"%{dtParameters.Search}%") ||
                        EF.Functions.Like(x.DivisionName, $"%{dtParameters.Search}%") ||
                        EF.Functions.Like(x.Email, $"%{dtParameters.Search}%"))
                    .Take(50)
                    .Select(x => new HRMS_Employee
                    {
                        EmployeeId = x.EmployeeId,
                        Name = x.Name,
                        LastName = x.LastName,
                        DepartmentName = x.DepartmentName,
                        DivisionName = x.DivisionName
                    })
                    .ToListAsync();

                return _dtService.GetResult(result, dtParameters);
            }
        }
    }
}
