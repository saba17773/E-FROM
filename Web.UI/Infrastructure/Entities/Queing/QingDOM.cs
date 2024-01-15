using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.Queing
{
    [Table("TB_QingDOM")]
    public class QingDOM_TB
    {
        public int ID { get; set; }
        public string NO { get; set; }
        public string WORKORDERNO { get; set; }
        public string LOADID { get; set; }
        public string PLANT { get; set; }
        public int TRANSPOTCATEID { get; set; }
        public string TRUCKID { get; set; }
        public string TRUCKDESC { get; set; }
        public string DRIVERID { get; set; }
        public string DRIVERNAME { get; set; }
        public int TRUCKCATEID { get; set; }
        public int STDTIME { get; set; }
        public int AGENTID { get; set; }
        public string DRIVERTEL { get; set; }
        public int PROVINCESID { get; set; }
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
        public decimal ASSIGNBAYBY { get; set; }
        public DateTime? ASSIGNBAYDATE { get; set; }
        public decimal CHECKOUTBY { get; set; }
        public DateTime? CHECKOUTDATE { get; set; }
    }
}
