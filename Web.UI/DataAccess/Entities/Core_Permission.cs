using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.DataAccess.Entities
{
    [Table("TB_Permission")]
    public class Core_Permission
    {
        public int Id { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        [Column("CapabilityId")]
        public string PermissionId { get; set; }

        [Required]
        [Column("PermissionType")]
        public string PermissionGroup { get; set; }
    }
}
