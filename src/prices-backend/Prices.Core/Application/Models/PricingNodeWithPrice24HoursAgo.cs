using Prices.Core.Domain.Models;

namespace Prices.Core.Application.Models
{
    public class PricingNodeWithPrice24HoursAgo : PricingNode
    {
        public decimal? Price24HoursAgo { get; set; }
    }
}
