using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2ELabTestGridModel
    {
        public int RMREQSAMID { get; set; }
        public int REQUESTSTATUS { get; set; }
        public string REQUESTCODE { get; set; }
        public string SUPPLIERNAME { get; set; }
        public int LABTESTID { get; set; }
        public int LABTESTLINEID { get; set; }
        public string PROJECTREFNO { get; set; }
        public int LABAPPROVESTATUS { get; set; }
        public string REQUESTBY { get; set; }
        public int APPROVEGROUPID { get; set; }
        public int ORDERLIST { get; set; }
        public string TESTRESULT { get; set; }
        public int ASSESSAPPROVESTATUS { get; set; }
        public int REQUESTID { get; set; }
        public int ASSESSMENTID { get; set; }
        public string CANCELREMARK { get; set; }
    }
}
