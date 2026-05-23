using LetsTalk.Server.ImageProcessing.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Globalization;

using var host = CreateDefaultBuilder().Build();
await host.RunAsync();

static IHostBuilder CreateDefaultBuilder()
{
    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(app =>
        {
            var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            app.AddJsonFile(filename, optional: false);
        })
        .ConfigureServices((context, services) => services.AddImageProcessingServiceServices(context.Configuration))
        .UseSerilog((context, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);
        });
}