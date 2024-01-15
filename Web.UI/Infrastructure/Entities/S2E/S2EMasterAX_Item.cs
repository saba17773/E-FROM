using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_MasterAX_Item")]
    public class S2EMasterAX_Item_TB
    {
        public string ITEMID { get; set; }
        public string ITEMNAME { get; set; }
        public string DATAAREAID { get; set; }
        public string ITEMGROUPID { get; set; }
    }
}
