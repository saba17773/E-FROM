using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class LabTestResultGridViewModel
    {
        public int ID { get; set; }
        public string LABRESULTDESC { get; set; }
        public int ISACTIVE { get; set; }
        public int ISREMARKA { get; set; }
        public int ISREMARKB { get; set; }
        public int ISPASS { get; set; }
        public string REMARKA { get; set; }
        public string REMARKB { get; set; }
        public int LOGID { get; set; }
    }
}
