using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EApproveMapping")]
    public class S2EApproveMapping_TB
    {
        public int ID { get; set; }
        public int CreateBy { get; set; }
        public string DESCRIPTION { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int APPROVEGROUPID { get; set; }
        public int STEP { get; set; }
        public int ISNEWREQUEST { get; set; }
        public int ISRMASSESSMENT { get; set; }
        public int ISLABTEST { get; set; }
        public int ISPURCHASESAMPLE { get; set; }
        public int ISADDRM { get; set; }
        public int ISREQUESTRM { get; set; }
        public int ISTRIALTEST { get; set; }
        public int ISADDMORERM { get; set; }
        public string PLANT { get; set; }
        public int ISADDRMSAMPLE { get; set; }
        public int ISREQUESTRMSAMPLE { get; set; }
    }
}
