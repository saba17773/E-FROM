using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ImportTemplateDeliveryTargetExcelModel
    {
        [Column(1)]
        public int Year { get; set; }

        [Column(2)]
        public int KPI { get; set; }
    }
}
