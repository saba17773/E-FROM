using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Infrastructure.Entities;


namespace Web.UI.Domain.Interfaces
{
    public interface IVenderControlRepository
    {
        Task<IEnumerable<VenderApproveTrans_TB>> GetApproveTransByRequestID(int requestid, int project);

        Task<VenderApproveTrans_TB> GetApproveTransByLevel(int requestid, int level, int approvemasterid);

        Task<IEnumerable<VenderLogDoc_TB>> GetVenderLogDocByRequestID(int requestid);

        Task<IEnumerable<VenderLogFile_TB>> GetVenderLogFileByRequestID(int requestid);

        Task<IEnumerable<VenderApproveTrans_TB>> GetApproveTranIsCurrentByRequestID(int requestid,int approvemasterid);

        Task<IEnumerable<VenderApproveTrans_TB>> GetTotalLevelInApproveTrans(int requestid, int approvemasterid);
        Task<IEnumerable<ApproveFlow_TB>> GetApproveFlowByApproveMasterIdAsync(int id);

    }
}
