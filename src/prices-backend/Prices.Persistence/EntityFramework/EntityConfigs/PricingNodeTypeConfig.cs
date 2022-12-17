using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.EntityConfigs
{
    public class PricingNodeTypeConfig : IEntityTypeConfiguration<PricingNodeType>
    {
        public void Configure(EntityTypeBuilder<PricingNodeType> entity)
        {
            entity.HasIndex(e => e.Name, "IX_PricingNodeTypes_Name")
                .IsUnique();

            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity
                .HasData(SeedData.PricingNodeTypes);
        }
    }
}