using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_FixAssetsApproveMapping")]
    public class AssetsApproveMappingTable
    {
        public int Id { get; set; }
        public string CCType { get; set; }

        [Required]
        public int? ApproveMasterId { get; set; }
        [Required]
        public int? CreateBy { get; set; }
        public string TypeProduct { get; set; }
        public int CompanyId { get; set; }
    }
}
