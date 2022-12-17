using NodaTime;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Domain.Enums;
using Prices.Downloader.Services.Services;

namespace Prices.Downloader.Tests.Mocks;

public class MockPricingNodesDownloaderFactory : IPricingNodesDownloaderFactory
{
    private readonly MockDbContextFactory _dbContextFactory;
    private readonly IClock _clock;

    public MockPricingNodesDownloaderFactory(IClock clock)
    {
        _dbContextFactory = new MockDbContextFactory();
        _clock = clock;
    }

    public IPricingNodesDownloader GetDownloaderByRto(Rtos rto) => new CaisoPricingNodesDownloader(_dbContextFactory, _clock);
}