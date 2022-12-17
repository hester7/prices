using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.EntityConfigs
{
    public class PricingNodeConfig : IEntityTypeConfiguration<PricingNode>
    {
        public void Configure(EntityTypeBuilder<PricingNode> entity)
        {
            entity.HasIndex(e => e.PricingNodeTypeId, "IX_PricingNodes_PricingNodeTypeId");

            entity.HasIndex(e => new { e.RegionalTransmissionOperatorId, e.DisplayName }, "IX_PricingNodes_RegionalTransmissionOperatorId_DisplayName")
                .IsUnique()
                .HasFilter("('DisplayName' IS NOT NULL)");

            entity.HasIndex(e => new { e.RegionalTransmissionOperatorId, e.Name }, "IX_PricingNodes_RegionalTransmissionOperatorId_Name")
                .IsUnique();

            entity.Property(e => e.Change24Hour).HasColumnType("decimal(19, 9)");

            entity.Property(e => e.CurrentPrice).HasColumnType("decimal(19, 9)");

            entity.Property(e => e.DisplayName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.PricingNodeType)
                .WithMany(p => p.PricingNodes)
                .HasForeignKey(d => d.PricingNodeTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PricingNodes_PricingNodeTypes");

            entity.HasOne(d => d.RegionalTransmissionOperator)
                .WithMany(p => p.PricingNodes)
                .HasForeignKey(d => d.RegionalTransmissionOperatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PricingNodes_RegionalTransmissionOperators");

            entity
                .HasData(SeedData.PricingNodes);
        }
    }
}