using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models.Import.TurnOver
{
    public class ImportTemplateTurnOverGroupExcel
    {
        [Column(1)]
        public int Year { get; set; }

        [Column(2)]
        public string Group { get; set; }

        [Column(3)]
        public float KPI { get; set; }
    }
}
