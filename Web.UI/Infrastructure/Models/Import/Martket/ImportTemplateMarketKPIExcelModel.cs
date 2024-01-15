using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.Import.Martket
{
    public class ImportTemplateMarketKPIExcelModel
    {
        public int Year { get; set; }
        public string Company { get; set; }
        public string SubGroupNew { get; set; }
        public float KPI { get; set; }
    }
}
