using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2EAssessmentGetDataPCRequestGridModel
    {
        public string SUPPLIERNAME { get; set; }
        public string PRODUCTIONSITE { get; set; }
        public string ITEMID { get; set; }
        public string ITEMNAME { get; set; }
        public decimal PRICE { get; set; }
    }
}
