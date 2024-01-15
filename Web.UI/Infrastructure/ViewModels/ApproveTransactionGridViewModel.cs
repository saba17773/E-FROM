using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class ApproveTransactionGridViewModel
    {
        public int Id { get; set; }
        public int ApproveLevel { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }
        public string Name { get; set; }
        public int IsDone { get; set; }
        public int Urgent { get; set; }
        public string ApproveGroup { get; set; }
        public string BackupEmail { get; set; }
        public DateTime? SendEmailDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public DateTime? RejectDate { get; set; }
    }
}