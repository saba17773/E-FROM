using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class UserClaimsModel
    {
        [Required]
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }
        public string CompanyGroup { get; set; }
    }
}
