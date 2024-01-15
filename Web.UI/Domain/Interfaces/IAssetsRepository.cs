using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.ViewModels;

namespace Web.UI.Domain.Interfaces
{
    public interface IAssetsRepository
    {
        Task<AssetsApproveMappingTable> GetApproveGroupId(int AssetsType, int CreateBy, string CompanyGroup);
        Task<IEnumerable<AssetsApproveTransTable>> GetApproveTransByCCId(int AssetsId);
        Task<AssetsApproveTransTable> GetApproveTransByLevel(int AssetsId, int level);
        Task<Company> GetCompany(string company);
        Task<AssetsUser> GetUser(int id);
        Task<IEnumerable<AssetsApproveTransTable>> GetApproveTransByReviewed(int AssetsId);
        Task<IEnumerable<AssetsApproveTransTable>> GetApproveTransByLV3(int AssetsId);
        Task<IEnumerable<AssetsLineTable>> GetAssetsLine(string AssetsLine);
        Task<AssetsApproveTransTable> GetLevelTrans(int level, int masterid);
    }
}