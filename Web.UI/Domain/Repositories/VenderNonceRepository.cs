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
    public class VenderNonceRepository : IVenderNonceRepository
    {
        private IDbTransaction _dbTransaction;

        public VenderNonceRepository(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public async Task<VenderNonce_TB> GetNonceByKey(string nonceKey)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<VenderNonce_TB>($@"
                SELECT * FROM TB_VenderNonce WHERE nonceKey = '{nonceKey}'
            ", null, _dbTransaction);
        }
    }
}
