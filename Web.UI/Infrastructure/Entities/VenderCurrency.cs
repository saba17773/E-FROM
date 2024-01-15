using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_VenderCurrency")]
    public class VenderCurrency_TB
    {
        public int ID { get; set; }

        [StringLength(10)]
        public string CODE { get; set; }

        [StringLength(50)]
        public string DESCRIPTION { get; set; }

        public int ISACTIVE { get; set; }
    }
}
