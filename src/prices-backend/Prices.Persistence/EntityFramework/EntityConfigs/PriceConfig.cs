using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.EntityConfigs
{
    public class PriceConfig : IEntityTypeConfiguration<Price>
    {
        public void Configure(EntityTypeBuilder<Price> entity)
        {
            entity.HasKey(e => new { e.PriceIndexId, e.PricingNodeId, e.IntervalEndTimeUtc });

            entity.HasIndex(e => new { e.PricingNodeId, e.IntervalEndTimeUtc }, "IX_Prices_PricingNodeId_IntervalEndTimeUtc")
                .IncludeProperties(p => new { p.PriceIndexId, p.LmpPrice });

            entity.HasIndex(e => new { e.PriceIndexId, e.IntervalEndTimeUtc }, "IX_Prices_PriceIndexId_IntervalEndTimeUtc")
                .IncludeProperties(p => new { p.PricingNodeId, p.LmpPrice });

            entity.Property(e => e.CongestionPrice).HasColumnType("decimal(19, 9)");

            entity.Property(e => e.EnergyPrice).HasColumnType("decimal(19, 9)");

            entity.Property(e => e.LmpPrice).HasColumnType("decimal(19, 9)");

            entity.Property(e => e.LossPrice).HasColumnType("decimal(19, 9)");

            entity.Property(e => e.PricingNodeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        }
    }
}