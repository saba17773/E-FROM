using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class TrialTestTodolistGridModel
    {
        public int TRIALID { get; set; }
        public int TRIALLINEID { get; set; }
        public string REQUESTDATE { get; set; }
        public string CHEMICALNAME { get; set; }
        public string REQUESTBY { get; set; }
        public string NONCEKEY { get; set; }
        public string APPROVETRANSID { get; set; }
        public string PROJECTREFNO { get; set; }
        public string REQUESTCODE { get; set; }
        public string SUPPLIERNAME { get; set; }
    }
}
