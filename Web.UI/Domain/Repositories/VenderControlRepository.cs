using Dapper;
using Dapper.Contrib.Extensions;
using Org.BouncyCastle.Ocsp;
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
    public class VenderControlRepository : IVenderControlRepository
    {
        private IDbTransaction _dbTransaction;

        public VenderControlRepository(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public async Task<IEnumerable<VenderApproveTrans_TB>> GetApproveTransByRequestID(int requestid, int approvemasterid)
        {
            return await _dbTransaction.Connection.QueryAsync<VenderApproveTrans_TB>($@"
                SELECT *
                FROM TB_VenderApproveTrans
                WHERE REQUESTID = '{requestid}'
                AND APPROVEMASTERID = {approvemasterid}
                AND ISCURRENTAPPROVE = 1
            ", null, _dbTransaction);
        }

        public async Task<VenderApproveTrans_TB> GetApproveTransByLevel(int requestid, int level, int approvemasterid)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<VenderApproveTrans_TB>($@"
                SELECT *
                FROM TB_VenderApproveTrans
                WHERE REQUESTID = '{requestid}'
                AND APPROVELEVEL = {level} 
                AND APPROVEMASTERID = {approvemasterid} 
                AND ISCURRENTAPPROVE = 1
            ", null, _dbTransaction);
        }

        public async Task<IEnumerable<VenderLogDoc_TB>> GetVenderLogDocByRequestID(int requestid)
        {
            return await _dbTransaction.Connection.QueryAsync<VenderLogDoc_TB>($@"
                SELECT *
                FROM TB_VenderLogDoc
                WHERE REQUESTID = '{requestid}' AND ISACTIVE = 1
            ", null, _dbTransaction);
        }

        public async Task<IEnumerable<VenderLogFile_TB>> GetVenderLogFileByRequestID(int requestid)
        {
            return await _dbTransaction.Connection.QueryAsync<VenderLogFile_TB>($@"
                SELECT *
                FROM TB_VenderLogFile
                WHERE REQUESTID = '{requestid}'
            ", null, _dbTransaction);
        }

        public async Task<IEnumerable<VenderApproveTrans_TB>> GetApproveTranIsCurrentByRequestID(int requestid,int approvemasterid)
        {
            return await _dbTransaction.Connection.QueryAsync<VenderApproveTrans_TB>($@"
                SELECT *
                FROM TB_VenderApproveTrans
                WHERE REQUESTID = '{requestid}' 
                AND APPROVEMASTERID = '{approvemasterid}'
            ", null, _dbTransaction);
        }
        
        public async Task<IEnumerable<VenderApproveTrans_TB>> GetTotalLevelInApproveTrans(int requestid, int approvemasterid)
        {
            return await _dbTransaction.Connection.QueryAsync<VenderApproveTrans_TB>($@"
                SELECT REQUESTID,APPROVEMASTERID,APPROVELEVEL
                FROM TB_VenderApproveTrans
                WHERE REQUESTID = '{requestid}'
                AND APPROVEMASTERID = {approvemasterid}
                AND ISCURRENTAPPROVE = 1
                GROUP BY REQUESTID,APPROVEMASTERID,APPROVELEVEL
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<ApproveFlow_TB>> GetApproveFlowByApproveMasterIdAsync(int id)
        {
            return await _dbTransaction.Connection.QueryAsync<ApproveFlow_TB>($@"
                SELECT * FROM TB_ApproveFlow
                WHERE ApproveMasterId = {id}
            ", null, _dbTransaction);
        }
    }
}
