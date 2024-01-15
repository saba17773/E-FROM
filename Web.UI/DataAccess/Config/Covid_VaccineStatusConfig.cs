using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.DataAccess.Entities;

namespace Web.UI.DataAccess.Config
{
    public class Covid_VaccineStatusConfig : IEntityTypeConfiguration<Covid_VaccineStatus>
    {
        public void Configure(EntityTypeBuilder<Covid_VaccineStatus> builder)
        {
            builder
                .HasMany(x => x.VaccineInjections)
                .WithOne(x => x.VaccineStatus)
                .HasForeignKey(x => x.VaccineStatusId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}
