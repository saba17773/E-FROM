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
    public class PromotionRepository : IPromotionRepository
    {
        private IDbTransaction _dbTransaction;

        public PromotionRepository(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public async Task<PromotionApproveMappingTable> GetApproveGroupId(string PromotionType, int CreateBy)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<PromotionApproveMappingTable>($@"
                SELECT * FROM TB_PromotionApproveMapping WHERE TypeProduct = '{PromotionType}' AND CreateBy = '{CreateBy}'
            ", null, _dbTransaction);
        }

        public async Task<IEnumerable<PromotionApproveTransTable>> GetApproveTransByCCId(int PromotionId)
        {
            return await _dbTransaction.Connection.QueryAsync<PromotionApproveTransTable>($@"
                SELECT *
                FROM TB_PromotionApproveTrans
                WHERE CCId = {PromotionId}
            ", null, _dbTransaction);
        }

        public async Task<PromotionApproveTransTable> GetApproveTransByLevel(int PromotionId, int level)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<PromotionApproveTransTable>($@"
                SELECT *
                FROM TB_PromotionApproveTrans
                WHERE CCId = {PromotionId}
                AND ApproveLevel = {level} 
            ", null, _dbTransaction);
        }

        public async Task<IEnumerable<PromotionAttachFileTable>> GetFileByCCIdAsync(int ccId, string requestType)
        {
            return await _dbTransaction.Connection.QueryAsync<PromotionAttachFileTable>($@"
                SELECT *
                FROM TB_PromotionAttachFile CF 
                WHERE CF.CCId = {ccId}
                AND CF.CCType = '{requestType}'
            ", null, _dbTransaction);
        }

        public async Task<IEnumerable<ApproveFlowTable>> GetApproveTransFlowByCCId(int ccId)
        {
            return await _dbTransaction.Connection.QueryAsync<ApproveFlowTable>($@"
                SELECT 
                AF.*
                FROM TB_PromotionApproveTrans AT
                LEFT JOIN TB_ApproveFlow AF ON AT.ApproveMasterId = AF.ApproveMasterId AND AT.ApproveLevel = AF.ApproveLevel
                WHERE AT.CCId= {ccId}
            ", null, _dbTransaction);
        }

        public async Task<PromotionApproveTransGridModel> GetCreateBy(int PromotionId)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<PromotionApproveTransGridModel>($@"
                SELECT 
                T.CreateBy,U.Email,E.Name,E.LastName
                FROM TB_Promotion T
                LEFT JOIN TB_User U ON T.CreateBy = U.Id
                LEFT JOIN TB_Employee E ON U.EmployeeId = E.EmployeeId
                WHERE T.Id={PromotionId}
            ", null, _dbTransaction);
        }

    }
}
