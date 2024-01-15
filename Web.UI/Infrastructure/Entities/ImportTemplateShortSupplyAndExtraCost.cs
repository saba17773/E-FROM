using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ImportTemplateShortSupplyAndExtraCost")]
    public class ImportTemplateShortSupplyAndExtraCostTable
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Date { get; set; }
        public string Company { get; set; }
        public string Group { get; set; }
        public string Item { get; set; }
        public string Supplier { get; set; }
        public string Issue { get; set; }
        public int ShortQtyMT { get; set; }
        public int ShortDays { get; set; }
        public int ExtraCost { get; set; }
        public string Action { get; set; }
    }
}
