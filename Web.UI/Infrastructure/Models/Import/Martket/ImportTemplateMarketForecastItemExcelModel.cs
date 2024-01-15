using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.Import.Martket
{
    public class ImportTemplateMarketForecastItemExcelModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string Company { get; set; }
        public string Item { get; set; }
        public float Amount { get; set; }

    }
}
