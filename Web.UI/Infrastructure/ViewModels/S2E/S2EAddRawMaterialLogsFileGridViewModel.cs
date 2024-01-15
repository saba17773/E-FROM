using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels.S2E
{
    public class S2EAddRawMaterialLogsFileGridViewModel
    {
        public int ID { get; set; }
        public int ADDRMID { get; set; }
        public int ADDRMLINEID { get; set; }
        public string FILENAME { get; set; }
        public int CREATEBY { get; set; }
        public string CREATEDATE { get; set; }
    }
}
