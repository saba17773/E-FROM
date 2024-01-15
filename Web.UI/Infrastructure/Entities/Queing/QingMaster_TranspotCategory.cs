using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.Queing
{
    [Table("TB_QingMaster_TranspotCategory")]
    public class QingMaster_TranspotCategory_TB
    {
        public int ID { get; set; }
        public string DESCRIPTION { get; set; }
        public int ISACTIVE { get; set; }
        public int ISAGENT { get; set; }
    }
}
