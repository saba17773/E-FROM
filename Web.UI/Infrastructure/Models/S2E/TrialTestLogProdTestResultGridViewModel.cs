using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class TrialTestLogProdTestResultGridViewModel
    {
        public int ROW { get; set; }
        public int ID { get; set; }
        public int TRIALID { get; set; }
        public string PRODUCTTESTDESC { get; set; }
        public int ISPASS { get; set; }
    }
}
