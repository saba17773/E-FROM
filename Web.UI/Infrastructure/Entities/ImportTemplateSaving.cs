using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_ImportTemplateSaving")]
    public class ImportTemplateSavingTable
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string SubGroupType { get; set; }
        public string Company { get; set; }
        public string ItemGroup { get; set; }
        public string ItemSubGroup { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string UOM { get; set; }
        public string CUR { get; set; }
        public string NewNegotiate { get; set; }
        public string Supplier1 { get; set; }
        public float LatestPrice { get; set; }
        public float NewPrice { get; set; }
        public string Supplier2 { get; set; }
        public float Saving { get; set; }
        public float Jan { get; set; }
        public float Feb { get; set; }
        public float Mar { get; set; }
        public float Apr { get; set; }
        public float May { get; set; }
        public float Jun { get; set; }
        public float Jul { get; set; }
        public float Aug { get; set; }
        public float Sep { get; set; }
        public float Oct { get; set; }
        public float Nov { get; set; }
        public float Dec { get; set; }
        public float TotalQTY { get; set; }
        public float TotalCostSaving { get; set; }
        public float CS_Jan { get; set; }
        public float CS_Feb { get; set; }
        public float CS_Mar { get; set; }
        public float CS_Apr { get; set; }
        public float CS_May { get; set; }
        public float CS_Jun { get; set; }
        public float CS_Jul { get; set; }
        public float CS_Aug { get; set; }
        public float CS_Sep { get; set; }
        public float CS_Oct { get; set; }
        public float CS_Nov { get; set; }
        public float CS_Dec { get; set; }
    }
}