using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.Queing
{
    public class QingOVSGridModel
    {
        public int ID { get; set; }
        public string NO { get; set; }
        public string WORKORDERNO { get; set; }
        public string PLANT { get; set; }
        public string AGENTCODE { get; set; }
        public string BOOKINGNUMBER { get; set; }
        public string TRUCKID { get; set; }
        public string DRIVERID { get; set; }
        public string DRIVERNAME { get; set; }
        public int TRUCKCATEID { get; set; }
        public string DRIVERTEL { get; set; }
        public string INVOICENO { get; set; }
        public string SEALNO { get; set; }
        public string CONTAINERSIZEID { get; set; }
        public string CONTAINERNO { get; set; }
        public int ROUTEID { get; set; }
        public int ISSUBWORKORDERNO { get; set; }
        public int BAYID { get; set; }
        public int WEIGHID { get; set; }
        public decimal WEIGHTIN { get; set; }
        public DateTime? WEIGHTINDATE { get; set; }
        public decimal WEIGHTOUT { get; set; }
        public DateTime? WEIGHTOUTDATE { get; set; }
        public int STATUS { get; set; }
        public int CHECKINBY { get; set; }
        public string CHECKINDATE { get; set; }
        public string ROUTEDESC { get; set; }
        public string TRUCKCATEDESC { get; set; }
        public string STATUSDETAIL { get; set; }
    }
}
