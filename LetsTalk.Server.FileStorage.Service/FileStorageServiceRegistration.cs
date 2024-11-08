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
using System.Net.Mime;

namespace LetsTalk.Server.FileStorage.Service;

public static class FileStorageServiceRegistration
{
    public static IServiceCollection AddFileStorageServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
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
            if (configuration.GetValue<string>("Features:EventBrokerMode") == "Aws")
            {
                x.AddConsumer<RemoveImageRequestConsumer>();

                x.UsingAmazonSqs((context, configure) =>
                {
                    var awsSettings = ConfigurationHelper.GetAwsSettings(configuration);
                    var queueSettings = ConfigurationHelper.GetQueueSettings(configuration);
                    configure.Host(awsSettings.Region, h =>
                    {
                        h.AccessKey(awsSettings.AccessKey);
                        h.SecretKey(awsSettings.SecretKey);
                    });
                    configure.WaitTimeSeconds = 20;
                    configure.ReceiveEndpoint(queueSettings.RemoveImageRequest!, e =>
                    {
                        e.WaitTimeSeconds = 20;
                        e.DefaultContentType = new ContentType("application/json");
                        e.UseRawJsonDeserializer();
                        e.ConfigureConsumeTopology = false;
                        e.ConfigureConsumer<RemoveImageRequestConsumer>(context);
                    });
                });
            }
            else
            {
                x.UsingInMemory();
                x.AddRider(rider =>
                {
                    rider.AddConsumer<RemoveImageRequestConsumer>();

                    rider.UsingKafka((context, k) =>
                    {
                        var kafkaSettings = ConfigurationHelper.GetKafkaSettings(configuration);
                        var topicSettings = ConfigurationHelper.GetTopicSettings(configuration);
                        k.Host(kafkaSettings.Url);
                        k.TopicEndpoint<RemoveImageRequest>(
                            topicSettings.RemoveImageRequest,
                            kafkaSettings.GroupId,
                            e =>
                            {
                                e.ConfigureConsumer<RemoveImageRequestConsumer>(context);
                                e.CreateIfMissing();
                            });
                    });
                });
            }
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
