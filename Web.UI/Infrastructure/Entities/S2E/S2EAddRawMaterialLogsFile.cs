using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EAddRawMaterialLogsFile")]
    public class S2EAddRawMaterialLogsFile_TB
    {
        public int ID { get; set; }
        public int ADDRMID { get; set; }
        public int ADDRMLINEID { get; set; }
        public string FILENAME { get; set; }
        public int ISACTIVE { get; set; }
        public int CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
    }
}
