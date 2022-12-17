using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NodaTime;
using Prices.AzureBlobStorage;
using Prices.Core.Application.Interfaces;
using Prices.Core.Application.Models;
using Prices.Core.Domain.Enums;
using Prices.Downloader.Services.Services;
using Prices.Downloader.Tests.Mocks;
using Prices.Persistence.EntityFramework;

namespace Prices.Downloader.Tests
{
    public class DownloaderTests
    {
        private readonly IOptions<Settings> _settingsOptions;
        private readonly AzureBlobStorageClientFactory _azureBlobStorageClientFactory;
        private readonly IAzureBlobStorageClient _azureBlobStorageClient;
        private readonly ILoggerFactory _loggerFactory;
        private readonly MockDbContextFactory _dbContextFactory;
        private readonly MockPricesFileProcessorFactory _pricesFileProcessorFactory;
        private readonly CaisoCurrentPricesDownloader _caisoCurrentPricesDownloader;
        private readonly CaisoPricesFileDownloader _caisoPricesFileDownloader;
        private readonly CaisoPricingNodesDownloader _caisoPricingNodesDownloader;
        private readonly CaisoPricesXmlFileProcessor _caisoPricesXmlFileProcessor;
        private readonly ErcotCurrentPricesDownloader _ercotCurrentPricesDownloader;
        private readonly ErcotPricesFileDownloader _ercotPricesFileDownloader;
        private readonly ErcotPricesExcelFileProcessor _ercotPricesExcelFileProcessor;
        private readonly LocalDate _date;
        private readonly List<string> _pricingNodes;

        public DownloaderTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<DownloaderTests>()
                .Build();

            var settings = configuration.Get<Settings>();
            _settingsOptions = Options.Create(settings);

            _azureBlobStorageClientFactory = new AzureBlobStorageClientFactory();
            _azureBlobStorageClient = _azureBlobStorageClientFactory.NewSasTokenClient(settings.SasUri);
            IClock clock = SystemClock.Instance;

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            _loggerFactory = serviceProvider.GetService<ILoggerFactory>()!;
            var caisoPricesFileDownloaderLogger = _loggerFactory.CreateLogger<CaisoPricesFileDownloader>();
            var caisoPricesXmlFileProcessorLogger = _loggerFactory.CreateLogger<CaisoPricesXmlFileProcessor>();
            var caisoCurrentPricesDownloaderLogger = _loggerFactory.CreateLogger<CaisoCurrentPricesDownloader>();
            var ercotPricesFileDownloaderLogger = _loggerFactory.CreateLogger<ErcotPricesFileDownloader>();
            var ercotCurrentPricesDownloaderLogger = _loggerFactory.CreateLogger<ErcotCurrentPricesDownloader>();
            var ercotPricesExcelFileProcessorLogger = _loggerFactory.CreateLogger<ErcotPricesExcelFileProcessor>();

            var mockAzureBlobStorageClientFactory = new MockAzureBlobStorageClientFactory();
            _dbContextFactory = new MockDbContextFactory();
            _pricesFileProcessorFactory = new MockPricesFileProcessorFactory(_loggerFactory, clock);

            _caisoCurrentPricesDownloader = new CaisoCurrentPricesDownloader(
                mockAzureBlobStorageClientFactory, _dbContextFactory, _pricesFileProcessorFactory, caisoCurrentPricesDownloaderLogger, _settingsOptions);
            _caisoPricesFileDownloader = new CaisoPricesFileDownloader(
                mockAzureBlobStorageClientFactory, _dbContextFactory, _pricesFileProcessorFactory, caisoPricesFileDownloaderLogger, _settingsOptions);
            _caisoPricingNodesDownloader = new CaisoPricingNodesDownloader(_dbContextFactory, clock);
            _caisoPricesXmlFileProcessor = new CaisoPricesXmlFileProcessor(clock, caisoPricesXmlFileProcessorLogger);

