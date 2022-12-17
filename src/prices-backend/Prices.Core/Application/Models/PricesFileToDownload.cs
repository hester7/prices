using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;

namespace Prices.Core.Application.Models;

public class PricesFileToDownload
{
    public PricesFileToDownload(
        string requestUri,
        PriceTypes priceTypeId,
        PriceIndexes priceIndexId,
        Rtos regionalTransmissionOperatorId,
        PriceMarkets priceMarketId,
        FileFormats fileFormatId,
        string? documentId = null)
    {
        RequestUri = requestUri;
        PriceTypeId = priceTypeId;
        PriceIndexId = priceIndexId;
        RegionalTransmissionOperatorId = regionalTransmissionOperatorId;
        PriceMarketId = priceMarketId;
        FileFormatId = fileFormatId;
        DocumentId = documentId;
    }

    public PricesFileToDownload(
        string requestUri,
        PriceTypes priceTypeId,
        PriceIndex priceIndex,
        FileFormats fileFormatId,
        string? documentId = null)
    {
        RequestUri = requestUri;
        PriceTypeId = priceTypeId;
        PriceIndexId = priceIndex.Id;
        RegionalTransmissionOperatorId = priceIndex.RegionalTransmissionOperatorId;
        PriceMarketId = priceIndex.PriceMarketId;
        FileFormatId = fileFormatId;
        DocumentId = documentId;
    }

    public string RequestUri { get; }
    public PriceTypes PriceTypeId { get; }
    public PriceIndexes PriceIndexId { get; }
    public Rtos RegionalTransmissionOperatorId { get; }
    public PriceMarkets PriceMarketId { get; }
    public FileFormats FileFormatId { get; }
    public string? DocumentId { get; }

    public override string ToString() => $"{RegionalTransmissionOperatorId} {PriceTypeId} {PriceMarketId}";
}