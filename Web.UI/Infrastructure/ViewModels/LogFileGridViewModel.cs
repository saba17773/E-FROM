using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class LogFileGridViewModel
    {
        public int ID { get; set; }
        public int REQUESTID { get; set; }
        public int CREATEBY { get; set; }
        public string FILENAME { get; set; }
        public string UPLOADDATE { get; set; }
    
        public int ISACTIVE { get; set; }
        public int ISTEMP { get; set; }
    }
}
