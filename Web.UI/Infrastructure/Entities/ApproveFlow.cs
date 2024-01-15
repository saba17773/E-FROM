using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ApproveFlow")]
    public class ApproveFlowTable
    {
        public int Id { get; set; }
        public int ApproveMasterId { get; set; }
        public int ApproveLevel { get; set; }


        [StringLength(20)]
        public string EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string LastName { get; set; }

        [Required]
        [StringLength(10)]
        public string Company { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [Required]
        public string Email { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string BackupEmail { get; set; }

        public int EditCreditLimit { get; set; }
        public int CanApprove { get; set; }
        public int IsFinalApprove { get; set; }
        public int ReceiveWhenComplete { get; set; }
        public int ReceiveWhenFailed { get; set; }

        [StringLength(50)]
        public string Remark { get; set; }

        public int IsActive { get; set; }
        public int IsFile { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
    }
}
