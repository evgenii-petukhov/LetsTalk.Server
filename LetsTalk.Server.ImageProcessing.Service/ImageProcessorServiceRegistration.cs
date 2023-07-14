using KafkaFlow;
using LetsTalk.Server.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.Persistence;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility;
using LetsTalk.Server.ImageProcessing.Utility;

namespace LetsTalk.Server.ImageProcessing.Service;

public static class ImageProcessorServiceRegistration
{
    public static IServiceCollection AddImageProcessorServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);
        services.AddPersistenceServices(configuration);
        services.AddFileStorageUtilityServices();
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
                        .AddConsumer(consumer => consumer
                            .Topic(kafkaSettings.ImageResizeRequest.Topic)
                            .WithGroupId(kafkaSettings.ImageResizeRequest.GroupId)
                            .WithBufferSize(100)
                            .WithWorkersCount(10)
                            .AddMiddlewares(middlewares => middlewares
                                .AddSerializer<JsonCoreSerializer>()
                                .AddTypedHandlers(h => h.AddHandler<ImageResizeRequestHandler>())
                            )
                        )
                )
        );
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));

        return services;
    }
}
