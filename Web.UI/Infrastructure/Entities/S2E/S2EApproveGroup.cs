using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EApproveGroup")]
    public class S2EApproveGroup_TB
    {
        public int ID { get; set; }
        public string GROUPDESCRIPTION { get; set; }
        public int ISACTIVE { get; set; }
        public int ISMAINPROCESS { get; set; }
        public int ISPLANT { get; set; }
    }
}
