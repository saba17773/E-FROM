using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.Models.CreditControl;
using Web.UI.Infrastructure.ViewModels;
using Web.UI.Infrastructure.ViewModels.CreditControl;

namespace Web.UI.Domain.Interfaces
{
    public interface ICreditControlRepository
    {
        Task<CreditControlApproveMappingTable> GetApproveGroupId(string creditControlType, int createBy);
        Task<IEnumerable<CreditControlApproveTransTable>> GetApproveTransByCCId(int creditControlId);
        Task<CreditControlApproveTransTable> GetApproveTransByLevel(int creditControlId, int level);
        Task<AddressTable> GetAddressByCCIdAsync(int ccId, string addressType);
        Task<IEnumerable<CreditControlAttachFileTable>> GetFileByCCIdAsync(int ccId, string requestType);
        Task<IEnumerable<ApproveFlowTable>> GetApproveFlowByApproveMasterIdAsync(int id);
        Task<CreditControlAttachFileTable> GetAttachFileAsync(int id, string creditControlType, int fileNo);
        Task<ApproveFlowTable> GetApproveFlowBackupEmailAsync(int approveMasterId, int approveLevel);
        Task<IEnumerable<ApproveFlowTable>> GetApproveFlowByCCIdAsync(int creditControlId);
        Task<IEnumerable<CustomerTypeByProductTransTable>> GetCustomerTypeTransByCCId(int creditControlId);
        Task<IEnumerable<CustomerTypeTransViewModel>> GetCustomerTypeTransViewByCCId(int creditControlId);
        Task<IEnumerable<PaymentMethodTransTable>> GetPaymentMethodTransByCCIdAsync(int creditControlId);
        Task<IEnumerable<CreditControlCheckListFileTable>> GetCheckListFileByCCIdAsync(int creditControlId);
        Task<ViewDOMViewModel> GetDOMDataByCCIdAsync(int creditControlId);
        Task<ViewOVSViewModel> GetOVSDataByCCIdAsync(int creditControlId);
        Task<IEnumerable<ApprovalRemarkViewModel>> GetApprovalRemarkAsync(int creditControlId);
        Task<RequesterInfoViewModel> GetRequesterInfoAsync(int creditControlId);
        Task<IEnumerable<EmailForAlertRequestApproveModel>> GetEmailForSendAlertApproveRequest();
        Task<IEnumerable<RequestWaitingToApproveModel>> GetRequestWaitingToApproveByEmail(string email);

    }
}
