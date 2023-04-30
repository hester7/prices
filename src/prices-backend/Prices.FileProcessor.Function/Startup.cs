﻿using System;
using System.Reflection;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prices.Core.Application.Models;
using Prices.Downloader.Services;
using Prices.FileProcessor.Function;
using Prices.Persistence;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Prices.FileProcessor.Function;

internal class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
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
            .AddSingleton<ILoggerProvider>((sp) =>
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .MinimumLevel.Override("Azure", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .WriteTo.Console(
                        theme: AnsiConsoleTheme.Literate,
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} ({SourceContext}){NewLine}{Exception}"
                    )
                    .WriteTo.ApplicationInsights(sp.GetRequiredService<TelemetryClient>(), TelemetryConverter.Traces)
                    .CreateLogger();
                return new SerilogLoggerProvider(Log.Logger, true);
            });
    }
}