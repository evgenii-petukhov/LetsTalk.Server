using KafkaFlow;
using LetsTalk.Server.ImageProcessing.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

using var host = CreateDefaultBuilder().Build();
using var serviceScope = host.Services.CreateScope();

var provider = serviceScope.ServiceProvider;
var bus = provider.CreateKafkaBus();

await bus.StartAsync();
await host.RunAsync();
await bus.StopAsync();

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
                .WriteTo.Console();
        });
}