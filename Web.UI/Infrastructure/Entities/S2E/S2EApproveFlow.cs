using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_ApproveFlow")]
    public class S2EApproveFlow_TB
    {
        public int Id { get; set; }
        public int ApproveMasterId { get; set; }
        public int ApproveLevel { get; set; }
        [StringLength(20)]
        public string EmployeeId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        [StringLength(10)]
        public string Company { get; set; }
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        [EmailAddress]
        [StringLength(100)]
        public string BackupEmail { get; set; }
        public int CanApprove { get; set; }
        public int IsFinalApprove { get; set; }
        public int ReceiveWhenComplete { get; set; }
        public int ReceiveWhenFailed { get; set; }
        [StringLength(50)]
        public string Remark { get; set; }
        public int IsActive { get; set; }
        public int IsKeyinWhenApprove { get; set; }
    }
}
