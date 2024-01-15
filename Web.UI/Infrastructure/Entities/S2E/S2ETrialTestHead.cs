using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ETrialTestHead")]
    public class S2ETrialTestHead_TB
    {
        public int ID { get; set; }
        public int RMREQID { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public string CANCELREMARK { get; set; }
    }
}
