using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_Unit")]
    public class Unit_TB
    {
        public int ID { get; set; }
        public string UNIT { get; set; }
    }
}
