using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Entities.S2E;

namespace Web.UI.Domain.Interfaces
{
    public interface IS2EControlRepository
    {
        Task<IEnumerable<S2ENewRequestApproveTrans_TB>> GetApproveTransByRequestID(int requestid, int approvemasterid, int approveGroupID);
        Task<S2ENewRequestNonce_TB> GetNewRequestNonceByKey(string nonceKey);
        Task<IEnumerable<ApproveFlowTable>> GetApproveFlowByApproveMasterIdAsync(int id);
        Task<S2ERMAssessmentNonce_TB> GetNonceRMAssessmentByKey(string nonceKey);
        Task<S2ELABTestNonce_TB> GetNonceLABTestByKey(string nonceKey);
        Task<IEnumerable<S2ERMAssessmentApproveTrans_TB>> GetApproveTransByAssessmentID(int AssessmentID, int approvemasterid, int approveGroupID);
        Task<IEnumerable<S2ELABTestApproveTrans_TB>> GetApproveTransByLABID(int LabID, int LabLineID, int approvemasterid, int approveGroupID);
        Task<IEnumerable<S2EPurchaseSampleLogsSendEmail_TB>> GetLogsSendEmailByPCSampleID(int PCSampleID, int approvemasterid, int ApproveGroupID);
        Task<IEnumerable<S2EAddRawMaterialApproveTrans_TB>> GetApproveTransByAddRMID(int AddRMID, int approvemasterid, int approveGroupID,int AddRMLineID);
        Task<S2EAddRawMaterialNonce_TB> GetNonceAddRawMaterialByKey(string nonceKey);
        Task<IEnumerable<S2EMaterialRequestApproveTrans_TB>> GetApproveTransByMaterialReqID(int RMREQID,int RMREQLineID, int approvemasterid, int approveGroupID);
        Task<S2EMaterialRequestNonce_TB> GetNonceMaterialRequestByKey(string nonceKey);
        Task<IEnumerable<S2ETrialTestApproveTrans_TB>> GetApproveTransByTrialID(int TrialID, int TrialLineID, int approvemasterid, int approveGroupID);
        Task<S2ETrialTestNonce_TB> GetNonceTrialTestByKey(string nonceKey);
        Task<IEnumerable<S2ENewRequestApproveTrans_TB>> GetApproveTransByRequestIDAllLevel(int requestid, int approvemasterid, int approveGroupID);
        Task<IEnumerable<S2ERMAssessmentApproveTrans_TB>> GetApproveTransByAssessmentIDAllLevel(int requestid, int approvemasterid, int approveGroupID);
        Task<IEnumerable<S2ELABTestApproveTrans_TB>> GetApproveTransByLABIDAllLevel(int LabID, int LabLineID, int approvemasterid, int approveGroupID);
        Task<IEnumerable<S2EAddRawMaterialApproveTrans_TB>> GetApproveTransByAddRMIDAllLevel(int AddRMID, int approvemasterid, int approveGroupID, int AddRMLineID);
        Task<IEnumerable<S2EMaterialRequestApproveTrans_TB>> GetApproveTransByMaterialReqIDAllLevel(int RMREQID, int RMREQLineID, int approvemasterid, int approveGroupID);
        Task<IEnumerable<S2ETrialTestApproveTrans_TB>> GetApproveTransByTrialIDAllLevel(int TrialID, int TrialLineID, int approvemasterid, int approveGroupID);
        Task<IEnumerable<S2EAddRawMaterialSampleApproveTrans_TB>> GetApproveTransByAddRMSampleID(int AddRMSampleID, int approvemasterid, int approveGroupID);
        Task<S2EAddRawMaterialSampleNonce_TB> GetNonceAddRawMaterialSampleByKey(string nonceKey);
        Task<IEnumerable<S2EMaterialRequestSampleApproveTrans_TB>> GetApproveTransByRMReqSampleID(int RMReqSamID, int RMReqSamLineID, int approvemasterid, int approveGroupID);
        Task<S2EMaterialRequestSampleNonce_TB> GetNonceMaterialRequestSampleByKey(string nonceKey);
        Task<IEnumerable<S2EMaterialRequestSampleApproveTrans_TB>> GetApproveTransByRMReqSampleIDAllLevel(int RMReqSamID, int RMReqSamLineID, int approvemasterid, int approveGroupID);
        Task<IEnumerable<S2ELogFileEditApproveTrans_TB>> GetApproveTransLogFileEdit(int groupid, int headid, int approvemasterid);
        Task<S2ELogFileEditNonce_TB> GetLogFileNonceByKey(string nonceKey);
    }
}
