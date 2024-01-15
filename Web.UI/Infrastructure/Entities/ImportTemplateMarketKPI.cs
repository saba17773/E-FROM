using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ImportTemplateMarketKPI")]
    public class ImportTemplateMarketKPI
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Company { get; set; }
        public string SubGroupNew { get; set; }
        public float KPI { get; set; }
    }
}
