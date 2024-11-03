using KafkaFlow;
using KafkaFlow.Serializer;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.LinkPreview.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LetsTalk.Server.SignPackage;
using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Services;

namespace LetsTalk.Server.LinkPreview;

public static class LinkPreviewServiceRegistration
{
    public static IServiceCollection AddLinkPreviewServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);
        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[]
                        {
                                kafkaSettings.Url
                        })
                        .CreateTopicIfNotExists(kafkaSettings.LinkPreviewRequest!.Topic, 1, 1)
                        .AddConsumer(consumer => consumer
                            .Topic(kafkaSettings.LinkPreviewRequest.Topic)
                            .WithGroupId(kafkaSettings.LinkPreviewRequest.GroupId)
                            .WithBufferSize(100)
                            .WithWorkersCount(10)
                            .AddMiddlewares(middlewares => middlewares
                                .AddDeserializer<JsonCoreDeserializer>()
                                .AddTypedHandlers(h => h.AddHandler<LinkPreviewRequestHandler>().WithHandlerLifetime(InstanceLifetime.Transient))
                            )
                        )
                )
        );
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.Configure<ApplicationUrlSettings>(configuration.GetSection("ApplicationUrls"));
        services.Configure<AwsSettings>(configuration.GetSection("Aws"));
        services.AddSignPackageServices(configuration);
        services.AddHttpClient(nameof(HttpClientService));
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddScoped<IDownloadService, DownloadService>();
        services.AddScoped<IRegexService, RegexService>();
        services.AddScoped<ILinkPreviewService, LinkPreviewService>();
        services.AddHttpClient(nameof(LinkPreviewGeneratorBase)).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
#if DEBUG
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
#endif
        });
        switch (configuration.GetValue<string>("Features:linkPreview"))
        {
            case "aws":
                services.AddScoped<ILinkPreviewGenerator, LambdaLinkPreviewGenerator>();
                break;
            default:
                services.AddScoped<ILinkPreviewGenerator, LinkPreviewGenerator>();
                break;
        }

        return services;
    }
}
