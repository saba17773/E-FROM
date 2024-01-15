using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2EMaterialRequestSampleDetailGridModel
    {
        public int ADDRMSAMPLEID { get; set; }
        public int RMREQSAMID { get; set; }
        public int RMREQSAMLINEID { get; set; }
        public string NO { get; set; }
        public string REQUESTDATE { get; set; }
        public string DEPARTMENT { get; set; }
        public string SUPGROUP { get; set; }
        public string QTY { get; set; }
        public string UNIT { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int APPROVESTATUS { get; set; }
        public int APPROVEGROUPID { get; set; }
        public string GROUPDESCRIPTION { get; set; }
        public string REQUESTBY { get; set; }
    }
}
