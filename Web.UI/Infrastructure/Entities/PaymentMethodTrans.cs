using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_PaymentMethodTrans")]
    public class PaymentMethodTransTable
    {
        public int Id { get; set; }
        public int CCId { get; set; }
        public int PaymentMethodId { get; set; }
        public string Remark { get; set; }
    }
}
