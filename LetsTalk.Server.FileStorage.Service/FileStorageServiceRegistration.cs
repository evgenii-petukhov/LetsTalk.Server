using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.GrpcInterceptors;
using LetsTalk.Server.FileStorage.Service.Services;
using LetsTalk.Server.Logging;
using LetsTalk.Server.FileStorage.Utility;
using LetsTalk.Server.ImageProcessing.Utility;
using System.Reflection;
using KafkaFlow;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using StackExchange.Redis;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Service.Services.Cache;
using LetsTalk.Server.DependencyInjection;

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
        services.AddScoped<IImageValidationService, ImageValidationService>();
        services.AddScoped<IIOService, IOService>();
        services.AddAuthenticationClientServices(configuration);
        services.AddLoggingServices();
        services.AddImageProcessingUtilityServices();
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[]
                        {
                                kafkaSettings.Url
                        })
                        .CreateTopicIfNotExists(kafkaSettings.RemoveImageRequest!.Topic, 1, 1)
                        .AddConsumer(consumer => consumer
                            .Topic(kafkaSettings.RemoveImageRequest.Topic)
                            .WithGroupId(kafkaSettings.RemoveImageRequest.GroupId)
                            .WithBufferSize(100)
                            .WithWorkersCount(10)
                            .AddMiddlewares(middlewares => middlewares
                                .AddSerializer<JsonCoreSerializer>()
                                .AddTypedHandlers(h => h.AddHandler<RemoveImageRequestHandler>().WithHandlerLifetime(InstanceLifetime.Transient))
                            )
                        )
                )
        );
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.Configure<CachingSettings>(configuration.GetSection("Caching"));

        services.AddFileStorageUtilityServices(configuration, Assembly.GetExecutingAssembly());

        switch (configuration.GetValue<string>("Caching:cachingMode"))
        {
            case "redis":
                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
                services.AddScoped<IImageCacheManager, ImageRedisCacheService>();
                services.DecorateScoped<IImageService, ImageRedisCacheService>();
                break;
            default:
                services.AddMemoryCache();
                services.AddScoped<IImageCacheManager, ImageMemoryCacheService>();
                services.DecorateScoped<IImageService, ImageMemoryCacheService>();
                break;
        }

        return services;
    }
}
