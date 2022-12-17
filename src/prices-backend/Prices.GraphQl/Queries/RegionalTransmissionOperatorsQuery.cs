using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.GraphQl.DataLoaders;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class RegionalTransmissionOperatorsQuery
{
    [UseProjection]
    [UseFiltering]
    public IQueryable<RegionalTransmissionOperator> GetRegionalTransmissionOperators(PricesContext context) => context.RegionalTransmissionOperators;

    public async Task<RegionalTransmissionOperator?> GetRegionalTransmissionOperatorById(
        Rtos id,
        RegionalTransmissionOperatorByIdDataLoader rtoById,
        CancellationToken cancellationToken)
        => await rtoById.LoadAsync(id, cancellationToken);

    public async Task<IEnumerable<RegionalTransmissionOperator?>> GetRegionalTransmissionOperatorsById(
        Rtos[] ids,
        RegionalTransmissionOperatorByIdDataLoader rtoById,
        CancellationToken cancellationToken)
        => await rtoById.LoadAsync(ids, cancellationToken);
}