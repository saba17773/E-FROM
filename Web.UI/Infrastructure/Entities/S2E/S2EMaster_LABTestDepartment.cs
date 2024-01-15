using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EMaster_LABTestDepartment")]
    public class S2EMaster_LABTestDepartment_TB
    {
        public int ID { get; set; }
        public string DEPARTMENTDESC { get; set; }
        public int ISACTIVE { get; set; }
        public int ISREMARK { get; set; }
        public int ISORDERBY { get; set; }
    }
}
