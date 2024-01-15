using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels.S2E
{
    public class S2EPurchaseSampleLogsFileGridViewModel
    {
        public int ID { get; set; }
        public int PCSAMPLEID { get; set; }
        public string FILENAME { get; set; }
        public int CREATEBY { get; set; }
        public string CREATEDATE { get; set; }
    }
}
