using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ImportTemplateSaving_ExchangeRateModel
    {
        [Column(1)]
        public int Year { get; set; }

        [Column(2)]
        public int Month { get; set; }

        [Column(3)]
        public string Currency { get; set; }

        [Column(4)]
        public float Rate { get; set; }
    }
}
