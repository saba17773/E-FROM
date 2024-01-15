using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.Queing
{
    [Table("TB_QingMaster_AgentTractor")]
    public class QingMaster_AgentTractor_TB
    {
        public int ID { get; set; }
        public string AGENTCODE { get; set; }
        public string AGENTDESCRIPTION { get; set; }
        public int ISACTIVE { get; set; }
    }
}
