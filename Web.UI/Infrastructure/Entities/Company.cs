using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_Company")]
    public class Company
    {
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
