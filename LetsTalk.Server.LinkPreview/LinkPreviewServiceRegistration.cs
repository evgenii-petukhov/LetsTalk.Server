using KafkaFlow;
using KafkaFlow.Serializer;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.LinkPreview.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.Persistence.AgnosticServices;
using System.Reflection;
using LetsTalk.Server.LinkPreview.Utility;

namespace LetsTalk.Server.LinkPreview;

public static class LinkPreviewServiceRegistration
{
    public static IServiceCollection AddLinkPreviewServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);
        services.AddLinkPreviewUtility();
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
                        .CreateTopicIfNotExists(kafkaSettings.ClearMessageCacheRequest!.Topic, 1, 1)
                        .AddConsumer(consumer => consumer
                            .Topic(kafkaSettings.LinkPreviewRequest.Topic)
                            .WithGroupId(kafkaSettings.LinkPreviewRequest.GroupId)
                            .WithBufferSize(100)
                            .WithWorkersCount(10)
                            .AddMiddlewares(middlewares => middlewares
                                .AddDeserializer<JsonCoreDeserializer>()
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
                        .AddProducer(
                            kafkaSettings.ClearMessageCacheRequest.Producer,
                            producer => producer
                                .DefaultTopic(kafkaSettings.ClearMessageCacheRequest.Topic)
                                .AddMiddlewares(m =>
                                    m.AddSerializer<JsonCoreSerializer>()
                                )
                        )
                )
        );
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.AddAgnosticServices(configuration);
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
