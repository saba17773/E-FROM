using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.Queing
{
    [Table("TB_QingMaster_Bay")]
    public class QingMaster_Bay_TB
    {
        public int ID { get; set; }
        public string BAY { get; set; }
        public int ISACTIVE { get; set; }
        public int ISDOM { get; set; }
        public int ISOVS { get; set; }
        public string PLANT { get; set; }
    }
}
