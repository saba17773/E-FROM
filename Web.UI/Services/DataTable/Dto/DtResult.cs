using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Services.DataTable.Dto
{
    public class DtResult<T>
    {
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
