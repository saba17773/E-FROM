using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_Employee")]
    public class AssetsUser
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string NameThai { get; set; }
        public string NameEng { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
    }
}