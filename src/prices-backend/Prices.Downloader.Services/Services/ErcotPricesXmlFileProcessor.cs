using Prices.Core.Application.Models;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using System.Xml.Serialization;
using NodaTime;
using Microsoft.Extensions.Logging;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Application.XsdClasses;

namespace Prices.Downloader.Services.Services
{
    public class ErcotPricesXmlFileProcessor : IPricesFileProcessor
    {
        private readonly IClock _clock;
        private readonly ILogger<ErcotPricesXmlFileProcessor> _logger;

        public ErcotPricesXmlFileProcessor(IClock clock, ILogger<ErcotPricesXmlFileProcessor> logger)
        {
            _clock = clock;
            _logger = logger;
        }

        public Rtos RegionalTransmissionOperator => Rtos.ERCOT;

        public FileFormats FileFormat => FileFormats.XML;

        public FileProcessorResult ProcessPrices(Stream blob, Rtos regionalTransmissionOperatorId, PriceMarkets priceMarketId,
            PriceIndexes priceIndexId, PriceTypes priceTypeId, IEnumerable<PricingNode> pricingNodes)
        {
            object file;
            try
            {
                file = MapFile(blob, priceMarketId, priceTypeId);
            }
            catch (Exception ex)
            {
                return new FileProcessorResult(false, Array.Empty<Price>(), new[] { ex.Message }, Array.Empty<string>());
            }

            var prices = ConvertToPrices(file, priceIndexId, pricingNodes).ToList();
            _logger.LogInformation("Processed prices for {rto} {priceMarketId} {minDate}",
                regionalTransmissionOperatorId, priceMarketId, $"{prices.Min(p => p.IntervalStartTimeUtc):%yyyy-MM-dd}");
            return new FileProcessorResult(true, prices, Array.Empty<string>(), Array.Empty<string>());
        }

        private static object MapFile(Stream blob, PriceMarkets priceMarketId, PriceTypes priceTypeId)
        {
            switch (priceMarketId)
            {
                case PriceMarkets.DAM:
                    {
                        var serializer = new XmlSerializer(typeof(DAMSettlementPointPrices));
                        return (DAMSettlementPointPrices)serializer.Deserialize(blob)!;
                    }
                case PriceMarkets.RTM:
                    {
                        if (priceTypeId == PriceTypes.Current)
                        {
                            var serializer = new XmlSerializer(typeof(LMPsByResourceNodes));
                            return (LMPsByResourceNodes)serializer.Deserialize(blob)!;
                        }
                        else
                        {
                            var serializer = new XmlSerializer(typeof(SPPatHubsLoadZones));
                            return (SPPatHubsLoadZones)serializer.Deserialize(blob)!;
                        }
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(priceMarketId), priceMarketId, null);
            }
        }

        private IEnumerable<Price> ConvertToPrices(object file, PriceIndexes priceIndexId, IEnumerable<PricingNode> pricingNodes) =>
            file switch
            {
                DAMSettlementPointPrices damPrices => ConvertToPrices(damPrices, priceIndexId, pricingNodes),
                LMPsByResourceNodes rtmPrices => ConvertToPrices(rtmPrices, priceIndexId, pricingNodes),
                SPPatHubsLoadZones rtmPrices => ConvertToPrices(rtmPrices, priceIndexId, pricingNodes),
                _ => throw new ArgumentOutOfRangeException(nameof(file), file, null)
            };

