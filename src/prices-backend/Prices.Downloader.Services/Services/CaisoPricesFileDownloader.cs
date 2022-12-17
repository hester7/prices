using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Prices.AzureBlobStorage;
using Prices.Core.Application.Extensions;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Application.Models;
using Prices.Core.Application.XsdClasses.Report.V1;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;

namespace Prices.Downloader.Services.Services
{
    public class CaisoPricesFileDownloader : PricesFileDownloaderBase, IPricesFileDownloader
    {
        public CaisoPricesFileDownloader(IAzureBlobStorageClientFactory azureBlobStorageClientFactory, IDbContextFactory<PricesContext> contextFactory,
            IPricesFileProcessorFactory pricesFileProcessorFactory, ILogger<CaisoPricesFileDownloader> logger, IOptions<Settings> settings)
            : base(azureBlobStorageClientFactory, contextFactory, pricesFileProcessorFactory, logger, settings)
        {
        }

        public override Rtos RegionalTransmissionOperator => Rtos.CAISO;
        public FileFormats DailyPricesFileFormat => FileFormats.XML;

        protected override string FormatFileName(PricesFileToDownload fileToDownload, string fileName)
        {
            var market = fileToDownload.PriceMarketId.ToString();
            var fileNamePrefix = fileName.Split(market)[0];
            var fileExtension = Path.GetExtension(fileName);
            return Path.ChangeExtension($"{fileNamePrefix}{market}", fileExtension);
        }

        protected async Task<List<PriceIndex>> GetDefaultPriceIndexesToDownloadPricesFor(Rtos rto, CancellationToken cancellationToken)
        {
            await using var context = await ContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.PriceIndexes.Where(i => i.RegionalTransmissionOperatorId == rto).AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<FilesToDownloadResult> GetFilesToDownload(PriceTypes priceType, PriceIndex priceIndex, FileFormats _,
            LocalDate startDate, LocalDate? endDate = null, IEnumerable<string>? nodes = null, CancellationToken cancellationToken = default)
        {
            endDate ??= startDate.PlusDays(1);

            var startDateTime = $"{startDate.ToDateTimeUnspecified().InZone(Rtos.CAISO):yyyyMMdd'T'HH':'mm'-0000'}";
            var endDateTime = $"{endDate.Value.ToDateTimeUnspecified().InZone(Rtos.CAISO):yyyyMMdd'T'HH':'mm'-0000'}";

            string urlParameters;

            var nodesList = nodes?.ToList();
            switch (priceType)
            {
                case PriceTypes.Historical:
                case PriceTypes.Daily:
                    switch (priceIndex.PriceMarketId)
                    {
                        case PriceMarkets.DAM:
                            {
                                if (nodesList is null || !nodesList.Any())
                                {
                                    var pricingNodes = await GetDefaultPricingNodesToDownloadPricesFor(RegionalTransmissionOperator, cancellationToken);
                                    nodesList = pricingNodes.Select(pn => pn.Name).ToList();
                                }
                                urlParameters = $"queryname={OASISReportType.PRC_LMP}&market_run_id={OASISMarketType.DAM}" +
                                                $"&node={string.Join(",", nodesList)}";
                            }
                            break;
                        case PriceMarkets.RTM:
                            {
                                if (nodesList is null || !nodesList.Any())
                                {
                                    var pricingNodes = await GetDefaultPricingNodesToDownloadPricesFor(RegionalTransmissionOperator, cancellationToken);
                                    nodesList = pricingNodes.Select(pn => pn.Name).ToList();
                                }
                                urlParameters = $"queryname={OASISReportType.PRC_INTVL_LMP}&market_run_id={OASISMarketType.RTM}" +
                                                $"&node={string.Join(",", nodesList)}";
                            }
                            break;
                        //case PriceMarkets.FMM:
                        //    {
                        //        if (nodesList is null || !nodesList.Any())
                        //        {
                        //            var pricingNodes = await GetDefaultPricingNodesToDownloadPricesFor(RegionalTransmissionOperator, cancellationToken);
                        //            nodesList = pricingNodes.Select(pn => pn.Name).ToList();
                        //        }
                        //        urlParameters = $"queryname={OASISReportType.PRC_RTPD_LMP}&market_run_id={OASISMarketType.RTPD}" +
                        //                        $"&node={string.Join(",", nodesList)}";
                        //    }
                        //    break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(priceIndex.PriceMarketId), priceIndex.PriceMarketId, null);
                    }
                    break;
                case PriceTypes.Current:
                    if (nodesList is null || !nodesList.Any())
                    {
                        var pricingNodes = await GetDefaultPricingNodesToDownloadPricesFor(RegionalTransmissionOperator, cancellationToken);
                        nodesList = pricingNodes.Select(pn => pn.Name).ToList();
                    }
                    urlParameters = $"queryname={OASISReportType.PRC_CURR_LMP}" +
                                    $"&node={string.Join(",", nodesList)}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(priceType), priceType, null);
            }

            var requestUri = $"http://oasis.caiso.com/oasisapi/SingleZip?{urlParameters}&startdatetime={startDateTime}&enddatetime={endDateTime}&version=1";

            // CSV: &resultformat=6

            var filesToDownload = new[] { new PricesFileToDownload(requestUri, priceType, priceIndex, FileFormats.XML) };
            return new FilesToDownloadResult(true, filesToDownload, Array.Empty<string>());
        }

        public (LocalDate StartDate, LocalDate EndDate) GetDailyPricesDownloadDates(PriceMarkets priceMarketId)
        {
            switch (priceMarketId)
            {
                case PriceMarkets.DAM:
                    return (LocalDate.FromDateTime(DateTime.Today), LocalDate.FromDateTime(DateTime.Today.AddDays(2)));
                case PriceMarkets.RTM:
                //case PriceMarkets.FMM:
                    return (LocalDate.FromDateTime(DateTime.Today.AddDays(-1)), LocalDate.FromDateTime(DateTime.Today.AddDays(1)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(priceMarketId), priceMarketId, null);
            }
        }
    }
}
