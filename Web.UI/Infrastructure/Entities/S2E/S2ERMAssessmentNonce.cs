using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities.S2E
{
    [Table("TB_S2ERMAssessmentNonce")]
    public class S2ERMAssessmentNonce_TB
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string NONCEKEY { get; set; }

        [Required]
        public DateTime? CREATEDATE { get; set; }

        [Required]
        public DateTime? EXPIREDATE { get; set; }

        [Required]
        public int ISUSED { get; set; }
    }
}
