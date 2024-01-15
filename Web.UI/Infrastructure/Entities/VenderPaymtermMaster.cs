using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_VenderPaymterm")]
    public class VenderPaymtermMaster_TB
    {
        public int ID { get; set; }
        public string PAYMTERMID { get; set; }
        public string DESCRIPTION { get; set; }
        public string DATAAREAID { get; set; }
    }
}
