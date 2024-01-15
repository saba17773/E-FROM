using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class LabTestLogTestResultGridViewModel
    {
        public int LABID { get; set; }
        public int LABEVALUATIONID { get; set; }
        public string LABEVALUATIONDESC { get; set; }
        public int ISPASS { get; set; }
        public int ISREMARKA { get; set; }
        public int ISREMARKB { get; set; }
        public string REMARKA { get; set; }
        public string REMARKB { get; set; }
    }
}
