using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EMaterialRequestSampleHead")]
    public class S2EMaterialRequestSampleHead_TB
    {
        public int ID { get; set; }
        public int ADDRMSAMPLEID { get; set; }
        public int REQUESTSTATUS { get; set; }
    }
}
