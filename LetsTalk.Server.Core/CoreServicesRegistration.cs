using KafkaFlow;
using KafkaFlow.Serializer;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Attributes;
using LetsTalk.Server.Core.Services;
using LetsTalk.Server.Persistence.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LetsTalk.Server.Core;

public static class CoreServicesRegistration
{
    public static IServiceCollection AddCoreServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddTransient<IOpenAuthProvider, FacebookOpenAuthProvider>();
        services.AddTransient<IOpenAuthProvider, VkOpenAuthProvider>();
        services.AddTransient<IRegexService, RegexService>();
        services.AddTransient<IHtmlGenerator, HtmlGenerator>();
        services.AddTransient<IMessageProcessor, MessageProcessor>();
        services.AddTransient<IAccountDataLayerService, AccountDataLayerService>();
        services.AddTransient<IOpenAuthProviderResolver<string>, OpenAuthProviderResolver<string, OpenAuthProviderIdAttribute>>();
        services.AddPersistenceRepositoryServices(configuration, Assembly.GetExecutingAssembly());

        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);

        services.AddKafka(kafka => kafka
            .UseConsoleLog()
            .AddCluster(cluster => cluster
                .WithBrokers(new[]
                {
                    kafkaSettings.Url
                })
                .CreateTopicIfNotExists(kafkaSettings.MessageNotification!.Topic, 1, 1)
                .CreateTopicIfNotExists(kafkaSettings.LinkPreviewRequest!.Topic, 1, 1)
                .CreateTopicIfNotExists(kafkaSettings.ImageResizeRequest!.Topic, 1, 1)
                .CreateTopicIfNotExists(kafkaSettings.SetImageDimensionsRequest!.Topic, 1, 1)
                .CreateTopicIfNotExists(kafkaSettings.RemoveImageRequest!.Topic, 1, 1)
                .AddProducer(
                    kafkaSettings.MessageNotification.Producer,
                    producer => producer
                        .DefaultTopic(kafkaSettings.MessageNotification.Topic)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>()))
                .AddProducer(
                    kafkaSettings.LinkPreviewRequest.Producer,
                    producer => producer
                        .DefaultTopic(kafkaSettings.LinkPreviewRequest.Topic)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>()))
                .AddProducer(
                    kafkaSettings.ImageResizeRequest.Producer,
                    producer => producer
                        .DefaultTopic(kafkaSettings.ImageResizeRequest.Topic)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>()))
                .AddProducer(
                    kafkaSettings.SetImageDimensionsRequest.Producer,
                    producer => producer
                        .DefaultTopic(kafkaSettings.SetImageDimensionsRequest.Topic)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>()))
                .AddProducer(
                    kafkaSettings.RemoveImageRequest.Producer,
                    producer => producer
                        .DefaultTopic(kafkaSettings.RemoveImageRequest.Topic)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>()))
        ));
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));

        return services;
    }
}
