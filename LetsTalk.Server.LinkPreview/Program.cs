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
            services.AddKafka(
                kafka => kafka
                    .UseConsoleLog()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[]
                            {
                                kafkaSettings.Url
                            })
                            .CreateTopicIfNotExists(kafkaSettings.LinkPreviewTopic, 1, 1)
                            .AddConsumer(consumer => consumer
                                .Topic(kafkaSettings.LinkPreviewTopic)
                                .WithGroupId(kafkaSettings.LinkPreviewGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(10)
                                .AddMiddlewares(middlewares => middlewares
                                    .AddSerializer<JsonCoreSerializer>()
                                    .AddTypedHandlers(h => h.AddHandler<LinkPreviewRequestHandler>())
                                )
                            )
                    )
            );
        });
}