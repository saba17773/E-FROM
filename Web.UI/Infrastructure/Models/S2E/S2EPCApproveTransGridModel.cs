using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2EApproveTransGridModel
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
        public string GROUPDESCRIPTION { get; set; }
    }
}
