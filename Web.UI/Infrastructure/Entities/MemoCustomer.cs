using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_MemoCustomer")]
    public class MemoCustomerTable
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string SalesId { get; set; }
        public string QuatationId { get; set; }
        public string ENQUIRY { get; set; }
    }
}
