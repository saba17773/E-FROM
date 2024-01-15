using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class VenderApproveTransactionGridViewModel
    {
        public int ID { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int APPROVELEVEL { get; set; }
        public string EMAIL { get; set; }
        public string REMARK { get; set; }
        public int ISDONE { get; set; }
        public string SENDEMAILDATE { get; set; }
        public string APPROVEDATE { get; set; }
        public string REJECTDATE { get; set; }

        public int GROUPID { get; set; }
        public string DESCRIPTION { get; set; }
    }
}
