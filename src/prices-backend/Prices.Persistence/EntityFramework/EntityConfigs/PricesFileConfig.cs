using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.EntityConfigs;

public class PricesFileConfig : IEntityTypeConfiguration<PricesFile>
{
    public void Configure(EntityTypeBuilder<PricesFile> entity)
    {
        entity.HasKey(e => e.BlobName);

        entity.HasIndex(e => e.PriceTypeId, "IX_PricesFiles_PriceTypeId");

        entity.Property(e => e.BlobName)
            .HasMaxLength(200)
            .IsUnicode(false);

        entity.Property(e => e.DocumentId)
            .HasMaxLength(50)
            .IsUnicode(false);

        entity.Property(e => e.FileName)
            .HasMaxLength(100)
            .IsUnicode(false);

        entity.Property(e => e.FileSourceUrl)
            .HasMaxLength(2048)
            .IsUnicode(false);

        entity.Property(e => e.VirtualFolder)
            .HasMaxLength(100)
            .IsUnicode(false);

        entity.HasOne(d => d.PriceIndex)
            .WithMany(p => p.PricesFiles)
            .HasForeignKey(d => d.PriceIndexId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PricesFiles_PriceIndexes");

        entity.HasOne(d => d.PriceType)
            .WithMany(p => p.PricesFiles)
            .HasForeignKey(d => d.PriceTypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PricesFiles_PriceTypes");
    }
}