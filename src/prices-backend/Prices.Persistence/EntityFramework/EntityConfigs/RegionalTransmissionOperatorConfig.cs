using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.EntityConfigs
{
    public class RegionalTransmissionOperatorConfig : IEntityTypeConfiguration<RegionalTransmissionOperator>
    {
        public void Configure(EntityTypeBuilder<RegionalTransmissionOperator> entity)
        {
            entity.HasIndex(e => e.LegalName, "IX_RegionalTransmissionOperators_LegalName")
                .IsUnique();

            entity.HasIndex(e => e.Name, "IX_RegionalTransmissionOperators_Name")
                .IsUnique();

            entity.Property(e => e.LegalName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity
                .HasData(SeedData.RegionalTransmissionOperators);
        }
    }
}