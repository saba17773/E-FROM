using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("SALESTABLE")]
    public class MemoSOTable
    {
        public string SalesId { get; set; }
        public string QuotationId { get; set; }
        public string Dsg_EnquiryId { get; set; }
    }
}
