using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_PaymentTerm")]
    public class PaymentTermTable
    {
        public int Id { get; set; }
        public string PaymentTermId { get; set; }
        public string Description { get; set; }
        public string DataAreaId { get; set; }
        public int ByDOM { get; set; }
        public int ByOVS { get; set; }
    }
}