using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.LinkPreview.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.SignPackage;
using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Services;
using LetsTalk.Server.DependencyInjection;
using MassTransit;
using LetsTalk.Server.Kafka.Models;

namespace LetsTalk.Server.LinkPreview;

public static class LinkPreviewServiceRegistration
{
    public static IServiceCollection AddLinkPreviewServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<LinkPreviewRequestConsumer>();

                rider.UsingKafka((context, k) =>
                {
                    k.Host(kafkaSettings.Url);

                    k.TopicEndpoint<LinkPreviewRequest>(
                        kafkaSettings.LinkPreviewRequest!.Topic,
                        kafkaSettings.LinkPreviewRequest.GroupId,
                        e =>
                        {
                            e.ConfigureConsumer<LinkPreviewRequestConsumer>(context);
                            e.CreateIfMissing();
                        });
                });
            });
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
        switch (configuration.GetValue<string>("Features:linkPreview"))
        {
            case "aws":
                services.Configure<AwsSettings>(configuration.GetSection("Aws"));
                services.AddScoped<ILinkPreviewService, LambdaLinkPreviewGenerator>();
                break;
            default:
                services.AddScoped<IDownloadService, DownloadService>();
                services.AddScoped<IRegexService, RegexService>();
                services.AddScoped<ILinkPreviewService, LinkPreviewService>();
                services.DecorateScoped<ILinkPreviewService, LinkPreviewGenerator>();
                break;
        }

        return services;
    }
}
