using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ImportTemplateTurnOverGroup")]
    public class ImportTemplateTurnOverGroupTable
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Group { get; set; }
        public float KPI { get; set; }
    }
}
