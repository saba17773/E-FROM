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
    public class NonceRepository : INonceRepository
    {
        private IDbTransaction _dbTransaction;

        public NonceRepository(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public async Task<NonceTable> GetNonceByKey(string nonceKey)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<NonceTable>($@"
                SELECT * FROM TB_Nonce WHERE nonceKey = '{nonceKey}'
            ", null, _dbTransaction);
        }
    }
}
