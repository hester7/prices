using Microsoft.EntityFrameworkCore;
using Prices.Persistence.EntityFramework;

namespace Prices.Downloader.Tests.Mocks;

public class MockDbContextFactory : IDbContextFactory<PricesContext>
{
    private readonly DbContextOptions<PricesContext> _options;

    public MockDbContextFactory()
    {
        _options = new DbContextOptionsBuilder<PricesContext>()
            .UseInMemoryDatabase(databaseName: "PricesDatabase" + Guid.NewGuid())
            .Options;

        var context = CreateDbContext();
        context.PriceIndexes.AddRange(SeedData.PriceIndexes);
        context.PriceMarkets.AddRange(SeedData.PriceMarkets);
        context.PriceTypes.AddRange(SeedData.PriceTypes);
        context.PricingNodes.AddRange(SeedData.PricingNodes);
        context.PricingNodeTypes.AddRange(SeedData.PricingNodeTypes);
        context.PricingNodeTypeMappings.AddRange(SeedData.PricingNodeTypeMappings);
        context.RegionalTransmissionOperators.AddRange(SeedData.RegionalTransmissionOperators);
        context.SaveChanges();
    }

    public PricesContext CreateDbContext() => new(_options);
}