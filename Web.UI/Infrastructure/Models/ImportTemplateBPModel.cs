using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ImportTemplateBPModel
    {
        [Ganss.Excel.Column(1)]
        public string GroupId { get; set; }

        [Ganss.Excel.Column(2)]
        public int Year { get; set; }

        [Ganss.Excel.Column(3)]
        public int Month { get; set; }

        [Ganss.Excel.Column(4)]
        public string CustomerCode { get; set; }

        [Ganss.Excel.Column(5)]
        public string CustomerName { get; set; }

        [Ganss.Excel.Column(6)]
        public string ItemId { get; set; }

        [Ganss.Excel.Column(7)]
        public string ItemName { get; set; }

        [Ganss.Excel.Column(8)]
        public string ProductGroup { get; set; }

        [Ganss.Excel.Column(9)]
        public string SubGroup { get; set; }

        [Ganss.Excel.Column(10)]
        public string ProductType { get; set; }

        [Ganss.Excel.Column(11)]
        public float QTY { get; set; }

        [Ganss.Excel.Column(12)]
        public float Amount { get; set; }

    }
}
