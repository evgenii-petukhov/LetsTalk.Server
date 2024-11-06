using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using LetsTalk.Server.API.Core.Services.Cache.Messages;
using LetsTalk.Server.DependencyInjection;
using LetsTalk.Server.API.Core.Services.Cache.Chats;
using LetsTalk.Server.API.Core.Services.Cache.Profile;
using LetsTalk.Server.Persistence.AgnosticServices;
using LetsTalk.Server.Persistence.Redis;
using MassTransit;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Notifications.Models;
using Confluent.Kafka;
using LetsTalk.Server.Kafka.Models;

namespace LetsTalk.Server.API.Core;

public static class CoreServicesRegistration
{
    public static IServiceCollection AddCoreServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(options => options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<IRegexService, RegexService>();
        services.AddScoped<IHtmlGenerator, HtmlGenerator>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<ILoginCodeGenerator, LoginCodeGenerator>();
        services.AddScoped<IEmailService, EmailService>();

        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.UsingKafka((_, k) => k.Host(kafkaSettings.Url));

                var defaultProducerConfig = new ProducerConfig
                {
                    AllowAutoCreateTopics = true,
                };

                rider.AddProducer<string, Notification<MessageDto>>(kafkaSettings.MessageNotification!.Topic, defaultProducerConfig);
                rider.AddProducer<string, Notification<LinkPreviewDto>>(kafkaSettings.LinkPreviewNotification!.Topic, defaultProducerConfig);
                rider.AddProducer<string, Notification<ImagePreviewDto>>(kafkaSettings.ImagePreviewNotification!.Topic, defaultProducerConfig);
                rider.AddProducer<string, LinkPreviewRequest>(kafkaSettings.LinkPreviewRequest!.Topic, defaultProducerConfig);
                rider.AddProducer<string, ImageResizeRequest>(kafkaSettings.ImageResizeRequest!.Topic, defaultProducerConfig);
                rider.AddProducer<string, RemoveImageRequest>(kafkaSettings.RemoveImageRequest!.Topic, defaultProducerConfig);
                rider.AddProducer<string, SendLoginCodeRequest>(kafkaSettings.SendLoginCodeRequest!.Topic, defaultProducerConfig);
            });
        });
        services.Configure<CachingSettings>(configuration.GetSection("Caching"));

        switch (configuration.GetValue<string>("Features:cachingMode"))
        {
            case "redis":
                services.AddRedisCache();

                services.AddScoped<IMessageService, MessageService>();
                services.DecorateScoped<IMessageService, MessageRedisCacheService>();
                services.AddScoped<IMessageCacheManager, MessageRedisCacheService>();

                services.AddScoped<IChatService, ChatService>();
                services.DecorateScoped<IChatService, ChatRedisCacheService>();
                services.AddScoped<IChatCacheManager, ChatRedisCacheService>();

                services.AddScoped<IAccountService, AccountService>();
                services.DecorateScoped<IAccountService, AccountRedisCacheService>();

                services.AddScoped<IProfileService, ProfileService>();
                services.DecorateScoped<IProfileService, ProfileRedisCacheService>();
                services.AddScoped<IProfileCacheManager, ProfileRedisCacheService>();
                services.AddScoped<ILoginCodeCacheService, LoginCodeRedisCacheService>();
                break;
            default:
                services.AddMemoryCache();

                services.AddScoped<IMessageService, MessageService>();
                services.DecorateScoped<IMessageService, MessageMemoryCacheService>();
                services.AddScoped<IMessageCacheManager, MessageMemoryCacheService>();

                services.AddScoped<IChatService, ChatService>();
                services.DecorateScoped<IChatService, ChatMemoryCacheService>();
                services.AddScoped<IChatCacheManager, ChatMemoryCacheService>();

                services.AddScoped<IAccountService, AccountService>();
                services.DecorateScoped<IAccountService, AccountMemoryCacheService>();

                services.AddScoped<IProfileService, ProfileService>();
                services.DecorateScoped<IProfileService, ProfileMemoryCacheService>();
                services.AddScoped<IProfileCacheManager, ProfileMemoryCacheService>();
                services.AddScoped<ILoginCodeCacheService, LoginCodeMemoryCacheService>();
                break;
        }

        services.AddAgnosticServices(configuration);

        return services;
    }
}
