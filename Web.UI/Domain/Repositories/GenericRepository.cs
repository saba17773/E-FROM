using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Domain.Interfaces;

namespace Web.UI.Domain.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private IDbTransaction _dbTransaction;

        public GenericRepository(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public bool Delete(TEntity entity)
        {
            return _dbTransaction.Connection.Delete<TEntity>(entity, _dbTransaction);
        }

        public bool DeleteAll()
        {
            return _dbTransaction.Connection.DeleteAll<TEntity>(_dbTransaction);
        }

        public async Task<bool> DeleteAllAsync()
        {
            return await _dbTransaction.Connection.DeleteAllAsync<TEntity>(_dbTransaction);
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            return await _dbTransaction.Connection.DeleteAsync<TEntity>(entity, _dbTransaction);
        }

        public TEntity Get(int id)
        {
            return _dbTransaction.Connection.Get<TEntity>(id, _dbTransaction);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbTransaction.Connection.GetAll<TEntity>(_dbTransaction);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbTransaction.Connection.GetAllAsync<TEntity>(_dbTransaction);
        }

        public async Task<TEntity> GetAsync(int id)
        {
            return await _dbTransaction.Connection.GetAsync<TEntity>(id, _dbTransaction);
        }

        public long Insert(TEntity entity)
        {
            return _dbTransaction.Connection.Insert<TEntity>(entity, _dbTransaction);
        }

        public long InsertAll(IEnumerable<TEntity> entities)
        {
            return _dbTransaction.Connection.Insert<IEnumerable<TEntity>>(entities, _dbTransaction);
        }

        public async Task<long> InsertAllAsync(IEnumerable<TEntity> entities)
        {
            return await _dbTransaction.Connection.InsertAsync<IEnumerable<TEntity>>(entities, _dbTransaction);
        }

        public async Task<long> InsertAsync(TEntity entity)
        {
            return await _dbTransaction.Connection.InsertAsync<TEntity>(entity, _dbTransaction);
        }

        public bool Update(TEntity entity)
        {
            return _dbTransaction.Connection.Update<TEntity>(entity, _dbTransaction);
        }

        public bool UpdateAll(IEnumerable<TEntity> entities)
        {
            return _dbTransaction.Connection.Update<IEnumerable<TEntity>>(entities, _dbTransaction);
        }

        public async Task<bool> UpdateAllAsync(IEnumerable<TEntity> entities)
        {
            return await _dbTransaction.Connection.UpdateAsync<IEnumerable<TEntity>>(entities, _dbTransaction);
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            return await _dbTransaction.Connection.UpdateAsync<TEntity>(entity, _dbTransaction);
        }

        internal Task GetAsync(object cREATEBY)
        {
            throw new NotImplementedException();
        }
    }
}
