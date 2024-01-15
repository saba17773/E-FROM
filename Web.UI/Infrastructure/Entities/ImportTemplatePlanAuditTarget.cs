using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ImportTemplatePlanAuditTarget")]
    public class ImportTemplatePlanAuditTargetTable
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Target { get; set; }
    }
}
