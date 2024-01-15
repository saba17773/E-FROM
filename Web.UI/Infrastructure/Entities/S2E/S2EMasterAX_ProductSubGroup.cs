using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_MasterAX_ProductSubGroup")]
    public class S2EMasterAX_ProductSubGroup_TB
    {
        public int ID { get; set; }
        public string DSGPRODUCTGROUPID { get; set; }
        public string DSGSUBGROUPID { get; set; }
        public string DSGSUBGROUPDESCRIPTION { get; set; }
        public string DATAAREAID { get; set; }
    }
}
