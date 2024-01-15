using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ETrialTestLogsSendEmailSuccess")]
    public class S2ETrialTestLogsSendEmailSuccess_TB
    {
        public int ID { get; set; }
        public int TRIALID { get; set; }
        public int TRIALLINEID { get; set; }
        public string EMAIL { get; set; }
        public int APPROVELEVEL { get; set; }
        public DateTime? SENDEMAILDATE { get; set; }
        public int ISCREATOR { get; set; }
    }
}
