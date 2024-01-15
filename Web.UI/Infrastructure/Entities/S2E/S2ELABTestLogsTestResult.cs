using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ELABTestLogsTestResult")]
    public class S2ELABTestLogsTestResult_TB
    {
        public int ID { get; set; }
        public int LABID { get; set; }
        public int LABLINEID { get; set; }
        public int LABEVALUATIONID { get; set; }
        public string LABEVALUATIONDESC { get; set; }
        public int ISPASS { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public string REMARK1 { get; set; }
        public string REMARK2 { get; set; }
    }
}
