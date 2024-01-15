using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Infrastructure.Entities;

namespace Web.UI.Domain.Interfaces
{
    public interface IImportRepository
    {
        Task<IEnumerable<ImportTemplateSavingTable>> GetImportTemplateSavingByYearAndTypeAsync(int year, string type);
        Task<int> DeleteImportTemplateSavingByYearAndTypeAsync(int year, string type);

    }
}
