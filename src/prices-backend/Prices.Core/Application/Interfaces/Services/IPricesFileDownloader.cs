using NodaTime;
using Prices.Core.Application.Models;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;

namespace Prices.Core.Application.Interfaces.Services;

public interface IPricesFileDownloader : IServiceByRto
{
    FileFormats DailyPricesFileFormat { get; }

    Task<FilesToDownloadResult> GetFilesToDownload(PriceTypes priceType, PriceIndex priceIndex, FileFormats fileFormat,
        LocalDate startDate, LocalDate? endDate = null, IEnumerable<string>? nodes = null, CancellationToken cancellationToken = default);

    Task<DownloadPricesFileResult> DownloadPricesFileAsync(PricesFileToDownload fileToDownload, int retryAttempts = 2,
        int delayInSecondsBetweenRetryAttempts = 30, CancellationToken cancellationToken = default);

    Task<DownloadPricesIntervalsResult> DownloadPriceIntervalsAsync(PricesFileToDownload fileToDownload, int retryAttempts = 2,
        int delayInSecondsBetweenRetryAttempts = 30, CancellationToken cancellationToken = default);

    (LocalDate StartDate, LocalDate EndDate) GetDailyPricesDownloadDates(PriceMarkets priceIndexPriceMarketId);
}