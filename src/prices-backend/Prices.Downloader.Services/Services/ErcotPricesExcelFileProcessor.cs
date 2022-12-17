using System.ComponentModel;
using System.Diagnostics;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using NodaTime;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Application.Models;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Downloader.Services.Models;

namespace Prices.Downloader.Services.Services;

public class ErcotPricesExcelFileProcessor : IPricesFileProcessor
{
    private readonly IClock _clock;
    private readonly ILogger<ErcotPricesExcelFileProcessor> _logger;

    public ErcotPricesExcelFileProcessor(IClock clock, ILogger<ErcotPricesExcelFileProcessor> logger)
    {
        _clock = clock;
        _logger = logger;
    }

    public Rtos RegionalTransmissionOperator => Rtos.ERCOT;

    public FileFormats FileFormat => FileFormats.Excel;

    public FileProcessorResult ProcessPrices(Stream blob, Rtos regionalTransmissionOperatorId, PriceMarkets priceMarketId,
        PriceIndexes priceIndexId, PriceTypes priceTypeId, IEnumerable<PricingNode> pricingNodes)
    {
        List<Price> convertedPrices;

        switch (priceMarketId)
        {
            case PriceMarkets.DAM:
                {
                    var prices = GetPrices<ErcotHistoricalDamRecord>(blob);
                    convertedPrices = ConvertToPrices(prices, priceIndexId, pricingNodes);
                }
                break;
            case PriceMarkets.RTM:
                {
                    var prices = GetPrices<ErcotHistoricalRtmRecord>(blob);

                    // Filter out "energy-weighted" point types
                    var filteredPrices = prices.Where(p => !p.SettlementPointType.EndsWith("ew", StringComparison.InvariantCultureIgnoreCase)).ToList();

                    convertedPrices = ConvertToPrices(filteredPrices, priceIndexId, pricingNodes);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(priceMarketId), priceMarketId, null);
        }

        var valid = Validate(convertedPrices, out var errors, out var warnings);
        if (!valid)
            return new FileProcessorResult(false, Array.Empty<Price>(), errors, warnings);

        _logger.LogInformation("Processed prices for {rto} {priceMarketId} {minDate}",
            regionalTransmissionOperatorId, priceMarketId, $"{convertedPrices.Min(p => p.IntervalStartTimeUtc):%yyyy-MM-dd}");
        return new FileProcessorResult(true, convertedPrices, Array.Empty<string>(), Array.Empty<string>());
    }

    private static List<T> GetPrices<T>(Stream blob)
    {
        var prices = new List<T>();

        using var workBook = new XLWorkbook(blob);

        foreach (var worksheet in workBook.Worksheets)
        {
            var columnInfo = Enumerable.Range(1, worksheet.Columns().Count())
                .Select(n => new { Index = n, ColumnName = worksheet.Cell(1, n).Value.ToString() })
                .ToList();

            foreach (var row in worksheet.RangeUsed().RowsUsed().Skip(1))
            {
                var obj = (T)Activator.CreateInstance(typeof(T))!;
                foreach (var prop in typeof(T).GetProperties())
                {
                    var attrs = prop.GetCustomAttributes(true);
                    var description = ((DescriptionAttribute)attrs.Single(a => a is DescriptionAttribute)).Description;
                    var col = columnInfo.Single(c => c.ColumnName == description).Index;
                    var val = row.Cell(col).Value;
                    var propType = prop.PropertyType;
                    prop.SetValue(obj, Convert.ChangeType(val, propType));
                }
                prices.Add(obj);
            }
        }

        return prices;
    }

    private List<Price> ConvertToPrices(List<ErcotHistoricalDamRecord> prices, PriceIndexes priceIndexId, IEnumerable<PricingNode> pricingNodes)
    {
        var pricingNodesList = pricingNodes.ToList();
        var pricingNodesNames = pricingNodesList.Select(pn => pn.Name).Distinct();
        var pricingNodesDict = pricingNodesList.ToDictionary(pn => pn.Name, pn => pn.Id);

        var filteredReportData = prices
            .Join(
                pricingNodesNames,
                d => d.SettlementPoint,
                n => n,
                (d, _) => d);

        return filteredReportData
            .Select(d =>
            {
                const int intervalLength = 60;

                ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(d.DeliveryDate, d.HourEnding, d.RepeatedHourFlag, intervalLength,
                    out var intervalStartTimeUtc, out var intervalEndTimeUtc);

                return new Price
                {
                    PriceIndexId = priceIndexId,
                    PricingNodeId = pricingNodesDict[d.SettlementPoint],
                    IntervalStartTimeUtc = intervalStartTimeUtc,
                    IntervalEndTimeUtc = intervalEndTimeUtc,
                    IntervalLength = intervalLength,
                    LmpPrice = Convert.ToDecimal(d.SettlementPointPrice),
                    PricingNodeName = d.SettlementPoint,
                    LastModifiedAtUtc = _clock.GetCurrentInstant()
                };
            })
            .OrderBy(p => p.PricingNodeId).ThenBy(p => p.IntervalEndTimeUtc)
            .ToList();
    }

    private List<Price> ConvertToPrices(List<ErcotHistoricalRtmRecord> prices, PriceIndexes priceIndexId, IEnumerable<PricingNode> pricingNodes)
    {
        var pricingNodesList = pricingNodes.ToList();
        var pricingNodesNames = pricingNodesList.Select(pn => pn.Name).Distinct();
        var pricingNodesDict = pricingNodesList.ToDictionary(pn => pn.Name, pn => pn.Id);

        var filteredReportData = prices
            .Join(
                pricingNodesNames,
                d => d.SettlementPointName,
                n => n,
                (d, _) => d);

        return filteredReportData
            .Select(d =>
            {
                const int intervalLength = 15;

                ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(d.DeliveryDate, d.DeliveryHour, d.DeliveryInterval, d.RepeatedHourFlag, intervalLength,
                    out var intervalStartTimeUtc, out var intervalEndTimeUtc);

                return new Price
                {
                    PriceIndexId = priceIndexId,
                    PricingNodeId = pricingNodesDict[d.SettlementPointName],
                    IntervalStartTimeUtc = intervalStartTimeUtc,
                    IntervalEndTimeUtc = intervalEndTimeUtc,
                    IntervalLength = intervalLength,
                    LmpPrice = Convert.ToDecimal(d.SettlementPointPrice),
                    PricingNodeName = d.SettlementPointName,
                    LastModifiedAtUtc = _clock.GetCurrentInstant()
                };
            })
            .OrderBy(p => p.PricingNodeId).ThenBy(p => p.IntervalEndTimeUtc)
            .ToList();
    }

    private static bool Validate(List<Price> prices, out IEnumerable<string> errors, out IEnumerable<string> warnings)
    {
        errors = new List<string>();
        warnings = new List<string>();

        var groupedPrices = prices
            .GroupBy(p => new { p.PriceIndexId, p.PricingNodeId, p.IntervalEndTimeUtc })
            .Select(p => (Count: p.Count(), Price: p))
            .ToList();

        if (groupedPrices.Any(gp => gp.Count > 1))
        {
            errors = groupedPrices
                .Where(gp => gp.Count > 1)
                .Select(gp => "Duplicate interval: " +
                              $"Index={gp.Price.Key.PriceIndexId} PricingNode={gp.Price.Key.PricingNodeId} IntervalEndTimeUtc={gp.Price.Key.IntervalEndTimeUtc}");

            return false;
        }

        return true;
    }
}