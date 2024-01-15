using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ELABTestApproveTrans")]
    public class S2ELABTestApproveTrans_TB
    {
        public int ID { get; set; }
        public int LABID { get; set; }
        public int LABLINEID { get; set; }
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
        public int ISKEYINWHENAPPROVE { get; set; }
    }
}
