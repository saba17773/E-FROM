using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_CreditControlApproveTrans")]
    public class CreditControlApproveTransTable
    {
        public int Id { get; set; }
        public int CCId { get; set; }
        public int Urgent { get; set; }

        [Required]
        public int ApproveMasterId { get; set; }
        public int ApproveFlowId { get; set; }
        public string Email { get; set; }

        [Required]
        public int ApproveLevel { get; set; }
        public DateTime? SendEmailDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public DateTime? RejectDate { get; set; }
        public int IsDone { get; set; }
        public string Remark { get; set; }
    }
}
