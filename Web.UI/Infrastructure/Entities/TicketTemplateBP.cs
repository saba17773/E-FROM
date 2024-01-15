using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_TicketTemplateBP")]
    public class TicketTemplateBPTable
    {
        public int Id { get; set; }
        public string GroupId { get; set; }
        [Required]
        public int Year { get; set; }
        public int Month { get; set; }
        [Required]
        public int VersionId { get; set; }
        [Required]
        public int Type { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ProductGroup { get; set; }
        public string SubGroup { get; set; }
        public string ProductType { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public float QTY { get; set; }
        public float Amount { get; set; }
        public string Status { get; set; }
        [StringLength(100)]
        public string Remark { get; set; }
        public int CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

    }
}