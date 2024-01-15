using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity Get(int id);
        Task<TEntity> GetAsync(int id);
        IEnumerable<TEntity> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync();
        long Insert(TEntity entity);
        long InsertAll(IEnumerable<TEntity> entities);
        Task<long> InsertAsync(TEntity entity);
        Task<long> InsertAllAsync(IEnumerable<TEntity> entities);
        bool Update(TEntity entity);
        bool UpdateAll(IEnumerable<TEntity> entities);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> UpdateAllAsync(IEnumerable<TEntity> entities);
        bool Delete(TEntity entity);
        bool DeleteAll();
        Task<bool> DeleteAsync(TEntity entity);
        Task<bool> DeleteAllAsync();
    }
}
