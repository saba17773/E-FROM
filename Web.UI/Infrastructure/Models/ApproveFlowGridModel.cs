using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ApproveFlowGridModel
    {
        public int Id { get; set; }
        public int ApproveMasterId { get; set; }
        public int ApproveLevel { get; set; }
        public string EmployeeId { get; set; }

        public string GroupDescription { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string BackupEmail { get; set; }
        public int CanApprove { get; set; }
        public int IsFinalApprove { get; set; }
        public int ReceiveWhenComplete { get; set; }
        public int ReceiveWhenFailed { get; set; }
        public string Remark { get; set; }
        public int IsActive { get; set; }

        public int IsSkipAlert { get; set; }
    }
}
