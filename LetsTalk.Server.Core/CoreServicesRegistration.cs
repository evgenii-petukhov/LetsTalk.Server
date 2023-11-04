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
using StackExchange.Redis;
using LetsTalk.Server.Core.Services.Cache.Messages;
using LetsTalk.Server.DependencyInjection;

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
        services.AddTransient<IMessageService, MessageService>();
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
                    kafkaSettings.RemoveImageRequest.Producer,
                    producer => producer
                        .DefaultTopic(kafkaSettings.RemoveImageRequest.Topic)
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>()))
        ));
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.Configure<CachingSettings>(configuration.GetSection("Caching"));

        if (string.Equals(configuration.GetValue<string>("Caching:cachingMode"), "redis", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnectionString")!));
            services.AddTransient<IMessageService, MessageService>();
            services.DecorateTransient<IMessageService, RedisCacheMessageService>();
            services.AddTransient<IMessageCacheManager, RedisCacheMessageService>();
            services.AddTransient<IAccountService, AccountService>();
            services.DecorateTransient<IAccountService, RedisCacheAccountService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddTransient<IMessageService, MessageService>();
            services.DecorateTransient<IMessageService, MemoryCacheMessageService>();
            services.AddTransient<IMessageCacheManager, MemoryCacheMessageService>();
            services.AddTransient<IAccountService, AccountService>();
            services.DecorateTransient<IAccountService, MemoryCacheAccountService>();
        }

        return services;
    }
}
