using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_Employee")]
    public class EmployeeTable
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string NameEng { get; set; }
        public string Company { get; set; }
        public int PositionCode { get; set; }
        public string PositionName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int Status { get; set; }
        public string EmployeeIdOld { get; set; }
        public string Email { get; set; }
        public string Domain { get; set; }
        public int Approve1 { get; set; }
        public int Approve2 { get; set; }
    }
}
