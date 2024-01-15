using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2ETrialTestGridModel
    {
        public int RMREQID { get; set; }
        public int REQUESTSTATUS { get; set; }
        public string REQUESTCODE { get; set; }
        public string PROJECTREFNO { get; set; }
        public string VENDORID { get; set; }
        public string SUPPLIERNAME { get; set; }
        public int TRIALID { get; set; }
        public int TRIALLINEID { get; set; }
        public int TRIALAPPROVESTATUS { get; set; }
        public string REQUESTBY { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMNAME { get; set; }
        public int APPROVEGROUPID { get; set; }
        public string CANCELREMARK { get; set; }
    }
}
