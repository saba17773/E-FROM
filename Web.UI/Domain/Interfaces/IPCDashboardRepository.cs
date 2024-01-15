using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models;

namespace Web.UI.Domain.Interfaces
{
    public interface IPCDashboardRepository
    {
        // KPI Delivery
        Task<IEnumerable<ImportTemplateDeliveryTable>> GetDataTemplateKPIDeliveryAsync(int year);
        Task<IEnumerable<ImportTemplateDeliveryTable>> GetDataTemplateKPIDeliveryAsync(int year, int month);
        Task<ImportTemplateDeliveryTable> GetDataTemplateKPIDeliveryAsync(int year, int month, string company);
        Task<ImportTemplateDeliveryTargetTable> GetDataTemplateKPIDeliveryTargetAsync(int year);

        // Stock Acc
        Task<IEnumerable<ImportTemplateStockAccTable>> GetDataTemplateStockAccAsync(int year);
        Task<IEnumerable<ImportTemplateStockAccTable>> GetDataTemplateStockAccAsync(int year, int month);
        Task<ImportTemplateStockAccTable> GetDataTemplateStockAccAsync(int year, int month, string company);
        Task<ImportTemplateStockAccTargetTable> GetDataTemplateStockAccTargetAsync(int year);
    }
}
