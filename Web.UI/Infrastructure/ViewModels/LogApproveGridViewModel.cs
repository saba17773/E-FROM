using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class LogApproveGridViewModel
    {
        public int ID { get; set; }
        public int REQUESTID { get; set; }
        public int APPROVEMASTERID { get; set; }
        public string EMAIL { get; set; }
        public int APPROVELEVEL { get; set; }

        public string APPROVEDATE { get; set; }
        public string DESCRIPTION { get; set; }
        public string Name { get; set; }
    }
}
