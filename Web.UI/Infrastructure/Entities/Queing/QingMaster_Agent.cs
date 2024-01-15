using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.Queing
{
    [Table("TB_QingMaster_Agent")]
    public class QingMaster_Agent_TB
    {
        public int ID { get; set; }
        public string AGENTNAME { get; set; }
        public string COMPANY { get; set; }
        public int ISACTIVE { get; set; }
        public int ISREMARK { get; set; }
    }
}
