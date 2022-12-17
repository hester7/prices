using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using System.Runtime.CompilerServices;
using Prices.Core.Domain.Models;
using Prices.GraphQl.DataLoaders;

namespace Prices.GraphQl.Subscriptions;

[ExtendObjectType(OperationTypeNames.Subscription)]
public sealed class PricingNodeSubscriptions
{
    // Create an event stream
    public async IAsyncEnumerable<int> CurrentPriceChangeStreamAsync(
        [Service] ITopicEventReceiver receiver,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ISourceStream stream = await receiver.SubscribeAsync<string, int>(Constants.OnCurrentPriceChange, cancellationToken);

        await foreach (int pricingNodeId in stream.ReadEventsAsync().WithCancellation(cancellationToken))
        {
            yield return pricingNodeId;
        }
    }

    // Subscribe to the event stream
    [Subscribe(With = nameof(CurrentPriceChangeStreamAsync))]
    public async Task<PricingNode> OnCurrentPriceChangeAsync(
        PricingNodeByIdDataLoader dataLoader,
        [EventMessage] int pricingNodeId,
        CancellationToken cancellationToken)
        => await dataLoader.LoadAsync(pricingNodeId, cancellationToken);
}