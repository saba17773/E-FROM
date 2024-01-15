using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2EMaterialRequestHead")]
    public class S2EMaterialRequestHead_TB
    {
        public int ID { get; set; }
        public int ADDRMID { get; set; }
        public string PLANT { get; set; }
        public string ITEMGROUP { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMNAME { get; set; }
        public string UNIT { get; set; }
        public int REQUESTSTATUS { get; set; }
        public string CANCELREMARK { get; set; }
    }
}
