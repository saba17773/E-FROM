using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.Queing
{
    public class QingDOMGridModel
    {
        public int ID { get; set; }
        public string NO { get; set; }
        public string WORKORDERNO { get; set; }
        public string PLANT { get; set; }
        public string TRUCKID { get; set; }
        public string DRIVERID { get; set; }
        public string DRIVERNAME { get; set; }
        public int TRUCKCATEID { get; set; }
        public int STDTIME { get; set; }
        public string DRIVERTEL { get; set; }
        public string PROVINCESNAME { get; set; }
        public int PROVINCESID { get; set; }
        public int STATUS { get; set; }
        public int CHECKINBY { get; set; }
        public string CHECKINDATE { get; set; }
        public string STATUSDETAIL { get; set; }
        public int BAYID { get; set; }
        public string BAYDESC { get; set; }
        public string TRUCKCATEDESC { get; set; }
        public int WEIGHID { get; set; }
    }
}
