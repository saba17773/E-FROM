﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Infrastructure.Entities;

namespace Web.UI.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<EmployeeTable> GetEmployeeByEmployeeId(string employeeId);
    }
}
