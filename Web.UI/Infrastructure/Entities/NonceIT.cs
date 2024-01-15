using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_NonceIT")]
    public class NonceITTable
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NonceKey { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime ExpireDate { get; set; }

        [Required]
        public int IsUsed { get; set; }
    }
}
