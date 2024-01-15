using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ETrialTestLogsTestResult")]
    public class S2ETrialTestLogsTestResult_TB
    {
        public int ID { get; set; }
        public int TRIALID { get; set; }
        public int TRIALLINEID { get; set; }
        public int TRIALEVALUATIONID { get; set; }
        public string TRIALEVALUATIONDESC { get; set; }
        public int ISPASS { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public string REMARK1 { get; set; }
        public string REMARK2 { get; set; }
    }
}
