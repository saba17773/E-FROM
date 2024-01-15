using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels.S2E
{
    public class S2EAddRawMaterialSampleLogsFileGridViewModel
    {
        public int ID { get; set; }
        public int ADDRMSAMPLEID { get; set; }
        public string FILENAME { get; set; }
        public string CREATEBY { get; set; }
        public string CREATEDATE { get; set; }
        public int ISACTIVE { get; set; }
    }
}
