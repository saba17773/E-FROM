using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_TicketTemplateForcast")]
    public class TicketTemplateFCTable
    {
        public int Id { get; set; }
        public string GroupId { get; set; }
        public string CustGroup { get; set; }
        [Required]
        public int Year { get; set; }
        public int Month { get; set; }
        [Required]
        public int VersionId { get; set; }
        public string Quarter { get; set; }
        public string ProductGroup { get; set; }
        public string SubGroup { get; set; }
        public string ProductType { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public double QTY { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        [StringLength(100)]
        public string Remark { get; set; }
        public int CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public double QTYNext { get; set; }
        public double AmountNext { get; set; }

    }
}