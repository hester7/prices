using Prices.Core.Domain.Models;

namespace Prices.Core.Application.Models;

public readonly record struct FileProcessorResult(bool Success, IEnumerable<Price> Prices, IEnumerable<string> Errors, IEnumerable<string> Warnings);