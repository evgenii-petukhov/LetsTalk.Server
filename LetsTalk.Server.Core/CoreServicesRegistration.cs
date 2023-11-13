using KafkaFlow;
using KafkaFlow.Serializer;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Attributes;
using LetsTalk.Server.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using StackExchange.Redis;
using LetsTalk.Server.Core.Services.Cache.Messages;
using LetsTalk.Server.DependencyInjection;
using LetsTalk.Server.Persistence.EntityFrameworkServices;
using LetsTalk.Server.Core.Services.Cache.Contacts;
using LetsTalk.Server.Core.Services.Cache.Profile;

namespace LetsTalk.Server.Core;

public static class CoreServicesRegistration
{
    public static IServiceCollection AddCoreServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IOpenAuthProvider, FacebookOpenAuthProvider>();
        services.AddScoped<IOpenAuthProvider, VkOpenAuthProvider>();
        services.AddScoped<IRegexService, RegexService>();
        services.AddScoped<IHtmlGenerator, HtmlGenerator>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IAccountDataLayerService, AccountDataLayerService>();
        services.AddScoped<IOpenAuthProviderResolver<string>, OpenAuthProviderResolver<string, OpenAuthProviderIdAttribute>>();

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

            services.AddScoped<IMessageService, MessageService>();
            services.DecorateScoped<IMessageService, MessageRedisCacheService>();
            services.AddScoped<IMessageCacheManager, MessageRedisCacheService>();

            services.AddScoped<IContactsService, ContactsService>();
            services.DecorateScoped<IContactsService, ContactsRedisCacheService>();

            services.AddScoped<IProfileService, ProfileService>();
            services.DecorateScoped<IProfileService, ProfileRedisCacheService>();
            services.AddScoped<IProfileCacheManager, ProfileRedisCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddScoped<IMessageService, MessageService>();
            services.DecorateScoped<IMessageService, MessageMemoryCacheService>();
            services.AddScoped<IMessageCacheManager, MessageMemoryCacheService>();
            services.AddScoped<IContactsService, ContactsService>();
            services.DecorateScoped<IContactsService, ContactsMemoryCacheService>();
        }

        services.AddEntityFrameworkServices(configuration, Assembly.GetExecutingAssembly());

        return services;
    }
}