            _ercotCurrentPricesDownloader = new ErcotCurrentPricesDownloader(
                mockAzureBlobStorageClientFactory, _dbContextFactory, _pricesFileProcessorFactory, ercotCurrentPricesDownloaderLogger, _settingsOptions);
            _ercotPricesFileDownloader = new ErcotPricesFileDownloader(
                mockAzureBlobStorageClientFactory, _dbContextFactory, _pricesFileProcessorFactory, ercotPricesFileDownloaderLogger, _settingsOptions);
            _ercotPricesExcelFileProcessor = new ErcotPricesExcelFileProcessor(clock, ercotPricesExcelFileProcessorLogger);

            _date = LocalDate.FromDateTime(DateTime.Today.AddDays(-5));
            _pricingNodes = SeedData.PricingNodes.Select(pn => pn.Name).ToList();
        }

        #region CAISO

        [Fact]
        public async Task DownloadCaisoPricingNodesTest()
        {
            var attempt = 1;
            while (attempt <= 2)
            {
                var result = await _caisoPricingNodesDownloader.DownloadPricingNodesAsync();
                if (!result.Success)
                {
                    await Task.Delay(5000);
                    attempt++;
                    continue;
                }

                Assert.True(result.Success);
                Assert.NotEmpty(result.PricingNodes);
                Assert.Empty(result.Errors);
                break;
            }
        }

        [Fact]
        public async Task DownloadCaisoCurrentPricesAllTest()
        {
            var attempt = 1;
            while (attempt <= 2)
            {
                var result = await _caisoCurrentPricesDownloader.DownloadCurrentPricesAsync();
                if (!result.Success)
                {
                    await Task.Delay(5000);
                    attempt++;
                    continue;
                }

                Assert.True(result.Success);
                Assert.NotEmpty(result.Prices);
                Assert.Empty(result.Errors);
                break;
            }
        }

        [Fact]
        public async Task DownloadCaisoCurrentPricesForNodesTest()
        {
            var attempt = 1;
            while (attempt <= 2)
            {
                var result = await _caisoCurrentPricesDownloader.DownloadCurrentPricesAsync(_pricingNodes);
                if (!result.Success)
                {
                    await Task.Delay(5000);
                    attempt++;
                    continue;
                }

                Assert.True(result.Success);
                Assert.NotEmpty(result.Prices);
                Assert.Empty(result.Errors);
                break;
            }
        }

        [Fact]
        public async Task DownloadCaisoDayAheadTest()
        {
            var attempt = 1;
            while (attempt <= 2)
            {
                var priceIndex = SeedData.PriceIndexes.Single(i => i.Id == PriceIndexes.CaisoDayAhead);
                var filesToDownloadResult = await _caisoPricesFileDownloader.GetFilesToDownload(PriceTypes.Daily, priceIndex, FileFormats.XML, _date);
                Assert.True(filesToDownloadResult.Success);
                Assert.NotEmpty(filesToDownloadResult.FilesToDownload);
                Assert.Empty(filesToDownloadResult.Errors);

                var fileToDownload = filesToDownloadResult.FilesToDownload.SingleOrDefault();
                Assert.NotNull(fileToDownload);

                var result = await _caisoPricesFileDownloader.DownloadPricesFileAsync(fileToDownload);
                if (!result.Success)
                {
                    await Task.Delay(5000);
                    attempt++;
                    continue;
                }

                Assert.True(result.Success);
                Assert.NotNull(result.Metadata);
                Assert.Empty(result.Errors);
                break;
            }
        }

        [Fact]
        public async Task DownloadCaisoRealTimeTest()
        {
            var attempt = 1;
            while (attempt <= 2)
            {
                var priceIndex = SeedData.PriceIndexes.Single(i => i.Id == PriceIndexes.CaisoRealTime);
                var filesToDownloadResult = await _caisoPricesFileDownloader.GetFilesToDownload(PriceTypes.Daily, priceIndex, FileFormats.XML, _date);
                Assert.True(filesToDownloadResult.Success);
                Assert.NotEmpty(filesToDownloadResult.FilesToDownload);
                Assert.Empty(filesToDownloadResult.Errors);

                var fileToDownload = filesToDownloadResult.FilesToDownload.SingleOrDefault();
                Assert.NotNull(fileToDownload);

                var result = await _caisoPricesFileDownloader.DownloadPricesFileAsync(fileToDownload);
                if (!result.Success)
                {
                    await Task.Delay(5000);
                    attempt++;
                    continue;
                }

                Assert.True(result.Success);
                Assert.NotNull(result.Metadata);
                Assert.Empty(result.Errors);
                break;
            }
        }

