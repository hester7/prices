using Newtonsoft.Json;
using NodaTime;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;

namespace Prices.Core.Application.Models;

public class PricesFileMetadata
{
    [JsonConstructor]
    public PricesFileMetadata(
        PriceTypes priceTypeId,
        PriceIndexes priceIndexId,
        Rtos regionalTransmissionOperatorId,
        PriceMarkets priceMarketId,
        FileFormats fileFormatId,
        string fileName,
        string fileSourceUrl,
        string? documentId = null)
    {
        PriceTypeId = priceTypeId;
        PriceIndexId = priceIndexId;
        RegionalTransmissionOperatorId = regionalTransmissionOperatorId;
        PriceMarketId = priceMarketId;
        FileFormatId = fileFormatId;
        FileName = fileName;
        FileSourceUrl = fileSourceUrl;
        DocumentId = documentId;
    }

    public PricesFileMetadata(
        PriceTypes priceTypeId,
        PriceIndex priceIndex,
        FileFormats fileFormatId,
        string fileName,
        string fileSourceUrl,
        string? documentId = null)
    {
        PriceTypeId = priceTypeId;
        PriceIndexId = priceIndex.Id;
        RegionalTransmissionOperatorId = priceIndex.RegionalTransmissionOperatorId;
        PriceMarketId = priceIndex.PriceMarketId;
        FileFormatId = fileFormatId;
        FileName = fileName;
        FileSourceUrl = fileSourceUrl;
        DocumentId = documentId;
    }

    public PricesFileMetadata(PricesFileToDownload fileToDownload, string fileName, string? documentId = null)
    {
        PriceTypeId = fileToDownload.PriceTypeId;
        PriceIndexId = fileToDownload.PriceIndexId;
        RegionalTransmissionOperatorId = fileToDownload.RegionalTransmissionOperatorId;
        PriceMarketId = fileToDownload.PriceMarketId;
        FileFormatId = fileToDownload.FileFormatId;
        FileName = fileName;
        FileSourceUrl = fileToDownload.RequestUri;
        DocumentId = documentId;
    }

    public PriceTypes PriceTypeId { get; }
    public PriceIndexes PriceIndexId { get; }
    public Rtos RegionalTransmissionOperatorId { get; }
    public PriceMarkets PriceMarketId { get; }
    public FileFormats FileFormatId { get; }
    public string FileName { get; }
    public string FileSourceUrl { get; }
    public string? DocumentId { get; }

    [JsonIgnore]
    public string RemoteFolder => $"{RegionalTransmissionOperatorId}/{PriceTypeId}/{PriceMarketId}";

    [JsonIgnore]
    public string BlobName => $"{RemoteFolder}/{FileName}";

    public override string ToString() => $"{RegionalTransmissionOperatorId} {PriceTypeId} {PriceMarketId}";

    public PricesFile ToPricesFile(string blobName, Instant startDateUtc, Instant endDateUtc, long fileSize, Instant createdAtUtc) =>
        new()
        {
            PriceIndexId = PriceIndexId,
            PriceTypeId = PriceTypeId,
            FileName = FileName,
            VirtualFolder = RemoteFolder,
            BlobName = blobName,
            StartDateUtc = startDateUtc,
            EndDateUtc = endDateUtc,
            FileSourceUrl = FileSourceUrl,
            DocumentId = DocumentId,
            FileSize = fileSize,
            CreatedAtUtc = createdAtUtc
        };
}