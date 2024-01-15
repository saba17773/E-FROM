using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Domain.Interfaces;
using Web.UI.Domain.Repositories;

namespace Web.UI.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;

        public UnitOfWork(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            _dbConnection.Open();
            _dbTransaction = _dbConnection.BeginTransaction();

            CreditControl = new CreditControlRepository(_dbTransaction);
            Nonce = new NonceRepository(_dbTransaction);
            Employee = new EmployeeRepository(_dbTransaction);
            VenderControl = new VenderControlRepository(_dbTransaction);
            VenderNonce = new VenderNonceRepository(_dbTransaction);
            Import = new ImportRepository(_dbTransaction);
            PCDashboard = new PCDashboardRepository(_dbTransaction);
            Promotion = new PromotionRepository(_dbTransaction);
            S2EControl = new S2EControlRepository(_dbTransaction);
            Memo = new MemoRepository(_dbTransaction);
            Assets = new AssetsRepository(_dbTransaction);
        }

        public IDbTransaction Transaction
        {
            get { return _dbTransaction; }
        }

        public ICreditControlRepository CreditControl { get; private set; }
        public INonceRepository Nonce { get; private set; }
        public IEmployeeRepository Employee { get; private set; }
        public IImportRepository Import { get; private set; }
        public IPCDashboardRepository PCDashboard { get; private set; }
        public IVenderControlRepository VenderControl { get; private set; }
        public IVenderNonceRepository VenderNonce { get; private set; }
        public IPromotionRepository Promotion { get; private set; }
        public IS2EControlRepository S2EControl { get; private set; }
        public IMemoRepository Memo { get; private set; }
        public IAssetsRepository Assets { get; private set; }
        public void Complete()
        {
            try
            {
                _dbTransaction.Commit();
            }
            catch
            {
                _dbTransaction.Rollback();
                throw;
            }
            finally
            {
                _dbTransaction.Dispose();
            }
        }

        public void Dispose()
        {
            if (_dbTransaction != null)
            {
                _dbTransaction.Dispose();
                _dbTransaction = null;
            }

            if (_dbConnection != null)
            {
                _dbConnection.Dispose();
                _dbConnection = null;
            }
        }
    }
}
