using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Services.DataTable.Dto
{
    public class DtParameters
    {
        public string Search { get; set; }
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string OrderDir { get; set; }
        public string OrderColumn { get; set; }
    }
}
