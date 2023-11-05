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
        if (string.Equals(configuration.GetValue<string>("Caching:cachingMode"), "redis", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnectionString")!));
            services.AddScoped<IImageCacheManager, RedisCacheImageService>();
            services.DecorateScoped<IImageService, RedisCacheImageService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddScoped<IImageCacheManager, MemoryCacheImageService>();
            services.DecorateScoped<IImageService, MemoryCacheImageService>();
        }

        return services;
    }
}
