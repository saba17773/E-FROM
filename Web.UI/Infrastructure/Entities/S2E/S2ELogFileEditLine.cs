using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ELogFileEditLine")]
    public class S2ELogFileEditLine_TB
    {
        public int ID { get; set; }
        public int LOGFILEHEADID { get; set; }
        public int FILEREFID { get; set; }
        public string FILENAME { get; set; }
        public int ISDELETE { get; set; }
        public int ISCURRENTLOGS { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
    }
}
