using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ImportTemplateDeliveryTarget")]
    public class ImportTemplateDeliveryTargetTable
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int KPI { get; set; }
    }
}
