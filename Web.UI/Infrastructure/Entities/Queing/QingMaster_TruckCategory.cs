using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.Queing
{
    [Table("TB_QingMaster_TruckCategory")]
    public class QingMaster_TruckCategory_TB
    {
        public int ID { get; set; }
        public string DESCRIPTION { get; set; }
        public int ISOVS { get; set; }
        public int ISDOM { get; set; }
        public int STDTIME { get; set; }
        public int ISACTIVE { get; set; }
    }
}
