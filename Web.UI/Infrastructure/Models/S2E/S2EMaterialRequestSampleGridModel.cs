using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2EMaterialRequestSampleGridModel
    {
        public int ADDRMSAMPLEID { get; set; }
        public string REQUESTCODE { get; set; }
        public string VENDORID { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMNAME { get; set; }
        public int ADDRMSAMPLEAPPROVESTATUS { get; set; }
        public int RMREQSAMID { get; set; }
        public int REQUESTSTATUS { get; set; }
        public int ISCOMPLETE { get; set; }
        public string CANCELREMARK { get; set; }
    }
}
