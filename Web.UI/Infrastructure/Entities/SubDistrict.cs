using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_SubDistricts")]
    public class SubDistrictTable
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string NameInThai { get; set; }
        public string NameInEnglish { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int DistrictId { get; set; }
        public int ZipCode { get; set; }
    }
}
