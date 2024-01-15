using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_PromotionGroupCustomer")]
    public class PromotionGroupCustomerTable
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string ByType { get; set; }
    }
}