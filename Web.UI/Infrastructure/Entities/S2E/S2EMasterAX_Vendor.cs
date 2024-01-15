using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_MasterAX_Vendor")]
    public class S2EMasterAX_Vendor_TB
    {
        public string ACCOUNTNUM { get; set; }
        public string DATAAREAID { get; set; }
        public string VENDGROUP { get; set; }
        public string CURRENCY { get; set; }
        public string ITEMBUYERGROUPID { get; set; }
        public string NAME { get; set; }
        public string ADDRESS { get; set; }
        public string PHONE { get; set; }
        public string DSG_VENDOREFORM { get; set; }
        public int DSG_VENDORTYPE { get; set; }
    }
}
