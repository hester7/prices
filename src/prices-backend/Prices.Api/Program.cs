//using Microsoft.Extensions.Logging.AzureAppServices;
using Prices.AzureBlobStorage;
using Prices.Core.Application.Models;
using Prices.Downloader.Services;
using Prices.GraphQl;
using Prices.Persistence;
using Prices.PriceUpdater;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.Get<Settings>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(settings.CorsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

ILogger logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console(
        theme: AnsiConsoleTheme.Literate,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} ({SourceContext}){NewLine}{Exception}"
    )
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Services.AddSingleton(logger);

//builder.Logging.AddAzureWebAppDiagnostics();
//builder.Services.Configure<AzureFileLoggerOptions>(options => {
//    options.FileName = "azure-diagnostics-";
//    options.FileSizeLimit = 50 * 1024;
//    options.RetainedFileCountLimit = 5;
//});
//builder.Services.Configure<AzureBlobLoggerOptions>(options => { options.BlobName = "log.txt"; });

builder.Services.AddEntityFrameworkServices(settings);

builder.Services.AddGraphQlServices();

builder.Services.AddAzureBlobStorageServices();
builder.Services.AddPricesDownloaderServices(settings);
builder.Services.AddPriceUpdaterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseWebSockets();
app.MapGraphQL();

app.Run();
