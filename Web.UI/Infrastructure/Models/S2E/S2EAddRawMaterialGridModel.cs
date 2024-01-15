using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2EAddRawMaterialGridModel
    {
        public int PCSAMPLEID { get; set; }
        public int REQUESTID { get; set; }
        public int ASSESSMENTID { get; set; }
        public int LABID { get; set; }
        public int LABLINEID { get; set; }
        public string VENDORID { get; set; }
        public string SUPPLIERNAME { get; set; }
        public int ISPURCHASESAMPLE { get; set; }
        public int APPROVEMASTERID { get; set; }
        public int PCSAMPLESTATUS { get; set; }
        public string REQUESTBY { get; set; }
        public string REQUESTCODE { get; set; }
        public string PROJECTREFNO { get; set; }
        public int ADDRMID { get; set; }
        public int ADDRMLINEID { get; set; }
        public int ADDRMAPPROVESTATUS { get; set; }
        public int APPROVEGROUPID { get; set; }
        public int ISADDMORE { get; set; }
    }
}
