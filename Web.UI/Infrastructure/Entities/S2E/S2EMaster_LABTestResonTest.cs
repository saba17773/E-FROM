using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EMaster_LABTestResonTest")]
    public class S2EMaster_LABTestResonTest_TB
    {
        public int ID { get; set; }
        public string RESONTESTDESC { get; set; }
        public int ISACTIVE { get; set; }
        public int ISREMARK { get; set; }
    }
}
