using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_FixAssetsLocation")]
    public class AssetsLocationTable
    {
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public string Location { get; set; }
        public string Floor { get; set; }
        public string Room { get; set; }

        public string EquipmentUse { get; set; }
    }
}
