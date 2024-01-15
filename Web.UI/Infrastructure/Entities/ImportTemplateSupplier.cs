using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ImportTemplateSupplier")]
    public class ImportTemplateSupplierTable
    {
        public int Id { get; set; }
        public int ImportId { get; set; }
        public int Year { get; set; }
        public int Period { get; set; }
        public string SupCode { get; set; }
        public float Quality { get; set; }
        public float Delivery { get; set; }
        public float Safety { get; set; }
        public string Company { get; set; }
        public string Grade { get; set; }
    }
}