using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ImportTemplateFCModel
    {
        [Ganss.Excel.Column(1)]
        public string GroupId { get; set; }

        [Ganss.Excel.Column(2)]
        public string CustGroup { get; set; }

        [Ganss.Excel.Column(3)]
        public int Year { get; set; }

        [Ganss.Excel.Column(4)]
        public string ItemId { get; set; }

        [Ganss.Excel.Column(5)]
        public string ItemName { get; set; }

        [Ganss.Excel.Column(6)]
        public string ProductGroup { get; set; }

        [Ganss.Excel.Column(7)]
        public string SubGroup { get; set; }

        [Ganss.Excel.Column(8)]
        public string ProductType { get; set; }

        [Ganss.Excel.Column(9)]
        public float QTY_01 { get; set; }

        [Ganss.Excel.Column(10)]
        public float Amount_01 { get; set; }

        [Ganss.Excel.Column(11)]
        public float QTY_02 { get; set; }

        [Ganss.Excel.Column(12)]
        public float Amount_02 { get; set; }

        [Ganss.Excel.Column(13)]
        public float QTY_03 { get; set; }

        [Ganss.Excel.Column(14)]
        public float Amount_03 { get; set; }
    }
}
