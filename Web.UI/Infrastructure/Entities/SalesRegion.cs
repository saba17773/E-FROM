using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_SalesRegion")]
    public class SalesRegionTable
    {
        public int Id { get; set; }
        public string DSG_SALESREGIONID { get; set; }
        public string DSG_SALESREGIONNAME { get; set; }
        public string DATAAREAID { get; set; }
    }
}