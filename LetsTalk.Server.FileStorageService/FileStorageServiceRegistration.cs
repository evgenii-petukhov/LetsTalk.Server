using KafkaFlow;
using KafkaFlow.Serializer;
using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.FileStorageService.GrpcInterceptors;
using LetsTalk.Server.FileStorageService.Services;
using LetsTalk.Server.Logging;

namespace LetsTalk.Server.FileStorageService;

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
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IFileNameGenerator, FileNameGenerator>();
        services.AddTransient<IImageInfoService, ImageInfoService>();
        services.AddTransient<IImageService, ImageService>();
        services.AddTransient<IImageValidationService, ImageValidationService>();
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
        services.AddAuthenticationClientServices(configuration);
        services.AddLoggingServices();
        services.AddMemoryCache();

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

        return services;
    }
}
