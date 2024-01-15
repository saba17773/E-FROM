using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Domain.Interfaces;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.S2E;

namespace Web.UI.Domain.Repositories
{
    public class S2EControlRepository : IS2EControlRepository
    {
        private IDbTransaction _dbTransaction;
        public S2EControlRepository(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }
        public async Task<IEnumerable<S2ENewRequestApproveTrans_TB>> GetApproveTransByRequestID(int requestid, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2ENewRequestApproveTrans_TB>($@"
                SELECT *
                FROM TB_S2ENewRequestApproveTrans
                WHERE REQUESTID = '{requestid}'
                AND APPROVEMASTERID = {approvemasterid}
                AND ISCURRENTAPPROVE = 1
                AND APPROVEGROUPID = {approveGroupID}
            ", null, _dbTransaction);
        }
        public async Task<S2ENewRequestNonce_TB> GetNewRequestNonceByKey(string nonceKey)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<S2ENewRequestNonce_TB>($@"
                SELECT * FROM TB_S2ENewRequestNonce WHERE NONCEKEY = '{nonceKey}'
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<ApproveFlowTable>> GetApproveFlowByApproveMasterIdAsync(int id)
        {
            return await _dbTransaction.Connection.QueryAsync<ApproveFlowTable>($@"
                SELECT * FROM TB_ApproveFlow
                WHERE ApproveMasterId = {id}
            ", null, _dbTransaction);
        }
        public async Task<S2ERMAssessmentNonce_TB> GetNonceRMAssessmentByKey(string nonceKey)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<S2ERMAssessmentNonce_TB>($@"
                SELECT * FROM TB_S2ERMAssessmentNonce WHERE NONCEKEY = '{nonceKey}'
            ", null, _dbTransaction);
        }
        public async Task<S2ELABTestNonce_TB> GetNonceLABTestByKey(string nonceKey)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<S2ELABTestNonce_TB>($@"
                SELECT * FROM TB_S2ELABTestNonce WHERE NONCEKEY = '{nonceKey}'
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2ERMAssessmentApproveTrans_TB>> GetApproveTransByAssessmentID(int AssessmentID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2ERMAssessmentApproveTrans_TB>($@"
                SELECT *
                    FROM TB_S2ERMAssessmentApproveTrans
                    WHERE ASSESSMENTID = '{AssessmentID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2ELABTestApproveTrans_TB>> GetApproveTransByLABID(int LabID, int LabLineID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2ELABTestApproveTrans_TB>($@"
                 SELECT *
                    FROM TB_S2ELABTestApproveTrans
                    WHERE LABID = '{LabID}'
                    AND LABLINEID = '{LabLineID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2EPurchaseSampleLogsSendEmail_TB>> GetLogsSendEmailByPCSampleID(int PCSampleID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2EPurchaseSampleLogsSendEmail_TB>($@"
                SELECT *
                FROM TB_S2EPurchaseSampleLogsSendEmail
                WHERE PCSAMPLEID = '{PCSampleID}'
                AND APPROVEMASTERID = {approvemasterid}
                AND APPROVEGROUPID = {approveGroupID}
                AND ISLASTSENDEMAIL = 1
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2EAddRawMaterialApproveTrans_TB>> GetApproveTransByAddRMID(int AddRMID, int approvemasterid, int approveGroupID,int AddRMLineID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2EAddRawMaterialApproveTrans_TB>($@"
                 SELECT *
                    FROM TB_S2EAddRawMaterialApproveTrans
                    WHERE ADDRMID = '{AddRMID}'
                    AND ADDRMLINEID = '{AddRMLineID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
            ", null, _dbTransaction);
        }
        public async Task<S2EAddRawMaterialNonce_TB> GetNonceAddRawMaterialByKey(string nonceKey)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<S2EAddRawMaterialNonce_TB>($@"
                SELECT * FROM TB_S2EAddRawMaterialNonce WHERE NONCEKEY = '{nonceKey}'
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2EMaterialRequestApproveTrans_TB>> GetApproveTransByMaterialReqID(int RMREQID, int RMREQLineID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2EMaterialRequestApproveTrans_TB>($@"
                  SELECT *
                    FROM TB_S2EMaterialRequestApproveTrans
                    WHERE RMREQID = '{RMREQID}'
                    AND RMREQLINEID = '{RMREQLineID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
            ", null, _dbTransaction);
        }
        public async Task<S2EMaterialRequestNonce_TB> GetNonceMaterialRequestByKey(string nonceKey)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<S2EMaterialRequestNonce_TB>($@"
                SELECT * FROM TB_S2EMaterialRequestNonce WHERE NONCEKEY = '{nonceKey}'
            ", null, _dbTransaction);
        }
        public async Task<S2ETrialTestNonce_TB> GetNonceTrialTestByKey(string nonceKey)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<S2ETrialTestNonce_TB>($@"
                SELECT * FROM TB_S2ETrialTestNonce WHERE NONCEKEY = '{nonceKey}'
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2ETrialTestApproveTrans_TB>> GetApproveTransByTrialID(int TrialID, int TrialLineID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2ETrialTestApproveTrans_TB>($@"
                SELECT *
                    FROM TB_S2ETrialTestApproveTrans
                    WHERE TRIALID = '{TrialID}'
                    AND TRIALLINEID = '{TrialLineID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2ENewRequestApproveTrans_TB>> GetApproveTransByRequestIDAllLevel(int requestid, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2ENewRequestApproveTrans_TB>($@"
                SELECT REQUESTID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
                FROM TB_S2ENewRequestApproveTrans
                WHERE REQUESTID = '{requestid}'
                AND APPROVEMASTERID = {approvemasterid}
                AND ISCURRENTAPPROVE = 1
                AND APPROVEGROUPID = {approveGroupID}
                GROUP BY REQUESTID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2ERMAssessmentApproveTrans_TB>> GetApproveTransByAssessmentIDAllLevel(int AssessmentID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2ERMAssessmentApproveTrans_TB>($@"
                SELECT ASSESSMENTID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
                    FROM TB_S2ERMAssessmentApproveTrans
                    WHERE ASSESSMENTID = '{AssessmentID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
                GROUP BY ASSESSMENTID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2ELABTestApproveTrans_TB>> GetApproveTransByLABIDAllLevel(int LabID, int LabLineID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2ELABTestApproveTrans_TB>($@"
                 SELECT LABID,LABLINEID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
                    FROM TB_S2ELABTestApproveTrans
                    WHERE LABID = '{LabID}'
                    AND LABLINEID = '{LabLineID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
                GROUP BY LABID,LABLINEID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2EAddRawMaterialApproveTrans_TB>> GetApproveTransByAddRMIDAllLevel(int AddRMID, int approvemasterid, int approveGroupID, int AddRMLineID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2EAddRawMaterialApproveTrans_TB>($@"
                 SELECT ADDRMID,ADDRMLINEID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
                    FROM TB_S2EAddRawMaterialApproveTrans
                    WHERE ADDRMID = '{AddRMID}'
                    AND ADDRMLINEID = '{AddRMLineID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
                    GROUP BY ADDRMID,ADDRMLINEID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2EMaterialRequestApproveTrans_TB>> GetApproveTransByMaterialReqIDAllLevel (int RMREQID, int RMREQLineID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2EMaterialRequestApproveTrans_TB>($@"
                  SELECT RMREQID,RMREQLINEID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
                    FROM TB_S2EMaterialRequestApproveTrans
                    WHERE RMREQID = '{RMREQID}'
                    AND RMREQLINEID = '{RMREQLineID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
                    GROUP BY RMREQID,RMREQLINEID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2ETrialTestApproveTrans_TB>> GetApproveTransByTrialIDAllLevel(int TrialID, int TrialLineID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2ETrialTestApproveTrans_TB>($@"
                SELECT TRIALID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
                    FROM TB_S2ETrialTestApproveTrans
                    WHERE TRIALID = '{TrialID}'
                    AND TRIALLINEID = '{TrialLineID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
                GROUP BY TRIALID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2EAddRawMaterialSampleApproveTrans_TB>> GetApproveTransByAddRMSampleID(int AddRMSampleID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2EAddRawMaterialSampleApproveTrans_TB>($@"
                 SELECT *
                    FROM TB_S2EAddRawMaterialSampleApproveTrans
                    WHERE ADDRMSAMPLEID = '{AddRMSampleID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
            ", null, _dbTransaction);
        }
        public async Task<S2EAddRawMaterialSampleNonce_TB> GetNonceAddRawMaterialSampleByKey(string nonceKey)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<S2EAddRawMaterialSampleNonce_TB>($@"
                SELECT * FROM TB_S2EAddRawMaterialSampleNonce WHERE NONCEKEY = '{nonceKey}'
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2EMaterialRequestSampleApproveTrans_TB>> GetApproveTransByRMReqSampleID(int RMReqSamID, int RMReqSamLineID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2EMaterialRequestSampleApproveTrans_TB>($@"
                  SELECT *
                    FROM TB_S2EMaterialRequestSampleApproveTrans
                    WHERE RMREQSAMID = '{RMReqSamID}'
                    AND RMREQSAMLINEID = '{RMReqSamLineID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
            ", null, _dbTransaction);
        }
        public async Task<S2EMaterialRequestSampleNonce_TB> GetNonceMaterialRequestSampleByKey(string nonceKey)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<S2EMaterialRequestSampleNonce_TB>($@"
                SELECT * FROM TB_S2EMaterialRequestSampleNonce WHERE NONCEKEY = '{nonceKey}'
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2EMaterialRequestSampleApproveTrans_TB>> GetApproveTransByRMReqSampleIDAllLevel(int RMReqSamID, int RMReqSamLineID, int approvemasterid, int approveGroupID)
        {
            return await _dbTransaction.Connection.QueryAsync<S2EMaterialRequestSampleApproveTrans_TB>($@"
                  SELECT RMREQSAMID,RMREQSAMLINEID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
                    FROM TB_S2EMaterialRequestSampleApproveTrans
                    WHERE RMREQSAMID = '{RMReqSamID}'
                    AND RMREQSAMLINEID = '{RMReqSamLineID}'
                    AND APPROVEMASTERID = {approvemasterid}
                    AND ISCURRENTAPPROVE = 1
                    AND APPROVEGROUPID = {approveGroupID}
                    GROUP BY RMREQSAMID,RMREQSAMLINEID,APPROVEMASTERID,APPROVEGROUPID,APPROVELEVEL
            ", null, _dbTransaction);
        }
        public async Task<IEnumerable<S2ELogFileEditApproveTrans_TB>> GetApproveTransLogFileEdit(int groupid, int headid, int approvemasterid)
        {
            return await _dbTransaction.Connection.QueryAsync<S2ELogFileEditApproveTrans_TB>($@"
                 SELECT *
                        FROM TB_S2ELogFileEditApproveTrans
                        WHERE LOGFILEHEADID = {headid}
                        AND APPROVEGROUPID = {groupid}
                        AND APPROVEMASTERID = {approvemasterid}
                        AND ISCURRENTAPPROVE = 1
            ", null, _dbTransaction);
        }
        public async Task<S2ELogFileEditNonce_TB> GetLogFileNonceByKey(string nonceKey)
        {
            return await _dbTransaction.Connection.QueryFirstOrDefaultAsync<S2ELogFileEditNonce_TB>($@"
                SELECT * FROM TB_S2ELogFileEditNonce WHERE NONCEKEY = '{nonceKey}'
            ", null, _dbTransaction);
        }
    }
}
