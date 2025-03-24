using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.GrpcInterceptors;
using LetsTalk.Server.FileStorage.Service.Services;
using LetsTalk.Server.Logging;
using LetsTalk.Server.FileStorage.AgnosticServices;
using LetsTalk.Server.ImageProcessing.ImageResizeEngine;
using System.Reflection;
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
        services.AddScoped<IImageStorageService, ImageStorageService>();
        services.AddScoped<IImageValidationService, ImageValidationService>();
        services.AddAuthenticationClientServices(configuration);
        services.AddLoggingServices();
        services.AddImageResizeEngineServices();
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMassTransit(x =>
        {
            if (configuration.GetValue<string>("Features:EventBrokerMode") == "aws")
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
        services.AddFileStorageAgnosticServices(configuration);
        services.AddSignPackageServices(configuration);

        switch (configuration.GetValue<string>("Features:CachingMode"))
        {
            case "redis":
                services.AddRedisCache();
                services.AddScoped<IImageStorageCacheManager, ImageStorageRedisCacheService>();
                services.DecorateScoped<IImageStorageService, ImageStorageRedisCacheService>();
                break;
            default:
                services.AddMemoryCache();
                services.AddScoped<IImageStorageCacheManager, ImageStorageMemoryCacheService>();
                services.DecorateScoped<IImageStorageService, ImageStorageMemoryCacheService>();
                break;
        }

        return services;
    }
}
