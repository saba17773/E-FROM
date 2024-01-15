using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_CustomerTypeByProductTrans")]
    public class CustomerTypeByProductTransTable
    {
        public int Id { get; set; }
        public int CCId { get; set; }
        public int CustomerByProductId { get; set; }
        public string CustomerCode { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
