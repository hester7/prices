using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.EntityConfigs;

public class IntervalEndTimeUtcScalarConfig : IEntityTypeConfiguration<IntervalEndTimeUtcScalar>
{
    public void Configure(EntityTypeBuilder<IntervalEndTimeUtcScalar> entity)
    {
        entity.HasNoKey();
    }
}