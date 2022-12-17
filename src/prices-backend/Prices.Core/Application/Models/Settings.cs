namespace Prices.Core.Application.Models
{
    public class Settings
    {
        public string SqlConnection { get; set; } = null!;
        public string SasUri { get; set; } = null!;
        public string[] CorsOrigins { get; set; } = null!;
    }
}
