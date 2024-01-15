using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ELABTestLogsDoc")]
    public class S2ELABTestLogsDoc_TB
    {
        public int ID { get; set; }
        public int LABID { get; set; }
        public int LABLINEID { get; set; }
        public int DOCID { get; set; }
        public string DOCDESCRIPTION { get; set; }
        public string REMARK { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
    }
}
