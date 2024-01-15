using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2EGetPONoByVendorIDGridModel
    {
        public string VENDORID { get; set; }
        public string ITEMID { get; set; }
        public string ITEMDESCRIPTION { get; set; }
        public string PONO { get; set; }
        public decimal QTY { get; set; }
        public decimal UNITPRICE { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string CURRENCYCODE { get; set; }
    }
}
