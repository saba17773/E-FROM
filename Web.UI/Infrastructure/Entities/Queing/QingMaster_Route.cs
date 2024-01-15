using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.Queing
{
    [Table("TB_QingMaster_Route")]
    public class QingMaster_Route_TB
    {
        public int ID { get; set; }
        public string ROUTE { get; set; }
        public int ISACTIVE { get; set; }
    }
}
