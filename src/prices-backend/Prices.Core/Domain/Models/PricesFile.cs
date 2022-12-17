using NodaTime;
using Prices.Core.Domain.Enums;

namespace Prices.Core.Domain.Models
{
    public class PricesFile
    {
        public string BlobName { get; set; } = null!;
        public PriceIndexes PriceIndexId { get; set; }
        public PriceTypes PriceTypeId { get; set; }
        public string FileName { get; set; } = null!;
        public string VirtualFolder { get; set; } = null!;
        public Instant StartDateUtc { get; set; }
        public Instant EndDateUtc { get; set; }
        public string FileSourceUrl { get; set; } = null!;
        public string? DocumentId { get; set; }
        public long FileSize { get; set; }
        public Instant CreatedAtUtc { get; set; }

        public PriceIndex PriceIndex { get; set; } = null!;
        public PriceType PriceType { get; set; } = null!;
    }
}
