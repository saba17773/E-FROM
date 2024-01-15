using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Domain.Interfaces;
using Web.UI.Infrastructure.Entities;

namespace Web.UI.Domain.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private IDbTransaction _dbTransaction;

        public EmployeeRepository(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public async Task<EmployeeTable> GetEmployeeByEmployeeId(string employeeId)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<EmployeeTable>($@"
                SELECT * FROM TB_Employee
                WHERE EmployeeId = '{employeeId}'
            ", null, _dbTransaction);
        }
    }
}
