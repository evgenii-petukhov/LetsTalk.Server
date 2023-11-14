using KafkaFlow;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.LinkPreview.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.Persistence.EntityFrameworkServices;
using System.Reflection;

namespace LetsTalk.Server.LinkPreview;

public static class LinkPreviewServiceRegistration
{
    public static IServiceCollection AddLinkPreviewServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);
        services.AddHttpClient(nameof(DownloadService));
        services.AddScoped<IDownloadService, DownloadService>();
        services.AddScoped<IRegexService, RegexService>();
        services.AddScoped<ILinkPreviewGenerator, LinkPreviewGenerator>();
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
                                .AddTypedHandlers(h => h.AddHandler<LinkPreviewRequestHandler>().WithHandlerLifetime(InstanceLifetime.Transient))
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
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.AddEntityFrameworkServices(configuration, Assembly.GetExecutingAssembly());

        return services;
    }
}
