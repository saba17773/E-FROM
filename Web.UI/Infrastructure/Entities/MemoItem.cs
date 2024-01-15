using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_MemoItem")]
    public class MemoItemTable
    {
        public int Id { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Double Qty { get; set; }
        public string Unit { get; set; }
        public string SO { get; set; }
        public string QA { get; set; }
        public string Enquiry { get; set; }
        public int Cancel { get; set; }
        public int Produced { get; set; }
        public int NoProduced { get; set; }
        public int MemoId { get; set; }
    }
}
