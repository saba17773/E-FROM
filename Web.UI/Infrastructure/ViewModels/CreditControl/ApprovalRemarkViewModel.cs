using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels.CreditControl
{
    public class ApprovalRemarkViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ApproveLevel { get; set; }
        public string Remark { get; set; }
        public int Urgent { get; set; }
        public string BackupEmail { get; set; }
    }
}