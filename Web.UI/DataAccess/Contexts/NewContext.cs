using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.DataAccess.Config;
using Web.UI.DataAccess.Entities;

namespace Web.UI.DataAccess.Contexts
{
    public class NewContext : DbContext
    {
        public NewContext(DbContextOptions<NewContext> options) : base(options)
        {
        }

        public DbSet<HRMS_Employee> Employees { get; set; }
        public DbSet<Covid_VaccineStatus> VaccineStatuses { get; set; }
        public DbSet<Covid_VaccineInjection> VaccineInjections { get; set; }
        public DbSet<Covid_Vaccine> Vaccine { get; set; }
        public DbSet<Core_Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new HRMS_EmployeeConfig());
            builder.ApplyConfiguration(new Covid_VaccineInjectionConfig());
            builder.ApplyConfiguration(new Covid_VaccineStatusConfig());
            builder.ApplyConfiguration(new Covid_VaccineConfig());
        }
    }
}
