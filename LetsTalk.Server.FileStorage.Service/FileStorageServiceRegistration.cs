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
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Service.Services.Cache;
using LetsTalk.Server.DependencyInjection;
using LetsTalk.Server.SignPackage;
using LetsTalk.Server.Persistence.Redis;
using MassTransit;
using LetsTalk.Server.Kafka.Models;

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
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<RemoveImageRequestConsumer>();

                rider.UsingKafka((context, k) =>
                {
                    k.Host(kafkaSettings.Url);

                    k.TopicEndpoint<RemoveImageRequest>(
                        kafkaSettings.RemoveImageRequest!.Topic,
                        kafkaSettings.RemoveImageRequest.GroupId,
                        e =>
                        {
                            e.ConfigureConsumer<RemoveImageRequestConsumer>(context);
                            e.CreateIfMissing();
                        });
                });
            });
        });

        services.Configure<CachingSettings>(configuration.GetSection("Caching"));
        services.AddFileStorageUtilityServices();
        services.AddSignPackageServices(configuration);

        switch (configuration.GetValue<string>("Features:cachingMode"))
        {
            case "redis":
                services.AddRedisCache();
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
