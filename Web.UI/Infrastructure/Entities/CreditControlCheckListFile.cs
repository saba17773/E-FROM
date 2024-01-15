using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_CreditControlCheckListFile")]
    public class CreditControlCheckListFileTable
    {
        public int Id { get; set; }
        public int CCId { get; set; }
        public int FileNo { get; set; }
    }
}
