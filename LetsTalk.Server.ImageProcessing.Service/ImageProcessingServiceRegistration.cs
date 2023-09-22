using KafkaFlow;
using LetsTalk.Server.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility;
using LetsTalk.Server.ImageProcessing.Utility;
using System.Reflection;

namespace LetsTalk.Server.ImageProcessing.Service;

public static class ImageProcessingServiceRegistration
{
    public static IServiceCollection AddImageProcessingServiceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);
        services.AddFileStorageUtilityServices(configuration, Assembly.GetExecutingAssembly());
        services.AddImageProcessingUtilityServices();
        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[]
                        {
                                kafkaSettings.Url
                        })
                        .CreateTopicIfNotExists(kafkaSettings.ImageResizeRequest!.Topic, 1, 1)
                        .CreateTopicIfNotExists(kafkaSettings.ImagePreviewNotification!.Topic, 1, 1)
                        .CreateTopicIfNotExists(kafkaSettings.SetImageDimensionsRequest!.Topic, 1, 1)
                        .AddConsumer(consumer => consumer
                            .Topic(kafkaSettings.ImageResizeRequest.Topic)
                            .WithGroupId(kafkaSettings.ImageResizeRequest.GroupId)
                            .WithBufferSize(100)
                            .WithWorkersCount(10)
                            .AddMiddlewares(middlewares => middlewares
                                .AddSerializer<JsonCoreSerializer>()
                                .AddTypedHandlers(h => h.AddHandler<ImageResizeRequestHandler>().WithHandlerLifetime(InstanceLifetime.Transient))
                            )
                        )
                        .AddConsumer(consumer => consumer
                            .Topic(kafkaSettings.SetImageDimensionsRequest.Topic)
                            .WithGroupId(kafkaSettings.SetImageDimensionsRequest.GroupId)
                            .WithBufferSize(100)
                            .WithWorkersCount(10)
                            .AddMiddlewares(middlewares => middlewares
                                .AddSerializer<JsonCoreSerializer>()
                                .AddTypedHandlers(h => h.AddHandler<SetImageDimensionsRequestHandler>().WithHandlerLifetime(InstanceLifetime.Transient))
                            )
                        )
                        .AddProducer(
                            kafkaSettings.ImagePreviewNotification.Producer,
                            producer => producer
                                .DefaultTopic(kafkaSettings.ImagePreviewNotification.Topic)
                                .AddMiddlewares(m =>
                                    m.AddSerializer<JsonCoreSerializer>()
                                )
                        )
                )
        );
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));

        return services;
    }
}
