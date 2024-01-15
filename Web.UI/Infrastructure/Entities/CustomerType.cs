using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_CustomerType")]
    public class CustomerTypeTable
    {
        public int Id { get; set; }
        public string TypeCode { get; set; }
        public string Description { get; set; }
        public string DataAreaId { get; set; }
    }
}
