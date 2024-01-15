using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Domain.Interfaces;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.ViewModels;

namespace Web.UI.Domain.Repositories
{
  public class MemoRepository : IMemoRepository
  {
    private IDbTransaction _dbTransaction;

    public MemoRepository(IDbTransaction dbTransaction)
    {
      _dbTransaction = dbTransaction;
    }

    public async Task<MemoCustomerTable> GetCustomerByCodeAsync(string custcode)
    {
      return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<MemoCustomerTable>($@"
          SELECT * FROM TB_MemoCustomer 
          WHERE CustomerCode = '{custcode}'
      ", null, _dbTransaction);
    }

    public async Task<IEnumerable<MemoAttachFileTable>> GetFileByCCIdAsync(int ccId)
    {
      return await _dbTransaction.Connection.QueryAsync<MemoAttachFileTable>($@"
          SELECT * FROM TB_MemoAttachFile 
          WHERE CCId = {ccId}
      ", null, _dbTransaction);
    }

    public async Task<MemoGridViewModel> GetCreateBy(int Id)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<MemoGridViewModel>($@"
                SELECT 
                E.Name,E.LastName
                FROM TB_Memo T
                LEFT JOIN TB_User U ON T.CreateBy = U.Id
                LEFT JOIN TB_Employee E ON U.EmployeeId = E.EmployeeId
                WHERE T.Id={Id}
            ", null, _dbTransaction);
        }

    public async Task<IEnumerable<MemoItemTable>> GetItemIdAsync(int Id)
    {
      return await _dbTransaction.Connection.QueryAsync<MemoItemTable>($@"
          SELECT * FROM TB_MemoItem 
          WHERE MemoId = {Id}
      ", null, _dbTransaction);
    }

    public async Task<IEnumerable<MemoItemTable>> GetItemByCCIdAsync(int ccId)
    {
      return await _dbTransaction.Connection.QueryAsync<MemoItemTable>($@"
          SELECT * FROM TB_MemoItem 
          WHERE MemoId = {ccId} AND Cancel = 1
      ", null, _dbTransaction);
    }

  }
}
