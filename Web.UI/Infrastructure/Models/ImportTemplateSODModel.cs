using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ImportTemplateSODModel
    {
        [Ganss.Excel.Column(1)]
        public DateTime? AllocateMonth { get; set; }

        [Ganss.Excel.Column(2)]
        public float RequestQTY { get; set; }

        [Ganss.Excel.Column(3)]
        public float ConfirmQTY { get; set; }

        [Ganss.Excel.Column(4)]
        public string Custcode { get; set; }

        [Ganss.Excel.Column(5)]
        public string Itemid{ get; set; }

        [Ganss.Excel.Column(6)]
        public float Cango { get; set; }

        [Ganss.Excel.Column(7)]
        public float Out { get; set; }
        

    }
}
