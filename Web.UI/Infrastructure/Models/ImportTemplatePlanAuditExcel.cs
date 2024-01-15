using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ImportTemplatePlanAuditExcel
    {
        [Column(1)]
        public int Year { get; set; }

        [Column(2)]
        public string SubCode { get; set; }

        [Column(3)]
        public string CompanyType { get; set; }

        [Column(4)]
        public string ProductSales { get; set; }

        [Column(5)]
        public string PlanMonth { get; set; }

        [Column(6)]
        public string ActualMonth { get; set; }
    }
}
