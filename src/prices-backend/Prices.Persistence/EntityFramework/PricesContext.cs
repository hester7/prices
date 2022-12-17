using Microsoft.EntityFrameworkCore;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework;

public class PricesContext : DbContext
{
    public PricesContext(DbContextOptions<PricesContext> options)
        : base(options)
    {
    }

    public DbSet<Price> Prices { get; set; } = null!;
    public DbSet<PriceIndex> PriceIndexes { get; set; } = null!;
    public DbSet<PriceMarket> PriceMarkets { get; set; } = null!;
    public DbSet<PriceType> PriceTypes { get; set; } = null!;
    public DbSet<PricesFile> PricesFiles { get; set; } = null!;
    public DbSet<PricingNode> PricingNodes { get; set; } = null!;
    public DbSet<PricingNodeType> PricingNodeTypes { get; set; } = null!;
    public DbSet<PricingNodeTypeMapping> PricingNodeTypeMappings { get; set; } = null!;
    public DbSet<RegionalTransmissionOperator> RegionalTransmissionOperators { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PricesContext).Assembly);
    }
}