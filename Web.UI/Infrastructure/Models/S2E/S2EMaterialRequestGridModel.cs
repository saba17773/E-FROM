using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class S2EMaterialRequestGridModel
    {
        public int ADDRMID { get; set; }
        public int ADDRMLINEID { get; set; }
        public string REQUESTCODE { get; set; }
        public string VENDORID { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMNAME { get; set; }
        public string PROJECTREFNO { get; set; }
        public int ADDRMAPPROVESTATUS { get; set; }
        public int REQUESTSTATUS { get; set; }
        public int RMREQID { get; set; }
        public int ISCOMPLETE { get; set; }
        public string CANCELREMARK { get; set; }
    }
}
