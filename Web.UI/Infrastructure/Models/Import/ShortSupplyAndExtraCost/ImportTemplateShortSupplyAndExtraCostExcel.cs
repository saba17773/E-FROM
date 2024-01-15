using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.Import.ShortSupplyAndExtraCost
{
    public class ImportTemplateShortSupplyAndExtraCostExcel
    {
        [Column(1)]
        public int Year { get; set; }

        [Column(2)]
        public int Month { get; set; }

        [Column(3)]
        public int Date { get; set; }

        [Column(4)]
        public string Company { get; set; }

        [Column(5)]
        public string Group { get; set; }

        [Column(6)]
        public string Item { get; set; }

        [Column(7)]
        public string Supplier { get; set; }

        [Column(8)]
        public string Issue { get; set; }

        [Column(9)]
        public int ShortQtyMT { get; set; }

        [Column(10)]
        public int ShortDays { get; set; }

        [Column(11)]
        public int ExtraCost { get; set; }

        [Column(12)]
        public string Action { get; set; }
    }
}
