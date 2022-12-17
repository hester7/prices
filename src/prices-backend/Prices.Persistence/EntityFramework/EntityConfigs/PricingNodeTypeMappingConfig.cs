using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.EntityConfigs
{
    public class PricingNodeTypeMappingConfig : IEntityTypeConfiguration<PricingNodeTypeMapping>
    {
        public void Configure(EntityTypeBuilder<PricingNodeTypeMapping> entity)
        {
            entity.HasKey(e => new { e.PricingNodeTypeId, e.RegionalTransmissionOperatorId });

            entity.HasIndex(e => e.RegionalTransmissionOperatorId, "IX_PricingNodeTypeMappings_RegionalTransmissionOperatorId");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity
                .HasData(SeedData.PricingNodeTypeMappings);
        }
    }
}