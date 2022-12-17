using Prices.Core.Domain.Enums;

namespace Prices.Core.Domain.Models
{
    public class PriceMarket
    {
        public PriceMarket()
        {
            PriceIndexes = new HashSet<PriceIndex>();
        }

        public PriceMarkets Id { get; set; }
        public string Name { get; set; } = null!;
        public string Abbreviation { get; set; } = null!;

        public ICollection<PriceIndex> PriceIndexes { get; set; }
    }
}
