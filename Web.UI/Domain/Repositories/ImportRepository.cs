using Dapper;
using NPOI.OpenXmlFormats.Dml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Domain.Interfaces;
using Web.UI.Infrastructure.Entities;

namespace Web.UI.Domain.Repositories
{
  public class ImportRepository : IImportRepository
  {
    private IDbTransaction _dbTransaction;

    public ImportRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public async Task<int> DeleteImportTemplateSavingByYearAndTypeAsync(int year, string type)
    {
            // DELETE FROM TB_ImportTemplateSaving WHERE Year = @Year AND SubGroupType = @Type
      return await _dbTransaction.Connection.ExecuteAsync($@"
                DELETE FROM TB_ImportTemplateSaving WHERE Year = @Year AND SubGroupType = @Type
            ", new { @Year = year, @Type  = type}, _dbTransaction);
    }

    public async Task<IEnumerable<ImportTemplateSavingTable>> GetImportTemplateSavingByYearAndTypeAsync(int year, string type)
    {
      return await _dbTransaction.Connection.QueryAsync<ImportTemplateSavingTable>($@"
                SELECT * FROM TB_ImportTemplateSaving
                WHERE Year = @Year AND SubGroupType = @Type
            ", new { @Year = year, @Type = type }, _dbTransaction);
    }
  }
}
