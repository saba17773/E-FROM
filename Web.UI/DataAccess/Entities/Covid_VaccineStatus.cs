using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.DataAccess.Entities
{
    [Table("Covid_VaccineStatus")]
    public class Covid_VaccineStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IsActive { get; set; }

        public virtual ICollection<Covid_VaccineInjection> VaccineInjections { get; set; }
    }
}
