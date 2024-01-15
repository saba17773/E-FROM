using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.ViewModels.User
{
    public class FormEditUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string EmployeeId { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int IsActive { get; set; }
        public string UserDomain { get; set; }
        public int CompanyGroup { get; set; }
    }
}
