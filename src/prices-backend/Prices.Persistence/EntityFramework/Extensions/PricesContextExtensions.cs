using Microsoft.EntityFrameworkCore;
using NodaTime;
using Prices.Core.Application.Models;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.Extensions;

public static class PricesContextExtensions
{
    public static async Task BulkSavePricesAsync(this PricesContext dbContext,
        PricesFileMetadata blobMetadata,
        string blobName,
        long fileSize,
        Instant now,
        IEnumerable<Price> prices,
        CancellationToken cancellationToken)
    {
        var pricesList = prices.ToList();
        if (!pricesList.Any())
            return;

        const int batchSize = 5_000;
        var batches = pricesList.Chunk(batchSize).ToList();
        foreach (var batch in batches)
        {
            // Build in delay to avoid database issues
            if (batches.Count > 1)
                await Task.Delay(Random.Shared.Next(3_000, 5_000), cancellationToken);

            await dbContext.Prices.BulkMergeAsync(batch.ToList(), cancellationToken: cancellationToken);
        }

        var startDate = pricesList.Min(p => p.IntervalStartTimeUtc);
        var endDate = pricesList.Max(p => p.IntervalEndTimeUtc);
        var pricesFile = blobMetadata.ToPricesFile(blobName, startDate, endDate, fileSize, now);

        var existingPricesFile = await dbContext.PricesFiles.SingleOrDefaultAsync(f => f.BlobName == blobName, cancellationToken: cancellationToken);
        if (existingPricesFile is not null)
        {
            existingPricesFile.StartDateUtc = pricesFile.StartDateUtc;
            existingPricesFile.EndDateUtc = pricesFile.EndDateUtc;
            existingPricesFile.FileSourceUrl = pricesFile.FileSourceUrl;
            existingPricesFile.DocumentId = pricesFile.DocumentId;
            existingPricesFile.FileSize = pricesFile.FileSize;
            existingPricesFile.CreatedAtUtc = pricesFile.CreatedAtUtc;
        }
        else
        {
            await dbContext.PricesFiles.AddAsync(pricesFile, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}