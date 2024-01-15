using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ImportTemplateMarketSubGroup")]
    public class ImportTemplateMarketSubgroup
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Company { get; set; }
        public string Type { get; set; }
        public float Amount { get; set; }
        public string SubGroupNew { get; set; }
    }
}
