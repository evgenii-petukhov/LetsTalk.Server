using LetsTalk.Server.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility;
using LetsTalk.Server.ImageProcessing.Utility;
using LetsTalk.Server.SignPackage;
using MassTransit;
using LetsTalk.Server.Kafka.Models;

namespace LetsTalk.Server.ImageProcessing.Service;

public static class ImageProcessingServiceRegistration
{
    public static IServiceCollection AddImageProcessingServiceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);
        services.AddFileStorageUtilityServices();
        services.AddImageProcessingUtilityServices();

        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<ImageResizeRequestConsumer>();

                rider.UsingKafka((context, k) =>
                {
                    k.Host(kafkaSettings.Url);

                    k.TopicEndpoint<ImageResizeRequest>(
                        kafkaSettings.ImageResizeRequest!.Topic,
                        kafkaSettings.ImageResizeRequest.GroupId,
                        e =>
                        {
                            e.ConfigureConsumer<ImageResizeRequestConsumer>(context);
                            e.CreateIfMissing();
                        });
                });
            });
        });

        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
        services.Configure<ApplicationUrlSettings>(configuration.GetSection("ApplicationUrls"));
        services.AddSignPackageServices(configuration);
        services.AddHttpClient(nameof(ImageResizeRequestConsumer)).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
#if DEBUG
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
#endif
        });

        return services;
    }
}
