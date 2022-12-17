using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.EntityConfigs;

public class PriceTypeConfig : IEntityTypeConfiguration<PriceType>
{
    public void Configure(EntityTypeBuilder<PriceType> entity)
    {
        entity.Property(e => e.Name)
            .HasMaxLength(50)
            .IsUnicode(false);

        entity
            .HasData(SeedData.PriceTypes);
    }
}