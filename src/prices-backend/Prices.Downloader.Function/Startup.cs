﻿using System;
using System.Reflection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prices.AzureBlobStorage;
using Prices.Core.Application.Models;
using Prices.Downloader.Function;
using Prices.Downloader.Services;
using Prices.Persistence;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Prices.Downloader.Function;

internal class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        ILogger logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .MinimumLevel.Override("Azure", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Literate,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} ({SourceContext}){NewLine}{Exception}"
            )
            .CreateLogger();

        builder.Services.AddLogging(logging => logging.AddSerilog(logger));
        builder.Services.AddSingleton(logger);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .AddEnvironmentVariables()
            .Build();

        var settings = configuration.Get<Settings>();

        builder.Services
            .AddEntityFrameworkServices(settings)
            .AddPricesDownloaderServices(settings)
            .AddAzureBlobStorageServices()
            ;
    }
}