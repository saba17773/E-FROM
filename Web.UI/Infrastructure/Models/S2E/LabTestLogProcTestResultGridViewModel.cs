using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class LabTestLogProcTestResultGridViewModel
    {
        public int LABID { get; set; }
        public string PROCESSDESC { get; set; }
        public int ISPASS { get; set; }
        public int PROCESSID { get; set; }
    }
}
