using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_MasterAX_ProductGroup")]
    public class S2EMasterAX_ProductGroup_TB
    {
        public int ID { get; set; }
        public string DSGPRODUCTGROUPID { get; set; }
        public string DSGPRODUCTGROUPDESCRIPTION { get; set; }
        public string DATAAREAID { get; set; }
    }
}
