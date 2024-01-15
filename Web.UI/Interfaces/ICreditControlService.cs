using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Interfaces
{
    public interface ICreditControlService
    {
        string GetLatestRequestNumberAsync(string seqValue, string prefix);
        string GetLatestRequestNumberPromotionAsync(string seqValue, string prefix);
        string GetLatestRequestNumberMemoAsync(string seqValue, string prefix);
        string GetLatestRequestNumberAssetsAsync(string seqValue, string prefix);
    }
}
