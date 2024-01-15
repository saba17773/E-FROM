using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EAddRawMaterialSampleLogsFile")]
    public class S2EAddRawMaterialSampleLogsFile_TB
    {
        public int ID { get; set; }
        public int ADDRMSAMPLEID { get; set; }
        public string FILENAME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public int ISACTIVE { get; set; }
        
    }
}
