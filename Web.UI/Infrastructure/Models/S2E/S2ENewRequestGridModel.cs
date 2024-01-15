using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2ENewRequestGridModel
    {
        public int ID { get; set; }
        public string REQUESTCODE { get; set; }
        public string REQUESTNO { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string REQUESTBY { get; set; }
        public int APPROVESTATUS { get; set; }
        public int APPROVEGROUPID { get; set; }
        public string PAGEBACK { get; set; }
        public string CANCELREMARK { get; set; }
    }
}
