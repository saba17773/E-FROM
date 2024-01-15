using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ImportTemplateStockAccExcelModel
    {
        [Column(1)]
        public int Year { get; set; }

        [Column(2)]
        public int Month { get; set; }

        [Column(3)]
        public float StockAcc { get; set; }

        [Column(4)]
        public string Company { get; set; }
    }
}
