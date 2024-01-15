using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("SalesQuotationTable")]
    public class MemoQATable
    {
        public string QuotationId { get; set; }
    }
}
