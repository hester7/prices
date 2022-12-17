using System.IO.Compression;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Prices.Core.Application.Extensions;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Application.Models;
using Prices.Core.Application.XsdClasses.Master.V1;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;

namespace Prices.Downloader.Services.Services
{
    public class CaisoPricingNodesDownloader : IPricingNodesDownloader
    {
        private readonly IDbContextFactory<PricesContext> _contextFactory;
        private readonly IClock _clock;

        public CaisoPricingNodesDownloader(IDbContextFactory<PricesContext> contextFactory, IClock clock)
        {
            _contextFactory = contextFactory;
            _clock = clock;
        }

        public Rtos RegionalTransmissionOperator => Rtos.CAISO;

        public async Task<DownloadPricingNodesResult> DownloadPricingNodesAsync(int retryAttempts = 2, int delayInSecondsBetweenRetryAttempts = 30,
            CancellationToken cancellationToken = default)
        {
            var attempts = 0;
            var errors = new List<string>();

            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var pricingNodeTypeMappings = await context.PricingNodeTypeMappings
                .Where(m => m.RegionalTransmissionOperatorId == RegionalTransmissionOperator)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            var nodeTypes = string.Join(",", pricingNodeTypeMappings.Select(m => m.Code));

            while (true)
            {
                try
                {
                    ++attempts;
                    var startDate = DateTime.SpecifyKind(DateTime.Today.AddDays(-1), DateTimeKind.Unspecified);
                    var endDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
                    var startDateTime = $"{startDate.InZone(Rtos.CAISO):yyyyMMdd'T'HH':'mm'-0000'}";
                    var endDateTime = $"{endDate.InZone(Rtos.CAISO):yyyyMMdd'T'HH':'mm'-0000'}";
                    var requestUri = $"http://oasis.caiso.com/oasisapi/SingleZip?queryname=ATL_APNODE&APnode_type={nodeTypes}" +
                                     $"&startdatetime={startDateTime}&enddatetime={endDateTime}&version=1";

                    using var client = new HttpClient();
                    using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                    using var response = await client.SendAsync(request, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    await using var zipStream = await response.Content.ReadAsStreamAsync(cancellationToken);

                    using var archive = new ZipArchive(zipStream);

                    foreach (var entry in archive.Entries)
                    {
                        await using var stream = entry.Open();
                        var serializer = new XmlSerializer(typeof(OASISMaster));
                        var master = (OASISMaster)serializer.Deserialize(stream)!;
                        var pricingNodes = ConvertToPricingNodes(master, pricingNodeTypeMappings);
                        return new DownloadPricingNodesResult(true, pricingNodes, Array.Empty<string>());
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to download pricing nodes: {ex.Message}");

                    if (attempts > retryAttempts)
                        return new DownloadPricingNodesResult(false, Array.Empty<PricingNode>(), errors);

                    await Task.Delay(delayInSecondsBetweenRetryAttempts * 1000, cancellationToken);
                }
            }
        }

        private IEnumerable<PricingNode> ConvertToPricingNodes(OASISMaster oasisMaster, List<PricingNodeTypeMapping> pricingNodeTypeMappings)
        {
            var now = _clock.GetCurrentInstant();
            var pricingNodeTypeMappingsDict = pricingNodeTypeMappings.ToDictionary(m => m.Code, m => m.PricingNodeTypeId);

            return oasisMaster
                .MessagePayload
                .RTO
                .ATLS_ITEM
                .SelectMany(i => i.ATLS_DATA)
                .Select(d =>
                {
                    if (!pricingNodeTypeMappingsDict.TryGetValue(d.APNODE_TYPE, out var pricingNodeType))
                        pricingNodeType = PricingNodeTypes.Unknown;

                    return new PricingNode
                    {
                        RegionalTransmissionOperatorId = Rtos.CAISO,
                        Name = d.APNODE_NAME,
                        PricingNodeTypeId = pricingNodeType,
                        StartDateUtc = d.START_DATE_GMT is not null
                            ? Instant.FromDateTimeOffset(DateTimeOffset.Parse(d.START_DATE_GMT))
                            : null,
                        EndDateUtc = d.END_DATE_GMT is not null
                            ? Instant.FromDateTimeOffset(DateTimeOffset.Parse(d.END_DATE_GMT))
                            : null,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    };
                });
        }
    }
}
