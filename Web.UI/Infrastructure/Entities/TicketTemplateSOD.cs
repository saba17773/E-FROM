using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("SOD")]
    public class TicketTemplateSODTable
    {
        [DataType(DataType.Date)]
        // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AllocateMonth { get; set; }
        public float RequestQTY { get; set; }
        public float ConfirmQTY { get; set; }
        public string Custcode { get; set; }
        public string Itemid { get; set; }
        public float Cango { get; set; }
        public float Out { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int Id { get; set; }

    }
}