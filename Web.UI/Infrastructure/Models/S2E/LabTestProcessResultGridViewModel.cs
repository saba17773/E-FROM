using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class LabTestProcessResultGridViewModel
    {
        public int ID { get; set; }
        public string PROCESSDESC { get; set; }
        public int ISACTIVE { get; set; }
        public int ISPASS { get; set; }
        public int LOGID { get; set; }
    }
}
