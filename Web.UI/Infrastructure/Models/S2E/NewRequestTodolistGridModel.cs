using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.S2E
{
    public class NewRequestTodolistGridModel
    {
        public int REQUESTID { get; set; }
        public string REQUESTCODE { get; set; }
        public string REQUESTDATE { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string REQUESTBY { get; set; }
        public string NONCEKEY { get; set; }
        public string APPROVETRANSID { get; set; }
        public int ISKEYINWHENAPPROVE { get; set; }
        public string DEALER { get; set; }
    }
}
