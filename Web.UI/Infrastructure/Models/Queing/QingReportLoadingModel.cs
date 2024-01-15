using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.Queing
{
    public class QingReportLoadingModel
    {
        public int ID { get; set; }
        public string NO { get; set; }
        public string WORKORDERNO { get; set; }
        public string CONTAINERNO { get; set; }
        public string QTY { get; set; }
        public string STDTIME { get; set; }
        public string TIMEUSE { get; set; }
        public string STATUS { get; set; }
        public string PROCESS { get; set; }
        public string CHECKOUTBY { get; set; }
        public string CHECKOUTDATE { get; set; }
        public string CHECKER { get; set; }
    }
}
