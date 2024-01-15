using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels
{
    public class ApproveMappingGridViewModel
    {
        public int Id { get; set; }
        public string CreditControlType { get; set; } // ex. DOM, OVS
        public string ApproveMaster { get; set; }
        public string CreateBy { get; set; }
    }
}
