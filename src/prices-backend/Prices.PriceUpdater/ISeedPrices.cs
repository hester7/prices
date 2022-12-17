namespace Prices.PriceUpdater;

public interface ISeedPrices
{
    Task Run(int startYear, CancellationToken cancellationToken = default);
}