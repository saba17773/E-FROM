using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EPurchaseSampleLogsFile")]
    public class S2EPurchaseSampleLogsFile_TB
    {
        public int ID { get; set; }
        public int PCSAMPLEID { get; set; }
        public string FILENAME { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
    }
}
