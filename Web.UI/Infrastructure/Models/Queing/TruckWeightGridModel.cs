using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.Queing
{
    public class TruckWeightGridModel
    {
        public string TRUCKID { get; set; }
        public int WEIGHID { get; set; }
        public string DATEIN { get; set; }
        public decimal WEIGHTIN { get; set; }
        public string DATEOUT { get; set; }
        public decimal WEIGHTOUT { get; set; }
    }
}
