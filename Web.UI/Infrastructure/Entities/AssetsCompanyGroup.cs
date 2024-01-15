using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_FixAssetsCompanyGroup")]
    public class CompanyGroupTable
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string GroupCompany { get; set; }
    }
}
