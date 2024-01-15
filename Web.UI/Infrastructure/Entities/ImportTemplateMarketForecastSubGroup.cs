using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ImportTemplateMarketForecastSubGroup")]
    public class ImportTemplateMarketForecastSubGroup
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Company { get; set; }
        public string SubGroupNew { get; set; }
        public float Amount { get; set; }
    }
}
