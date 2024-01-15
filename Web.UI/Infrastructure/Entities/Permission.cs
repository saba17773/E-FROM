using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_Permission")]
    public class PermissionTable
    {
        public int Id { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string CapabilityId { get; set; }

        [Required]
        [StringLength(50)]
        public string PermissionType { get; set; }
    }
}
