using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2EAddRawMaterialSampleGridModel
    {
        public int REQUESTID { get; set; }
        public int ASSESSMENTID { get; set; }
        public int ADDRMSAMPLEID { get; set; }
        public string REQUESTCODE { get; set; }
        public string PROJECTREFNO { get; set; }
        public string VENDORID { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string REQUESTBY { get; set; }
        public int ASSESSAPPROVESTATUS { get; set; }
        public int ADDRMSAMPLEAPPROVESTATUS { get; set; }
        public int APPROVEGROUPID { get; set; }
        public string ORDERLIST { get; set; }
    }
}
