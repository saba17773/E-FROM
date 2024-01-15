using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.Queing
{
    [Table("TB_QingOVS")]
    public class QingOVS_TB
    {
        public int ID { get; set; }
        public string NO { get; set; }
        public string WORKORDERNO { get; set; }
        public string LOADID { get; set; }
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
        public int CONTAINERSIZEID { get; set; }
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
        public string REMARK { get; set; }
        public int CHECKINBY { get; set; }
        public DateTime? CHECKINDATE { get; set; }
        public int ASSIGNBAYBY { get; set; }
        public DateTime? ASSIGNBAYDATE { get; set; }
        public int CHECKOUTBY { get; set; }
        public DateTime? CHECKOUTDATE { get; set; }
        public int CANCELBY { get; set; }
        public DateTime? CANCELDATE { get; set; }
    }
}
