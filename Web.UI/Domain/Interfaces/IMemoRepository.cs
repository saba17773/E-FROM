using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Infrastructure.Entities;
using Web.UI.Infrastructure.ViewModels;

namespace Web.UI.Domain.Interfaces
{
    public interface IMemoRepository
    {
        Task<MemoCustomerTable> GetCustomerByCodeAsync(string custcode);
        Task<IEnumerable<MemoAttachFileTable>> GetFileByCCIdAsync(int ccId);
        Task<MemoGridViewModel> GetCreateBy(int Id);
        Task<IEnumerable<MemoItemTable>> GetItemIdAsync(int Id);
        Task<IEnumerable<MemoItemTable>> GetItemByCCIdAsync(int ccId);

    }
}
