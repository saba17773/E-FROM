using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ImportTemplateStockAcc")]
    public class ImportTemplateStockAccTable
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public float StockAcc { get; set; }
        public string Company { get; set; }
    }
}
