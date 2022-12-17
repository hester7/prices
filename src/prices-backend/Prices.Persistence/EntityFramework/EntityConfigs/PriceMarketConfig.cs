using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.EntityConfigs
{
    public class PriceMarketConfig : IEntityTypeConfiguration<PriceMarket>
    {
        public void Configure(EntityTypeBuilder<PriceMarket> entity)
        {
            entity.HasIndex(e => e.Abbreviation, "IX_PriceMarkets_Abbreviation")
                .IsUnique();

            entity.HasIndex(e => e.Name, "IX_PriceMarkets_Name")
                .IsUnique();

            entity.Property(e => e.Abbreviation)
                .HasMaxLength(3)
                .IsUnicode(false);

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity
                .HasData(SeedData.PriceMarkets);
        }
    }
}