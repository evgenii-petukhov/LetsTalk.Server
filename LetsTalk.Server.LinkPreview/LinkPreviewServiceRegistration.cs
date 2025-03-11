using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.LinkPreview.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.SignPackage;
using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Services;
using MassTransit;
using LetsTalk.Server.Kafka.Models;
using System.Net.Mime;

namespace LetsTalk.Server.LinkPreview;

public static class LinkPreviewServiceRegistration
{
    public static IServiceCollection AddLinkPreviewServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            if (configuration.GetValue<string>("Features:EventBrokerMode") == "aws")
            {
                x.AddConsumer<LinkPreviewRequestConsumer>();

                x.UsingAmazonSqs((context, configure) =>
                {
                    var awsSettings = ConfigurationHelper.GetAwsSettings(configuration);
                    configure.Host(awsSettings.Region, h =>
                    {
                        h.AccessKey(awsSettings.AccessKey);
                        h.SecretKey(awsSettings.SecretKey);
                    });
                    configure.WaitTimeSeconds = 20;
                    configure.ReceiveEndpoint("letstalk-link-preview-request-queue", e =>
                    {
                        e.WaitTimeSeconds = 20;
                        e.DefaultContentType = new ContentType("application/json");
                        e.UseRawJsonDeserializer();
                        e.ConfigureConsumeTopology = false;
                        e.ConfigureConsumer<LinkPreviewRequestConsumer>(context);
                    });
                });
            }
            else
            {
                x.UsingInMemory();

                x.AddRider(rider =>
                {
                    rider.AddConsumer<LinkPreviewRequestConsumer>();
                    rider.UsingKafka((context, k) =>
                    {
                        var kafkaSettings = ConfigurationHelper.GetKafkaSettings(configuration);
						var topicSettings = ConfigurationHelper.GetTopicSettings(configuration);
                        k.Host(kafkaSettings.Url);
                        k.TopicEndpoint<LinkPreviewRequest>(
                            topicSettings.LinkPreviewRequest,
                            kafkaSettings.GroupId,
                            e =>
                            {
                                e.ConfigureConsumer<LinkPreviewRequestConsumer>(context);
                                e.CreateIfMissing();
                            });
                    });
                });
            }
        });
        services.Configure<ApplicationUrlSettings>(configuration.GetSection("ApplicationUrls"));
        services.AddSignPackageServices(configuration);
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddHttpClient(nameof(HttpClientService)).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
#if DEBUG
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
#endif
        });
        switch (configuration.GetValue<string>("Features:LinkPreviewMode"))
        {
            case "aws":
                services.Configure<AwsSettings>(configuration.GetSection("Aws"));
                services.AddScoped<ILinkPreviewService, LambdaLinkPreviewGenerator>();
                break;
            default:
                services.AddScoped<IDownloadService, DownloadService>();
                services.AddScoped<IRegexService, RegexService>();
                services.AddScoped<ILinkPreviewService, LinkPreviewService>();
                break;
        }

        return services;
    }
}
