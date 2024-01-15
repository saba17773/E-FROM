using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ELABTestHead")]
    public class S2ELABTestHead_TB
    {
        public int ID { get; set; }
        public int REQUESTID { get; set; }
        public int ASSESSMENTID { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public string ITEMGROUP { get; set; }
        public string CANCELREMARK { get; set; }
    }
}
