using Prices.Core.Application.Models;

namespace Prices.Core.Application.Exceptions
{
    public class PricesFileProcessorFunctionException : ApplicationException
    {
        public PricesFileProcessorFunctionException(PricesFileMetadata blobMetadata, IEnumerable<string> errors)
            : base($"Error processing prices for {blobMetadata} ({blobMetadata.FileSourceUrl}): {string.Join(Environment.NewLine, errors)}")
        {
        }
    }
}