using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.ViewModels;

namespace Web.UI.Domain.Interfaces
{
    public interface IPromotionRepository
    {
        Task<PromotionApproveMappingTable> GetApproveGroupId(string PromotionType, int CreateBy);
        Task<IEnumerable<PromotionApproveTransTable>> GetApproveTransByCCId(int PromotionId);
        Task<PromotionApproveTransTable> GetApproveTransByLevel(int PromotionId, int level);
        Task<IEnumerable<PromotionAttachFileTable>> GetFileByCCIdAsync(int ccId, string requestType);
        Task<IEnumerable<ApproveFlowTable>> GetApproveTransFlowByCCId(int ccId);
        Task<PromotionApproveTransGridModel> GetCreateBy(int PromotionId);
    }
}
