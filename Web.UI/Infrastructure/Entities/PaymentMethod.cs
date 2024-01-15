using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_PaymentMethod")]
    public class PaymentMethodTable
    {
        public int Id { get; set; }
        public string MethodOfPayment { get; set; }
        public string MethodName { get; set; }
        public string DataAreaId { get; set; }
    }
}