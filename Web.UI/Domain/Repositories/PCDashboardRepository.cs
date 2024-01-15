using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Domain.Interfaces;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;

namespace Web.UI.Domain.Repositories
{
    public class PCDashboardRepository : IPCDashboardRepository
    {
        private IDbTransaction _dbTransaction;

        public PCDashboardRepository(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public async Task<IEnumerable<ImportTemplateDeliveryTable>> GetDataTemplateKPIDeliveryAsync(int year)
        {
            return await _dbTransaction.Connection.QueryAsync<ImportTemplateDeliveryTable>($@"SELECT * FROM TB_ImportTemplateDelivery WHERE Year = {year}", null, _dbTransaction);
        }

        public async Task<ImportTemplateDeliveryTable> GetDataTemplateKPIDeliveryAsync(int year, int month, string company)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<ImportTemplateDeliveryTable>($@"SELECT * FROM TB_ImportTemplateDelivery WHERE Year = {year} AND Month = {month} AND Company = '{company}'", null, _dbTransaction);
        }

        public async Task<IEnumerable<ImportTemplateDeliveryTable>> GetDataTemplateKPIDeliveryAsync(int year, int month)
        {
            return await _dbTransaction.Connection.QueryAsync<ImportTemplateDeliveryTable>($@"SELECT * FROM TB_ImportTemplateDelivery WHERE Year = {year} AND Month = {month}", null, _dbTransaction);
        }

        public async Task<ImportTemplateDeliveryTargetTable> GetDataTemplateKPIDeliveryTargetAsync(int year)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<ImportTemplateDeliveryTargetTable>($@"SELECT * FROM TB_ImportTemplateDeliveryTarget WHERE Year = {year}", null, _dbTransaction);
        }

        public async Task<IEnumerable<ImportTemplateStockAccTable>> GetDataTemplateStockAccAsync(int year)
        {
            return await _dbTransaction.Connection.QueryAsync<ImportTemplateStockAccTable>($@"SELECT * FROM TB_ImportTemplateStockAcc WHERE Year = {year}", null, _dbTransaction);
        }

        public async Task<IEnumerable<ImportTemplateStockAccTable>> GetDataTemplateStockAccAsync(int year, int month)
        {
            return await _dbTransaction.Connection.QueryAsync<ImportTemplateStockAccTable>($@"SELECT * FROM TB_ImportTemplateStockAcc WHERE Year = {year} AND Month = {month}", null, _dbTransaction);
        }

        public async Task<ImportTemplateStockAccTable> GetDataTemplateStockAccAsync(int year, int month, string company)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<ImportTemplateStockAccTable>($@"SELECT * FROM TB_ImportTemplateStockAcc WHERE Year = {year} AND Month = {month} AND Company = '{company}'", null, _dbTransaction);
        }

        public async Task<ImportTemplateStockAccTargetTable> GetDataTemplateStockAccTargetAsync(int year)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<ImportTemplateStockAccTargetTable>($@"SELECT * FROM TB_ImportTemplateStockAccTarget WHERE Year = {year}", null, _dbTransaction);
        }
    }
}
