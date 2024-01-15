using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.DataAccess.Entities;

namespace Web.UI.DataAccess.Config
{
    public class Covid_VaccineInjectionConfig : IEntityTypeConfiguration<Covid_VaccineInjection>
    {
        public void Configure(EntityTypeBuilder<Covid_VaccineInjection> builder)
        {
            builder
                .HasOne(x => x.VaccineStatus)
                .WithMany(x => x.VaccineInjections)
                .HasForeignKey(x => x.VaccineStatusId)
                .HasPrincipalKey(x => x.Id);

            builder
                .HasOne(x => x.Employee)
                .WithMany(x => x.VaccineInjections)
                .HasForeignKey(x => x.EmployeeId)
                .HasPrincipalKey(x => x.EmployeeId);

            builder
                .HasOne(x => x.Vaccine)
                .WithMany(x => x.VaccineInjections)
                .HasForeignKey(x => x.VaccineId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}
