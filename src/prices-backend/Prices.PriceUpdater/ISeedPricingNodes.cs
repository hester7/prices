namespace Prices.PriceUpdater;

public interface ISeedPricingNodes
{
    Task Run(CancellationToken cancellationToken = default);
}