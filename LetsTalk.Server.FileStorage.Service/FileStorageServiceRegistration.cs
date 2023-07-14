using KafkaFlow;
using KafkaFlow.Serializer;
using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.GrpcInterceptors;
using LetsTalk.Server.FileStorage.Service.Services;
using LetsTalk.Server.Logging;
using LetsTalk.Server.FileStorage.Utility;
using LetsTalk.Server.ImageProcessing;

namespace LetsTalk.Server.FileStorage.Service;

public static class FileStorageServiceRegistration
{
    public static IServiceCollection AddFileStorageServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);

        services.AddCors(options =>
        {
            options.AddPolicy("all", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        services.AddGrpc(options => options.Interceptors.Add<JwtInterceptor>());
        services.AddGrpcReflection();
        services.AddTransient<IImageValidationService, ImageValidationService>();
        services.AddTransient<IIOService, IOService>();
        services.AddAuthenticationClientServices(configuration);
        services.AddLoggingServices();
        services.AddFileStorageUtilityServices();
        services.AddImageProcessingServices();

        services.AddKafka(kafka => kafka
            .UseConsoleLog()
            .AddCluster(cluster => cluster
                .WithBrokers(new[]
                {
                    kafkaSettings.Url
                })
                .CreateTopicIfNotExists(kafkaSettings.ImageResizeRequest!.Topic, 1, 1)
                .AddProducer(
                    kafkaSettings.ImageResizeRequest.Producer,
                    producer => producer
                        .DefaultTopic(kafkaSettings.ImageResizeRequest.Topic)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>()))
        ));
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));

        return services;
    }
}
