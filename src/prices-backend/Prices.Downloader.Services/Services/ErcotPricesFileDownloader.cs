using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Prices.AzureBlobStorage;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Application.Models;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;

namespace Prices.Downloader.Services.Services;

public class ErcotPricesFileDownloader : PricesFileDownloaderBase, IPricesFileDownloader
{
    public ErcotPricesFileDownloader(IAzureBlobStorageClientFactory azureBlobStorageClientFactory, IDbContextFactory<PricesContext> contextFactory,
        IPricesFileProcessorFactory pricesFileProcessorFactory, ILogger<ErcotPricesFileDownloader> logger, IOptions<Settings> settings)
        : base(azureBlobStorageClientFactory, contextFactory, pricesFileProcessorFactory, logger, settings)
    {
    }

    public override Rtos RegionalTransmissionOperator => Rtos.ERCOT;
    public FileFormats DailyPricesFileFormat => FileFormats.XML;

    public async Task<FilesToDownloadResult> GetFilesToDownload(PriceTypes priceType, PriceIndex priceIndex, FileFormats fileFormat,
        LocalDate startDate, LocalDate? endDate = null, IEnumerable<string>? nodes = null, CancellationToken cancellationToken = default)
    {
        endDate ??= startDate;

        var requestUri = GetRequestUri(priceType, priceIndex);

        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var files = response.Content.ReadFromJsonAsync<ErcotPriceFilesList>(cancellationToken: cancellationToken).Result;
        if (files is null)
        {
            var errorMessagePrefix = $"{RegionalTransmissionOperator} {priceType} {priceIndex.PriceMarketId} {startDate:d}";
            var errors = new[] { $"{errorMessagePrefix}: No files returned by {requestUri}." };
            return new FilesToDownloadResult(false, Enumerable.Empty<PricesFileToDownload>(), errors);
        }

        var existingDocIds = new List<string>();
        if (priceType != PriceTypes.Historical)
        {
            var existingFiles = await GetExistingPricesFiles(RegionalTransmissionOperator, priceType, cancellationToken);
            existingDocIds = existingFiles.Select(f => f.DocumentId!).ToList();
        }

        var filteredFiles = new List<ErcotDocument>();
        for (var date = startDate; date <= endDate; date = date.PlusDays(1))
        {
            filteredFiles.AddRange(FilteredFiles(files, date, priceType, fileFormat, existingDocIds!));
        }

        var filesToDownload = ConvertToFilesToDownload(filteredFiles, priceType, priceIndex, fileFormat);
        return new FilesToDownloadResult(true, filesToDownload, Array.Empty<string>());
    }

    public (LocalDate StartDate, LocalDate EndDate) GetDailyPricesDownloadDates(PriceMarkets priceMarketId)
        => (LocalDate.FromDateTime(DateTime.Today.AddDays(-1)), LocalDate.FromDateTime(DateTime.Today.AddDays(1)));

    private static string GetRequestUri(PriceTypes priceType, PriceIndex priceIndex)
    {
        var reportTypeId = (priceType, priceIndex.PriceMarketId) switch
        {
            (PriceTypes.Daily, PriceMarkets.DAM) => 12331, // https://www.ercot.com/mp/data-products/data-product-details?id=NP4-190-CD
            (PriceTypes.Daily, PriceMarkets.RTM) => 12301, // https://www.ercot.com/mp/data-products/data-product-details?id=NP6-905-CD
            (PriceTypes.Historical, PriceMarkets.DAM) => 13060, // https://www.ercot.com/mp/data-products/data-product-details?id=NP4-180-ER
            (PriceTypes.Historical, PriceMarkets.RTM) => 13061, // https://www.ercot.com/mp/data-products/data-product-details?id=NP6-785-ER
            (PriceTypes.Current, PriceMarkets.RTM) => 12300, // https://www.ercot.com/mp/data-products/data-product-details?id=NP6-788-CD
            _ => throw new ArgumentOutOfRangeException(nameof(priceIndex.PriceMarketId), priceIndex.PriceMarketId, null)
        };

        return $"https://www.ercot.com/misapp/servlets/IceDocListJsonWS?reportTypeId={reportTypeId}";
    }

    private static List<ErcotDocument> FilteredFiles(ErcotPriceFilesList files, LocalDate date, PriceTypes priceType,
        FileFormats fileFormat, IEnumerable<string> existingDocIds)
    {
        switch (priceType)
        {
            case PriceTypes.Historical:
                return files.ListDocsByRptTypeRes.DocumentList.Select(d => d.Document)
                    .Where(d => d.ConstructedName.ToLowerInvariant().EndsWith($"{date:yyyy}.zip".ToLowerInvariant()) &&
                                !existingDocIds.Contains(d.DocID))
                    .ToList();
            case PriceTypes.Daily:
                return files.ListDocsByRptTypeRes.DocumentList.Select(d => d.Document)
                    .Where(d => d.ConstructedName.ToLowerInvariant().EndsWith($"{fileFormat}.zip".ToLowerInvariant()) &&
                                d.ConstructedName.Contains($"{date:yyyyMMdd}") &&
                                !existingDocIds.Contains(d.DocID))
                    .ToList();
            case PriceTypes.Current:
                return files.ListDocsByRptTypeRes.DocumentList.Select(d => d.Document)
                    .Where(d => d.ConstructedName.ToLowerInvariant().EndsWith($"{fileFormat}.zip".ToLowerInvariant()) &&
                                d.ConstructedName.Contains($"{date:yyyyMMdd}") &&
                                !existingDocIds.Contains(d.DocID))
                    .OrderByDescending(d => d.PublishDate)
                    .Take(1)
                    .ToList();
            default:
                throw new ArgumentOutOfRangeException(nameof(priceType), priceType, null);
        }
    }

    private static IEnumerable<PricesFileToDownload> ConvertToFilesToDownload(List<ErcotDocument> filteredFiles, PriceTypes priceType,
        PriceIndex priceIndex, FileFormats fileFormat)
        => filteredFiles.Select(f => new PricesFileToDownload(f.DownloadUrl, priceType, priceIndex, fileFormat, f.DocID));
}