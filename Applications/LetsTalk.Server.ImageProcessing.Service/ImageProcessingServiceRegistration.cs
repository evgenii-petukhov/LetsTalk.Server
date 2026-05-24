using LetsTalk.Server.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.AgnosticServices;
using LetsTalk.Server.ImageProcessing.Utility;
using MassTransit;
using LetsTalk.Server.Models.Kafka;
using System.Net.Mime;
using LetsTalk.Server.ImageProcessing.ImageResizeEngine;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions;

namespace LetsTalk.Server.ImageProcessing.Service;

public static class ImageProcessingServiceRegistration
{
    public static IServiceCollection AddImageProcessingServiceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddFileStorageAgnosticServices(configuration);
        services.AddImageResizeEngineServices();
        services.AddMassTransit(x =>
        {
            if (configuration.GetValue<string>("Features:EventBrokerMode") == "aws")
            {
                x.AddConsumer<ImageResizeRequestConsumer>();

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
                    configure.ReceiveEndpoint(queueSettings.ImageResizeRequest!, e =>
                    {
                        e.WaitTimeSeconds = 20;
                        e.DefaultContentType = new ContentType("application/json");
                        e.UseRawJsonDeserializer();
                        e.ConfigureConsumeTopology = false;
                        e.ConfigureConsumer<ImageResizeRequestConsumer>(context);
                    });
                });
            }
            else
            {
                x.UsingInMemory();
                x.AddRider(rider =>
                {
                    rider.AddConsumer<ImageResizeRequestConsumer>();
                    rider.UsingKafka((context, k) =>
                    {
                        var kafkaSettings = ConfigurationHelper.GetKafkaSettings(configuration);
                        var topicSettings = ConfigurationHelper.GetTopicSettings(configuration);
                        k.Host(kafkaSettings.Url);
                        k.TopicEndpoint<ImageResizeRequest>(
                            topicSettings.ImageResizeRequest,
                            kafkaSettings.GroupId,
                            e =>
                            {
                                e.ConfigureConsumer<ImageResizeRequestConsumer>(context);
                                e.CreateIfMissing();
                            });
                    });
                });
            }
        });

        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
        services.Configure<ApplicationUrlSettings>(configuration.GetSection("ApplicationUrls"));
        services.AddHttpClient(nameof(ImageResizeRequestConsumer)).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
#if DEBUG
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
#endif
        });

        services.AddScoped<IImageProcessingService, ImageProcessingService>();

        return services;
    }
}
