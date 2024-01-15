using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_VenderApproveGroup")]
    public class VenderApproveGroupMaster_TB
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string DESCRIPTION { get; set; }

        public int ISACTIVE { get; set; }
    }
}
