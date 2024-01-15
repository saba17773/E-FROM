using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.DataAccess.Entities;

namespace Web.UI.DataAccess.Config
{
    public class HRMS_EmployeeConfig : IEntityTypeConfiguration<HRMS_Employee>
    {
        public void Configure(EntityTypeBuilder<HRMS_Employee> builder)
        {
            builder
                .HasMany(x => x.VaccineInjections)
                .WithOne(x => x.Employee)
                .HasForeignKey(x => x.EmployeeId)
                .HasPrincipalKey(x => x.EmployeeId);
        }
    }
}
