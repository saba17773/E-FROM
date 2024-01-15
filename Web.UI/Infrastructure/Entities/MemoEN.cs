using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("DSG_EnquiryLine")]
    public class MemoENTable
    {
        public string Dsg_EnquiryId { get; set; }
        public string SalesIdRef { get; set; }
    }
}
