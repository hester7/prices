using Prices.Core.Application.Models;
using Prices.Core.Application.XsdClasses.Report.V1;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using System.Xml.Serialization;
using NodaTime;
using Microsoft.Extensions.Logging;
using Prices.Core.Application.Extensions;
using Prices.Core.Application.Interfaces.Services;

namespace Prices.Downloader.Services.Services
{
    public class CaisoPricesXmlFileProcessor : IPricesFileProcessor
    {
        private readonly IClock _clock;
        private readonly ILogger<CaisoPricesXmlFileProcessor> _logger;

        public CaisoPricesXmlFileProcessor(IClock clock, ILogger<CaisoPricesXmlFileProcessor> logger)
        {
            _clock = clock;
            _logger = logger;
        }

        public Rtos RegionalTransmissionOperator => Rtos.CAISO;

        public FileFormats FileFormat => FileFormats.XML;

        public FileProcessorResult ProcessPrices(Stream blob, Rtos regionalTransmissionOperatorId, PriceMarkets priceMarketId,
            PriceIndexes priceIndexId, PriceTypes priceTypeId, IEnumerable<PricingNode> pricingNodes)
        {
            OASISReport file;
            try
            {
                file = MapFile(blob);
            }
            catch (Exception ex)
            {
                return new FileProcessorResult(false, Array.Empty<Price>(), new[] { ex.Message }, Array.Empty<string>());
            }
            var valid = Validate(file, out var errors, out var warnings);
            if (!valid)
                return new FileProcessorResult(false, Array.Empty<Price>(), errors, warnings);

            var prices = ConvertToPrices(file, priceIndexId, pricingNodes).ToList();
            _logger.LogInformation("Processed prices for {rto} {priceMarketId} {minDate}",
                regionalTransmissionOperatorId, priceMarketId, $"{prices.Min(p => p.IntervalStartTimeUtc):%yyyy-MM-dd}");
            return new FileProcessorResult(true, prices, Array.Empty<string>(), Array.Empty<string>());
        }

        private static OASISReport MapFile(Stream blob)
        {
            var serializer = new XmlSerializer(typeof(OASISReport));
            return (OASISReport)serializer.Deserialize(blob)!;
        }

        private static bool Validate(OASISReport file, out IEnumerable<string> errors, out IEnumerable<string> warnings)
        {
            errors = new List<string>();
            warnings = new List<string>();

            if (file.MessagePayload.RTO.ERROR is null)
                return true;

            if (file.MessagePayload.RTO.ERROR[0].ERR_DESC == OASISErrDescription.Nodatareturnedforthespecifiedselection)
            {
                warnings = new List<string>
                {
                    $"{file.MessagePayload.RTO.ERROR[0].ERR_CODE!.ToDescription()}: {file.MessagePayload.RTO.ERROR[0].ERR_DESC!.ToDescription()}"
                };
                return false;
            }

            errors = file.MessagePayload.RTO.ERROR.Select(e => $"{e.ERR_CODE!.ToDescription()}: {e.ERR_DESC!.ToDescription()}").ToList();
            return false;
        }

        private IEnumerable<Price> ConvertToPrices(OASISReport file, PriceIndexes priceIndexId, IEnumerable<PricingNode> pricingNodes)
        {
            var pricingNodesList = pricingNodes.ToList();
            var pricingNodesNames = pricingNodesList.Select(pn => pn.Name).Distinct();
            var pricingNodesDict = pricingNodesList.ToDictionary(pn => pn.Name, pn => pn.Id);

            var reportData = file
                .MessagePayload
                .RTO
                .REPORT_ITEM
                .SelectMany(i => i.REPORT_DATA);

            var filteredReportData = reportData
                .Join(
                    pricingNodesNames,
                    d => d.RESOURCE_NAME,
                    n => n,
                    (d, _) => d);

            return filteredReportData
                .Select(d =>
                {
                    var intervalStartTimeUtc = Instant.FromDateTimeOffset(DateTimeOffset.Parse(d.INTERVAL_START_GMT!));
                    var intervalEndTimeUtc = Instant.FromDateTimeOffset(DateTimeOffset.Parse(d.INTERVAL_END_GMT!));

                    return new Price
                    {
                        PriceIndexId = priceIndexId,
                        PricingNodeId = pricingNodesDict[d.RESOURCE_NAME],
                        IntervalStartTimeUtc = intervalStartTimeUtc,
                        IntervalEndTimeUtc = intervalEndTimeUtc,
                        IntervalLength = (int)(intervalEndTimeUtc - intervalStartTimeUtc).TotalMinutes,
                        LmpPrice = (decimal)(d.DATA_ITEM == OASISDataItems.LMP_PRC ? d.VALUE!.Value : 0),
                        EnergyPrice = (decimal)(d.DATA_ITEM == OASISDataItems.LMP_ENE_PRC ? d.VALUE!.Value : 0),
                        CongestionPrice = (decimal)(d.DATA_ITEM == OASISDataItems.LMP_CONG_PRC ? d.VALUE!.Value : 0),
                        LossPrice = (decimal)(d.DATA_ITEM == OASISDataItems.LMP_LOSS_PRC ? d.VALUE!.Value : 0),
                        PricingNodeName = d.RESOURCE_NAME
                    };
                })
                .GroupBy(p => new
                {
                    p.PriceIndexId, p.PricingNodeId, p.IntervalStartTimeUtc, p.IntervalEndTimeUtc, p.IntervalLength, p.PricingNodeName
                })
                .Select(p => new Price
                {
                    PriceIndexId = p.Key.PriceIndexId,
                    PricingNodeId = p.Key.PricingNodeId,
                    IntervalStartTimeUtc = p.Key.IntervalStartTimeUtc,
                    IntervalEndTimeUtc = p.Key.IntervalEndTimeUtc,
                    IntervalLength = p.Key.IntervalLength,
                    LmpPrice = p.FirstOrDefault(x => x.LmpPrice != 0)?.LmpPrice ?? 0,
                    EnergyPrice = p.FirstOrDefault(x => x.EnergyPrice != 0)?.EnergyPrice ?? 0,
                    CongestionPrice = p.FirstOrDefault(x => x.CongestionPrice != 0)?.CongestionPrice ?? 0,
                    LossPrice = p.FirstOrDefault(x => x.LossPrice != 0)?.LossPrice ?? 0,
                    PricingNodeName = p.Key.PricingNodeName,
                    LastModifiedAtUtc = _clock.GetCurrentInstant()
                })
                .OrderBy(p => p.PricingNodeId).ThenBy(p => p.IntervalEndTimeUtc);
        }
    }
}