        //[Fact]
        //public async Task DownloadCaisoFifteenMinuteTest()
        //{
        //    var attempt = 1;
        //    while (attempt <= 2)
        //    {
        //        var priceIndex = SeedData.PriceIndexes.Single(i => i.PriceIndexId == PriceIndexes.CaisoFifteenMinute);
        //        var filesToDownloadResult = await _caisoPricesFileDownloader.GetFilesToDownload(PriceTypes.Daily, priceIndex, FileFormats.XML, _date);
        //        Assert.True(filesToDownloadResult.Success);
        //        Assert.NotEmpty(filesToDownloadResult.FilesToDownload);
        //        Assert.Empty(filesToDownloadResult.Errors);

        //        var fileToDownload = filesToDownloadResult.FilesToDownload.SingleOrDefault();
        //        Assert.NotNull(fileToDownload);

        //        var result = await _caisoPricesFileDownloader.DownloadPricesFileAsync(fileToDownload);
        //        if (!result.Success)
        //        {
        //            await Task.Delay(5000);
        //            attempt++;
        //            continue;
        //        }

        //        Assert.True(result.Success);
        //        Assert.NotNull(result.Metadata);
        //        Assert.Empty(result.Errors);
        //        break;
        //    }
        //}

        //[Fact]
        //public async Task UploadCaisoPricesToAzureTest()
        //{
        //    if (!Debugger.IsAttached)
        //        return;

        //    var caisoPricesFileDownloaderLogger = _loggerFactory.CreateLogger<CaisoPricesFileDownloader>();
        //    var caisoPricesFileDownloader = new CaisoPricesFileDownloader(
        //        _azureBlobStorageClientFactory, _dbContextFactory, _pricesFileProcessorFactory, caisoPricesFileDownloaderLogger, _settingsOptions);

        //    var attempt = 1;
        //    while (attempt <= 2)
        //    {
        //        var priceIndex = SeedData.PriceIndexes.Single(i => i.Id == PriceIndexes.CaisoRealTime);
        //        var filesToDownloadResult = await caisoPricesFileDownloader.GetFilesToDownload(PriceTypes.Daily, priceIndex, FileFormats.XML, _date);
        //        Assert.True(filesToDownloadResult.Success);
        //        Assert.NotEmpty(filesToDownloadResult.FilesToDownload);
        //        Assert.Empty(filesToDownloadResult.Errors);

        //        var fileToDownload = filesToDownloadResult.FilesToDownload.SingleOrDefault();
        //        Assert.NotNull(fileToDownload);

        //        var result = await caisoPricesFileDownloader.DownloadPricesFileAsync(fileToDownload);
        //        if (!result.Success)
        //        {
        //            await Task.Delay(5000);
        //            attempt++;
        //            continue;
        //        }

        //        Assert.True(result.Success);
        //        Assert.NotNull(result.Metadata);
        //        Assert.Empty(result.Errors);

        //        var blobs = _azureBlobStorageClient.GetBlobs();
        //        var blob = blobs.SingleOrDefault(b => b.BlobName == result.Metadata.BlobName);
        //        Assert.NotNull(blob);

        //        await _azureBlobStorageClient.DeleteFileAsync(blob.BlobName);
        //        break;
        //    }
        //}

