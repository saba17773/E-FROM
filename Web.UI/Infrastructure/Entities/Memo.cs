using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_Memo")]
    public class MemoTable
    {
        public int Id { get; set; }
        public string MemoNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime? MemoDate { get; set; }
        [Required]
        public int AttnId { get; set; }
        [Required]
        public int SubjectId { get; set; }
        [Required]
        public string CustomerCode { get; set; }
        [Required]
        public string Description { get; set; }
        public string Remark { get; set; }
        public string SO { get; set; }
        public string QA { get; set; }
        public string Enquiry { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
