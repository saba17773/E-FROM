using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_VenderProductType")]
    public class VenderProductType_TB
    {
        public int ID { get; set; }

        [StringLength(10)]
        public string CODE { get; set; }
        [Required]
        [StringLength(150)]
        public string DESCRIPTION { get; set; }

        [Required]
        public int ISACTIVE { get; set; }

    }
}
