using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_VenderLogsApprove")]
    public class VenderLogsApprove_TB
    {
        public int ID { get; set; }
        public int REQUESTID { get; set; }
        [StringLength(100)]
        public string EMAIL { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public int ISAPPROVE { get; set; }
        public int ISREJECT { get; set; }
    }
}
