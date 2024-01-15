using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels.S2E
{
    public class ApproveMappingGridModel
    {
        public int Id { get; set; }
        public string CreateBy { get; set; }
        public string S2EType { get; set; } // ex. Purchase,Qtech
        public string ApproveMaster { get; set; }
    }
}
