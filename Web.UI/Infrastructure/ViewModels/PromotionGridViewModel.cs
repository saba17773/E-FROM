using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class PromotionGridViewModel
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public string Pattern { get; set; }
        public string RequestType { get; set; }
        public string CustomerName { get; set; }
        public string RequestStatus { get; set; }
        public int CurrentApproveStep { get; set; }
        public int TotalApproveStep { get; set; }
        public string ByName { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string CancelRemark { get; set; }

    }
}
