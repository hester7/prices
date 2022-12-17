using Prices.Core.Domain.Enums;

namespace Prices.Core.Domain.Models
{
    public class PriceType
    {
        public PriceType()
        {
            PricesFiles = new HashSet<PricesFile>();
        }

        public PriceTypes Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<PricesFile> PricesFiles { get; set; }
    }
}
