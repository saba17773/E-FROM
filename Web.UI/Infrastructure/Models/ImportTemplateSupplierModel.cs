using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ImportTemplateSupplierModel
    {
        [Ganss.Excel.Column(1)]
        public int Year { get; set; }

        [Ganss.Excel.Column(2)]
        public int Period { get; set; }

        [Ganss.Excel.Column(3)]
        public string SupCode { get; set; }

        [Ganss.Excel.Column(4)]
        public float Quality { get; set; }

        [Ganss.Excel.Column(5)]
        public float Delivery { get; set; }

        [Ganss.Excel.Column(6)]
        public float Safety { get; set; }

        [Ganss.Excel.Column(7)]
        public string Company { get; set; }
    }
}
