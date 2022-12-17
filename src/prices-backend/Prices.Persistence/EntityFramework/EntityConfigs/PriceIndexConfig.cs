using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.EntityConfigs
{
    public class PriceIndexConfig : IEntityTypeConfiguration<PriceIndex>
    {
        public void Configure(EntityTypeBuilder<PriceIndex> entity)
        {
            entity.HasIndex(e => e.Name, "IX_PriceIndexes_Name")
                .IsUnique();

            entity.HasIndex(e => e.PriceMarketId, "IX_PriceIndexes_PriceMarketId");

            entity.HasIndex(e => e.RegionalTransmissionOperatorId, "IX_PriceIndexes_RegionalTransmissionOperatorId");

            entity.HasIndex(e => new { e.RegionalTransmissionOperatorId, e.PriceMarketId }, "IX_PriceIndexes_RegionalTransmissionOperatorId_PriceMarketId");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.PriceMarket)
                .WithMany(p => p.PriceIndexes)
                .HasForeignKey(d => d.PriceMarketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PriceIndexes_PriceMarkets");

            entity.HasOne(d => d.RegionalTransmissionOperator)
                .WithMany(p => p.PriceIndexes)
                .HasForeignKey(d => d.RegionalTransmissionOperatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PriceIndexes_RegionalTransmissionOperators");

            entity
                .HasData(SeedData.PriceIndexes);
        }
    }
}