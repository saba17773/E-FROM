using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_CreditControlApproveMapping")]
    public class CreditControlApproveMappingTable
    {
        public int Id { get; set; }
        public string CCType { get; set; }
        public int ApproveMasterId { get; set; }
        public int CreateBy { get; set; }
    }
}
