using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ETrialTestLogsPrdTestResult")]
    public class S2ETrialTestLogsPrdTestResult_TB
    {
        public int ID { get; set; }
        public int TRIALID { get; set; }
        public int TRIALLINEID { get; set; }
        public string PRODUCTTESTDESC { get; set; }
        public int ISPASS { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
    }
}
