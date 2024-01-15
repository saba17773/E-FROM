using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_FixAssetsMapHeadApprove")]
    public class AssetsMapHeadTable
    {
        public int Id { get; set; }

        [Required]
        public string Mg_EmpId { get; set; }
        public string Director_EmpId { get; set; }
    }
}
