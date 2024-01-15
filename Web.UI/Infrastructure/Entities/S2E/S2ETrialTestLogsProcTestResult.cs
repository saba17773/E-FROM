using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ETrialTestLogsProcTestResult")]
    public class S2ETrialTestLogsProcTestResult_TB
    {
        public int ID { get; set; }
        public int TRIALID { get; set; }
        public int TRIALLINEID { get; set; }
        public int PROCESSID { get; set; }
        public string PROCESSDESC { get; set; }
        public int ISPASS { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
    }
}
