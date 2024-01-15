using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_RequestStatus")]
    public class RequestStatusTable
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int CreditControl { get; set; }
    }
}
