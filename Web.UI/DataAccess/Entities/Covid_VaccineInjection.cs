using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.DataAccess.Entities
{
    [Table("Covid_VaccineInjection")]
    public class Covid_VaccineInjection
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; }
        public int VaccineId { get; set; }
        public int VaccineStatusId { get; set; }
        public DateTime VaccineDate { get; set; }
        public int IsLatest { get; set; }

        public virtual Covid_VaccineStatus VaccineStatus { get; set; }
        public virtual HRMS_Employee Employee { get; set; }
        public virtual Covid_Vaccine Vaccine { get; set; }
    }
}
