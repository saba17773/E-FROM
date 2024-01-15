using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EMaster_LABTestTypeOfRM")]
    public class S2EMaster_LABTestTypeOfRM_TB
    {
        public int ID { get; set; }
        public string TYPEOFRMDESC { get; set; }
        public int ISACTIVE { get; set; }
    }
}
