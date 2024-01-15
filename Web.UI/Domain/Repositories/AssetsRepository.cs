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
    public class AssetsRepository : IAssetsRepository
    {
        private IDbTransaction _dbTransaction;

        public AssetsRepository(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public async Task<AssetsApproveMappingTable> GetApproveGroupId(int AssetsType, int CreateBy, string CompanyGroup)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<AssetsApproveMappingTable>($@"
                SELECT 
                    M.*
                FROM TB_FixAssetsApproveMapping M
                LEFT JOIN TB_Company C ON C.Id = M.CompanyId
                WHERE TypeProduct = '{AssetsType}' 
                AND CreateBy = '{CreateBy}'
                AND C.CompanyId ='{CompanyGroup}'
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<AssetsApproveTransTable>> GetApproveTransByCCId(int AssetsId)
        {
            return await _dbTransaction.Connection.QueryAsync<AssetsApproveTransTable>($@"
                SELECT *
                FROM TB_FixAssetsApproveTrans
                WHERE CCId = {AssetsId}
                AND Status = 'Approved By'
            ", null, _dbTransaction);
        }
        public async Task<AssetsApproveTransTable> GetApproveTransByLevel(int AssetsId, int level)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<AssetsApproveTransTable>($@"
                SELECT *
                FROM TB_FixAssetsApproveTrans
                WHERE CCId = {AssetsId}
                AND ApproveLevel = {level} 
            ", null, _dbTransaction);
        }
        public async Task<Company> GetCompany(string company)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<Company>($@"
                SELECT *
                FROM TB_Company
                WHERE CompanyId='{company}'
            ", null, _dbTransaction);
        }
        public async Task<AssetsUser> GetUser(int id)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<AssetsUser>($@"
                SELECT 
                    U.Id
                    ,U.EmployeeId
                    ,E.Name+' '+E.LastName [NameThai]
                    ,E.NameEng
                    ,E.DivisionName
                    ,E.DepartmentName
                    ,E.PositionName
                    ,C.CompanyName
                    ,E.Email
                FROM TB_User U
                LEFT JOIN TB_Employee E ON E.EmployeeId=U.EmployeeId
                LEFT JOIN TB_Company C ON C.CompanyId=E.Company
                LEFT JOIN TB_FixAssets F ON F.CreateBy=U.Id
                WHERE F.Id={id}
            ", null, _dbTransaction);
        }

        public async Task<IEnumerable<AssetsApproveTransTable>> GetApproveTransByReviewed(int AssetsId)
        {
            return await _dbTransaction.Connection.QueryAsync<AssetsApproveTransTable>($@"
                SELECT *
                FROM TB_FixAssetsApproveTrans
                WHERE CCId = {AssetsId}
                AND Status = 'Reviewed By'
            ", null, _dbTransaction);
        }

        public async Task<IEnumerable<AssetsApproveTransTable>> GetApproveTransByLV3(int AssetsId)
        {
            return await _dbTransaction.Connection.QueryAsync<AssetsApproveTransTable>($@"
                SELECT *
                FROM TB_FixAssetsApproveTrans
                WHERE CCId = {AssetsId}
                AND ApproveLevel = 3
            ", null, _dbTransaction);
        }

        public async Task<IEnumerable<AssetsLineTable>> GetAssetsLine(string AssetsLine)
        {
            return await _dbTransaction.Connection.QueryAsync<AssetsLineTable>($@"
                SELECT *
                FROM TB_FixAssetsLine
                WHERE AssetsNumber LIKE '%{AssetsLine}%'
            ", null, _dbTransaction);
        }

        public async Task<AssetsApproveTransTable> GetLevelTrans(int level, int masterid)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<AssetsApproveTransTable>($@"
                SELECT 
                    *
                FROM TB_FixAssetsApproveTrans
                WHERE ApproveMasterId = '{masterid}' 
                AND ApproveLevel = '{level}'
            ", null, _dbTransaction);
        }

    }
}
