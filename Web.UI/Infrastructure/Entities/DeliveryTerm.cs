using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_DeliveryTerm")]
    public class DeliveryTermTable
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Txt { get; set; }
        public string DataAreaId { get; set; }
    }
}
