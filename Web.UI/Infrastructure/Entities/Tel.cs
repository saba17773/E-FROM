using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_FixAssetsTel")]
    public class TelTable
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string TelNumber { get; set; }
    }
}
