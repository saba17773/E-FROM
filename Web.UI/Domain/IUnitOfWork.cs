using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Domain.Interfaces;

namespace Web.UI.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        ICreditControlRepository CreditControl { get; }
        INonceRepository Nonce { get; }
        IEmployeeRepository Employee { get; }

        IVenderNonceRepository VenderNonce { get; }
        IVenderControlRepository VenderControl { get; }
        IImportRepository Import { get; }
        IPCDashboardRepository PCDashboard { get; }
        IPromotionRepository Promotion { get; }
        IS2EControlRepository S2EControl { get; }
        IMemoRepository Memo { get; }
        IAssetsRepository Assets { get; }
        void Complete();
    }
}
