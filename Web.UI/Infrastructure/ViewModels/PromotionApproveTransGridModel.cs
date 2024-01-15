using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class PromotionApproveTransGridModel
    {
        public int Id { get; set; }
        public int CCId { get; set; }
        public int ApproveMasterId { get; set; }
        public string Email { get; set; }
        public int ApproveLevel { get; set; }
        public DateTime? SendEmailDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public DateTime? RejectDate { get; set; }
        public int IsDone { get; set; }
        public string Remark { get; set; }
        public int Urgent { get; set; }
        public int ApproveFlowId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}