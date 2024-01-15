using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EMaster_TrialTestEvaluation")]
    public class S2EMaster_TrialTestEvaluation_TB
    {
        public int ID { get; set; }
        public string TRIALRESULTDESC { get; set; }
        public int ISACTIVE { get; set; }
        public int ISREMARK1 { get; set; }
        public int ISREMARK2 { get; set; }
    }
}
