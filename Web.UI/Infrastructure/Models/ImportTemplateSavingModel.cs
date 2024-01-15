using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ImportTemplateSavingModel
    {
        [Column(1)]
        public string SubGroupType { get; set; }

        [Column(2)]
        public int Year { get; set; }

        [Column(3)]
        public string Company { get; set; }

        [Column(4)]
        public string ItemGroup { get; set; }

        [Column(5)]
        public string ItemSubGroup { get; set; }

        [Column(6)]
        public string ItemCode { get; set; }

        [Column(7)]
        public string ItemDescription { get; set; }

        [Column(8)]
        public string UOM { get; set; }

        [Column(9)]
        public string CUR { get; set; }

        [Column(10)]
        public string NewNegotiate { get; set; }

        [Column(11)]
        public string Supplier1 { get; set; }

        [Column(12)]
        public float LatestPrice { get; set; }

        [Column(13)]
        public float NewPrice { get; set; }

        [Column(14)]
        public string Supplier2 { get; set; }

        [Column(15)]
        public float Jan { get; set; }

        [Column(16)]
        public float Feb { get; set; }

        [Column(17)]
        public float Mar { get; set; }

        [Column(18)]
        public float Apr { get; set; }

        [Column(19)]
        public float May { get; set; }

        [Column(20)]
        public float Jun { get; set; }

        [Column(21)]
        public float Jul { get; set; }

        [Column(22)]
        public float Aug { get; set; }

        [Column(23)]
        public float Sep { get; set; }

        [Column(24)]
        public float Oct { get; set; }

        [Column(25)]
        public float Nov { get; set; }

        [Column(26)]
        public float Dec { get; set; }
    }
}
