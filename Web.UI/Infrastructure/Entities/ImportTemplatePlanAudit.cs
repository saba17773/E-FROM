using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ImportTemplatePlanAudit")]
    public class ImportTemplatePlanAuditTable
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string SubCode { get; set; }
        public string CompanyType { get; set; }
        public string ProductSales { get; set; }
        public string PlanMonth { get; set; }
        public string ActualMonth { get; set; }
    }
}
