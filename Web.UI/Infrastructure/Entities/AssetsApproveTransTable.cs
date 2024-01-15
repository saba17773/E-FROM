using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_FixAssetsApproveTrans")]
    public class AssetsApproveTransTable
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
        public string Position { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Disposition { get; set; }
        public string BackupEmail { get; set; }
    }
}
