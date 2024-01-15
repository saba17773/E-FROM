using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.DataAccess.Entities;

namespace Web.UI.DataAccess.Config
{
    public class Covid_VaccineConfig : IEntityTypeConfiguration<Covid_Vaccine>
    {
        public void Configure(EntityTypeBuilder<Covid_Vaccine> builder)
        {
            builder
                .HasMany(x => x.VaccineInjections)
                .WithOne(x => x.Vaccine)
                .HasForeignKey(x => x.VaccineId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}
