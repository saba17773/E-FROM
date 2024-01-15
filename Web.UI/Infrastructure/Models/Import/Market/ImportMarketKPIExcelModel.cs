using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.Import.Market
{
    public class ImportMarketKPIExcelModel
    {
        [Column(1)]
        public int Year { get; set; }

        [Column(2)]
        public string Company { get; set; }

        [Column(3)]
        public string SubGroupNew { get; set; }

        [Column(4)]
        public float KPI { get; set; }
    }
}
