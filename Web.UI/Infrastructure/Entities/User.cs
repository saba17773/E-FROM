using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Entities
{
    [Table("TB_User")]
    public class UserTable
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        //[MinLength(6)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string Salt { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int IsActive { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        public string UserDomain { get; set; }
        public int CompanyGroup { get; set; }

    }
}
