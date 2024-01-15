using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class UserGridModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int IsActive { get; set; }

        public string EmployeeId { get; set; }
        public string UserDomain { get; set; }
    }
}
