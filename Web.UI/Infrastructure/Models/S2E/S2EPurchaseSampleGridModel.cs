using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2EPurchaseSampleGridModel
    {
        public int LABID { get; set; }
        public int LABLINEID { get; set; }
        public int REQUESTID { get; set; }
        public int ASSESSMENTID { get; set; }
        public string REQUESTCODE { get; set; }
        public string REQUESTNO { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string VENDORID { get; set; }
        public string REQUESTBY { get; set; }
        public int LABSTATUS { get; set; }
        public int PCSAMPLEID { get; set; }
        public int PCSAMPLESTATUS { get; set; }
        public int ADDRMID { get; set; }
        public int ISPURCHASESAMPLE { get; set; }
    }
}
