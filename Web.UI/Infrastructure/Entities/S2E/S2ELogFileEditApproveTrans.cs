using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ELogFileEditApproveTrans")]
    public class S2ELogFileEditApproveTrans_TB
    {
        public int ID { get; set; }
        public int LOGFILEHEADID { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int APPROVEGROUPID { get; set; }
        public string EMAIL { get; set; }
        public int APPROVELEVEL { get; set; }
        public DateTime? SENDEMAILDATE { get; set; }
        public DateTime? APPROVEDATE { get; set; }
        public DateTime? REJECTDATE { get; set; }
        public int ISDONE { get; set; }
        public string REMARK { get; set; }
        public int ISCURRENTAPPROVE { get; set; }
    }
}
