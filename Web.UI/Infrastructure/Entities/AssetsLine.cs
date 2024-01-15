using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_FixAssetsLine")]
    public class AssetsLineTable
    {
        public int Id { get; set; }
        public string AssetsNumber { get; set; }
        public int AssetsId { get; set; }
        public double Qty { get; set; }
        public string Description { get; set; }
        public string RoomName { get; set; }
        public string EmployeeName { get; set; }
    }
}
