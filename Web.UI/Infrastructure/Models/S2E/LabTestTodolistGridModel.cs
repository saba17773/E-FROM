using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class LabTestTodolistGridModel
    {
        public int LABID { get; set; }
        public int LABLINEID { get; set; }
        public string REQUESTDATE { get; set; }
        public string MANUFACTURE { get; set; }
        public string REQUESTBY { get; set; }
        public string NONCEKEY { get; set; }
        public string APPROVETRANSID { get; set; }
        public string PROJECTREFNO { get; set; }
        public string REQUESTCODE { get; set; }
        public int ISKEYINWHENAPPROVE { get; set; }
    }
}