        private IEnumerable<Price> ConvertToPrices(DAMSettlementPointPrices file, PriceIndexes priceIndexId, IEnumerable<PricingNode> pricingNodes)
        {
            var pricingNodesList = pricingNodes.ToList();
            var pricingNodesNames = pricingNodesList.Select(pn => pn.Name).Distinct();
            var pricingNodesDict = pricingNodesList.ToDictionary(pn => pn.Name, pn => pn.Id);

            var filteredReportData = file
                .DAMSettlementPointPrice
                .Join(
                    pricingNodesNames,
                    d => d.SettlementPoint,
                    n => n,
                    (d, _) => d);

            return filteredReportData
                .Select(d =>
                {
                    const int intervalLength = 60;
                    
                    ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(d.DeliveryDate, d.HourEnding, d.DSTFlag, intervalLength,
                        out var intervalStartTimeUtc, out var intervalEndTimeUtc);

                    return new Price
                    {
                        PriceIndexId = priceIndexId,
                        PricingNodeId = pricingNodesDict[d.SettlementPoint],
                        IntervalStartTimeUtc = intervalStartTimeUtc,
                        IntervalEndTimeUtc = intervalEndTimeUtc,
                        IntervalLength = intervalLength,
                        LmpPrice = d.SettlementPointPrice,
                        PricingNodeName = d.SettlementPoint,
                        LastModifiedAtUtc = _clock.GetCurrentInstant()
                    };
                })
                .OrderBy(p => p.PricingNodeId).ThenBy(p => p.IntervalEndTimeUtc);
        }

        private IEnumerable<Price> ConvertToPrices(LMPsByResourceNodes file, PriceIndexes priceIndexId, IEnumerable<PricingNode> pricingNodes)
        {
            var pricingNodesList = pricingNodes.ToList();
            var pricingNodesNames = pricingNodesList.Select(pn => pn.Name).Distinct();
            var pricingNodesDict = pricingNodesList.ToDictionary(pn => pn.Name, pn => pn.Id);

            var filteredReportData = file
                .LMPsByResourceNode
                .Join(
                    pricingNodesNames,
                    d => d.SettlementPoint,
                    n => n,
                    (d, _) => d);

            return filteredReportData
                .Select(d =>
                {
                    const int intervalLength = 5;

                    ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(d.SCEDTimestamp, d.RepeatedHourFlag, intervalLength,
                        out var intervalStartTimeUtc, out var intervalEndTimeUtc);

                    return new Price
                    {
                        PriceIndexId = priceIndexId,
                        PricingNodeId = pricingNodesDict[d.SettlementPoint],
                        IntervalStartTimeUtc = intervalStartTimeUtc,
                        IntervalEndTimeUtc = intervalEndTimeUtc,
                        IntervalLength = intervalLength,
                        LmpPrice = d.LMP,
                        PricingNodeName = d.SettlementPoint,
                        LastModifiedAtUtc = _clock.GetCurrentInstant()
                    };
                })
                .OrderBy(p => p.PricingNodeId).ThenBy(p => p.IntervalEndTimeUtc);
        }

        private IEnumerable<Price> ConvertToPrices(SPPatHubsLoadZones file, PriceIndexes priceIndexId, IEnumerable<PricingNode> pricingNodes)
        {
            var pricingNodesList = pricingNodes.ToList();
            var pricingNodesNames = pricingNodesList.Select(pn => pn.Name).Distinct();
            var pricingNodesDict = pricingNodesList.ToDictionary(pn => pn.Name, pn => pn.Id);

            var filteredReportData = file
                .SPPatHubsLoadZone
                .Join(
                    pricingNodesNames,
                    d => d.SettlementPointName,
                    n => n,
                    (d, _) => d);

            return filteredReportData
                .Where(d => !d.SettlementPointType.EndsWith("ew", StringComparison.InvariantCultureIgnoreCase)) // Filter out "energy-weighted" point types
                .Select(d =>
                {
                    const int intervalLength = 15;

                    ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(d.DeliveryDate, d.DeliveryHour, d.DeliveryInterval, d.DSTFlag, intervalLength,
                        out var intervalStartTimeUtc, out var intervalEndTimeUtc);

                    return new Price
                    {
                        PriceIndexId = priceIndexId,
                        PricingNodeId = pricingNodesDict[d.SettlementPointName],
                        IntervalStartTimeUtc = intervalStartTimeUtc,
                        IntervalEndTimeUtc = intervalEndTimeUtc,
                        IntervalLength = intervalLength,
                        LmpPrice = d.SettlementPointPrice,
                        PricingNodeName = d.SettlementPointName,
                        LastModifiedAtUtc = _clock.GetCurrentInstant()
                    };
                })
                .OrderBy(p => p.PricingNodeId).ThenBy(p => p.IntervalEndTimeUtc);
        }
    }
}
