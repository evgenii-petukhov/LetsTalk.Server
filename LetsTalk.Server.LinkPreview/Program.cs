using LetsTalk.Server.LinkPreview;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

using var host = CreateDefaultBuilder().Build();
using var serviceScope = host.Services.CreateScope();

var provider = serviceScope.ServiceProvider;
await host.RunAsync();

// https://thecodeblogger.com/2021/05/04/how-to-use-appsettings-json-config-file-with-net-console-applications/
static IHostBuilder CreateDefaultBuilder()
{
    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(app =>
        {
            var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            app.AddJsonFile(filename, optional: false);
        })
        .ConfigureServices((context, services) => services.AddLinkPreviewServices(context.Configuration))
        .UseSerilog((context, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console();
        });
}