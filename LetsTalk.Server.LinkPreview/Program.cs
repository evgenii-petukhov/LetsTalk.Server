using KafkaFlow;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.LinkPreview;
using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.LinkPreview.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

using var host = CreateDefaultBuilder().Build();
using var serviceScope = host.Services.CreateScope();

var provider = serviceScope.ServiceProvider;
var bus = provider.CreateKafkaBus();

await bus.StartAsync();
AppDomain.CurrentDomain.ProcessExit += new EventHandler(async (sender, e) =>
{
    await bus.StopAsync();
});
await host.RunAsync();
await bus.StopAsync();

// https://thecodeblogger.com/2021/05/04/how-to-use-appsettings-json-config-file-with-net-console-applications/
static IHostBuilder CreateDefaultBuilder()
{
    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(app =>
        {
            var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            app.AddJsonFile(filename, optional: false);
        })
        .ConfigureServices((context, services) =>
        {
            var kafkaSettings = ConfigurationHelper.GetKafkaSettings(context.Configuration);

            services.AddTransient<IDownloadService, DownloadService>();
            services.AddTransient<IRegexService, RegexService>();
            services.AddConfigurationServices(context.Configuration);
            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[]
                            {
                                kafkaSettings.Url
                            })
                            .CreateTopicIfNotExists(kafkaSettings.LinkPreviewRequest!.Topic, 1, 1)
                            .CreateTopicIfNotExists(kafkaSettings.LinkPreviewNotification!.Topic, 1, 1)
                            .AddConsumer(consumer => consumer
                                .Topic(kafkaSettings.LinkPreviewRequest.Topic)
                                .WithGroupId(kafkaSettings.LinkPreviewRequest.GroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .AddMiddlewares(middlewares => middlewares
                                    .AddSerializer<JsonCoreSerializer>()
                                    .AddTypedHandlers(h => h.AddHandler<LinkPreviewRequestHandler>())
                                )
                            )
                            .AddProducer(
                                kafkaSettings.LinkPreviewNotification.Producer,
                                producer => producer
                                    .DefaultTopic(kafkaSettings.LinkPreviewNotification.Topic)
                                    .AddMiddlewares(m =>
                                        m.AddSerializer<JsonCoreSerializer>()
                                    )
                            )
                    )
            );
        })
        .UseSerilog((context, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console();
        });
}