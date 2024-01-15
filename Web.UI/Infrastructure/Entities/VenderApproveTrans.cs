using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_VenderApproveTrans")]
    public class VenderApproveTrans_TB
    {
        public int ID { get; set; }

        public int REQUESTID { get; set; }

        public int APPROVEMASTERID { get; set; }

        public int APPROVELEVEL { get; set; }

        [StringLength(100)]
        public string EMAIL { get; set; }

        public DateTime? SENDEMAILDATE { get; set; }

        public DateTime? APPROVEDATE { get; set; }

        public DateTime? REJECTDATE { get; set; }

        [StringLength(100)]
        public string REMARK { get; set; }

        public int ISDONE { get; set; }

        public int ISCURRENTAPPROVE { get; set; }

        public string PROCESS { get; set; }

        public int ISALERT { get; set; }

        public int ISSKIPALERT { get; set; }

    }
}