        [Fact]
        public async Task ProcessCaisoFileSuccessTest()
        {
            const string fileName = @".\TestFiles\20221025_20221026_PRC_LMP_DAM_20221030_05_18_05_v1.xml";
            var priceIndex = SeedData.PriceIndexes.Single(i => i.Id == PriceIndexes.CaisoDayAhead);
            var metadata = new PricesFileMetadata(PriceTypes.Daily, priceIndex.Id, priceIndex.RegionalTransmissionOperatorId, priceIndex.PriceMarketId,
                FileFormats.XML, "20221025_20221026_PRC_LMP_DAM_20221030_05_18_05_v1.xml", string.Empty);
            await using var fs = File.OpenRead(fileName);

            var pricingNodes = SeedData.PricingNodes;
            var result = _caisoPricesXmlFileProcessor.ProcessPrices(
                fs, Rtos.CAISO, priceIndex.PriceMarketId, priceIndex.Id, PriceTypes.Daily, pricingNodes);

            Assert.True(result.Success);
            Assert.NotEmpty(result.Prices);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ProcessCaisoFileFailureTest()
        {
            const string fileName = @".\TestFiles\INVALID_REQUEST.xml";
            var priceIndex = SeedData.PriceIndexes.Single(i => i.Id == PriceIndexes.CaisoDayAhead);
            var metadata = new PricesFileMetadata(PriceTypes.Daily, priceIndex.Id, priceIndex.RegionalTransmissionOperatorId, priceIndex.PriceMarketId,
                FileFormats.XML, "INVALID_REQUEST.xml", string.Empty);
            await using var fs = File.OpenRead(fileName);

            var pricingNodes = SeedData.PricingNodes;
            var result = _caisoPricesXmlFileProcessor.ProcessPrices(
                fs, Rtos.CAISO, priceIndex.PriceMarketId, priceIndex.Id, PriceTypes.Daily, pricingNodes);

            Assert.False(result.Success);
            Assert.Empty(result.Prices);
            Assert.NotEmpty(result.Errors);
        }

        #endregion

        #region ERCOT

        [Fact]
        public async Task DownloadErcotCurrentPricesTest()
        {
            var result = await _ercotCurrentPricesDownloader.DownloadCurrentPricesAsync();
            Assert.True(result.Success);
            Assert.NotEmpty(result.Prices);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task DownloadErcotDayAheadTest()
        {
            var priceIndex = SeedData.PriceIndexes.Single(i => i.Id == PriceIndexes.ErcotDayAhead);
            var filesToDownloadResult = await _ercotPricesFileDownloader.GetFilesToDownload(PriceTypes.Daily, priceIndex, FileFormats.XML, _date);
            Assert.True(filesToDownloadResult.Success);
            Assert.NotEmpty(filesToDownloadResult.FilesToDownload);
            Assert.Empty(filesToDownloadResult.Errors);

            var fileToDownload = filesToDownloadResult.FilesToDownload.SingleOrDefault();
            Assert.NotNull(fileToDownload);

            var result = await _ercotPricesFileDownloader.DownloadPricesFileAsync(fileToDownload);
            Assert.True(result.Success);
            Assert.NotNull(result.Metadata);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task DownloadErcotRealTimeTest()
        {
            var priceIndex = SeedData.PriceIndexes.Single(i => i.Id == PriceIndexes.ErcotRealTime);
            var filesToDownloadResult = await _ercotPricesFileDownloader.GetFilesToDownload(PriceTypes.Daily, priceIndex, FileFormats.XML, _date);
            Assert.True(filesToDownloadResult.Success);
            Assert.NotEmpty(filesToDownloadResult.FilesToDownload);
            Assert.Empty(filesToDownloadResult.Errors);

            var fileToDownload = filesToDownloadResult.FilesToDownload.FirstOrDefault();
            Assert.NotNull(fileToDownload);

            var result = await _ercotPricesFileDownloader.DownloadPricesFileAsync(fileToDownload);
            Assert.True(result.Success);
            Assert.NotNull(result.Metadata);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ErcotXmlProcessorDayAheadTest()
        {
            var priceIndex = SeedData.PriceIndexes.Single(i => i.Id == PriceIndexes.ErcotDayAhead);
            var filesToDownloadResult = await _ercotPricesFileDownloader.GetFilesToDownload(PriceTypes.Daily, priceIndex, FileFormats.XML, _date);
            Assert.True(filesToDownloadResult.Success);
            Assert.NotEmpty(filesToDownloadResult.FilesToDownload);
            Assert.Empty(filesToDownloadResult.Errors);

            var fileToDownload = filesToDownloadResult.FilesToDownload.FirstOrDefault();
            Assert.NotNull(fileToDownload);

            var result = await _ercotPricesFileDownloader.DownloadPriceIntervalsAsync(fileToDownload);
            Assert.True(result.Success);
            Assert.NotEmpty(result.Prices);
            Assert.Empty(result.Errors);
            Assert.Empty(result.Warnings);
        }

        [Fact]
        public async Task ErcotXmlProcessorRealTimeTest()
        {
            var priceIndex = SeedData.PriceIndexes.Single(i => i.Id == PriceIndexes.ErcotRealTime);
            var filesToDownloadResult = await _ercotPricesFileDownloader.GetFilesToDownload(PriceTypes.Daily, priceIndex, FileFormats.XML, _date);
            Assert.True(filesToDownloadResult.Success);
            Assert.NotEmpty(filesToDownloadResult.FilesToDownload);
            Assert.Empty(filesToDownloadResult.Errors);

            var fileToDownload = filesToDownloadResult.FilesToDownload.FirstOrDefault();
            Assert.NotNull(fileToDownload);

            var result = await _ercotPricesFileDownloader.DownloadPriceIntervalsAsync(fileToDownload);
            Assert.True(result.Success);
            Assert.NotEmpty(result.Prices);
            Assert.Empty(result.Errors);
            Assert.Empty(result.Warnings);
        }

        [Fact]
        public async Task ErcotExcelProcessorDayAheadTest()
        {
            const string fileName = @".\TestFiles\rpt.00013060.0000000000000000.DAMLZHBSPP_2021.xlsx";
            var priceIndex = SeedData.PriceIndexes.Single(i => i.Id == PriceIndexes.ErcotDayAhead);
            var metadata = new PricesFileMetadata(PriceTypes.Historical, priceIndex.Id, priceIndex.RegionalTransmissionOperatorId, priceIndex.PriceMarketId,
                FileFormats.Excel, "rpt.00013060.0000000000000000.DAMLZHBSPP_2021.xlsx", string.Empty);
            await using var fs = File.OpenRead(fileName);

            var pricingNodes = SeedData.PricingNodes;
            var result = _ercotPricesExcelFileProcessor.ProcessPrices(
                fs, Rtos.ERCOT, priceIndex.PriceMarketId, priceIndex.Id, PriceTypes.Historical, pricingNodes);

            Assert.True(result.Success);
            Assert.NotEmpty(result.Prices);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ErcotExcelProcessorRealTimeMinuteTest()
        {
            const string fileName = @".\TestFiles\rpt.00013061.0000000000000000.RTMLZHBSPP_2021.xlsx";
            var priceIndex = SeedData.PriceIndexes.Single(i => i.Id == PriceIndexes.ErcotRealTime);
            var metadata = new PricesFileMetadata(PriceTypes.Historical, priceIndex.Id, priceIndex.RegionalTransmissionOperatorId, priceIndex.PriceMarketId,
                FileFormats.Excel, "rpt.00013061.0000000000000000.RTMLZHBSPP_2021.xlsx", string.Empty);
            await using var fs = File.OpenRead(fileName);

            var pricingNodes = SeedData.PricingNodes;
            var result = _ercotPricesExcelFileProcessor.ProcessPrices(
                fs, Rtos.ERCOT, priceIndex.PriceMarketId, priceIndex.Id, PriceTypes.Historical, pricingNodes);

            Assert.True(result.Success);
            Assert.NotEmpty(result.Prices);
            Assert.Empty(result.Errors);
        }

        #endregion

        [Fact]
        public void MetadataSerializerTest()
        {
            var metadataOriginal = new PricesFileMetadata(PriceTypes.Daily, PriceIndexes.CaisoDayAhead, Rtos.CAISO, PriceMarkets.DAM, FileFormats.XML,
                "fileName", "fileSourceUrl");
            var json = JsonConvert.SerializeObject(metadataOriginal);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var metadata = JsonConvert.DeserializeObject<PricesFileMetadata>(JsonConvert.SerializeObject(dictionary))!;
            Assert.Equal(metadataOriginal.ToString(), metadata.ToString());
        }
    }
}