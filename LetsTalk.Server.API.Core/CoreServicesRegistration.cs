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
using Confluent.Kafka;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.API.Core.Services.Cache.Accounts;
using LetsTalk.Server.API.Core.Services.Cache.IceServerConfiguration;

namespace LetsTalk.Server.API.Core;

public static class CoreServicesRegistration
{
    public static async Task<IServiceCollection> AddCoreServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(options => options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<IRegexService, RegexService>();
        services.AddScoped<IHtmlGenerator, HtmlGenerator>();
        services.AddScoped<IIceServerConfigurationService, IceServerConfigurationService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddMassTransit(x =>
        {
            var topicSettings = ConfigurationHelper.GetTopicSettings(configuration);
            if (configuration.GetValue<string>("Features:EventBrokerMode") == "aws")
            {
                x.UsingAmazonSqs((_, configure) =>
                {
                    var awsSettings = ConfigurationHelper.GetAwsSettings(configuration);
                    configure.Host(awsSettings.Region, h =>
                    {
                        h.AccessKey(awsSettings.AccessKey);
                        h.SecretKey(awsSettings.SecretKey);
                    });

                    configure.Message<Notification>(x => x.SetEntityName(topicSettings.Notification!));
                    configure.Message<LinkPreviewRequest>(x => x.SetEntityName(topicSettings.LinkPreviewRequest!));
                    configure.Message<ImageResizeRequest>(x => x.SetEntityName(topicSettings.ImageResizeRequest!));
                    configure.Message<RemoveImageRequest>(x => x.SetEntityName(topicSettings.RemoveImageRequest!));
                    configure.Message<SendEmailRequest>(x => x.SetEntityName(topicSettings.SendEmailRequest!));
                });
                services.AddScoped(typeof(IProducer<>), typeof(SqsProducer<>));
            }
            else
            {
                x.UsingInMemory();
                services.AddScoped(typeof(IProducer<>), typeof(KafkaProducer<>));
                x.AddRider(rider =>
                {
                    var kafkaSettings = ConfigurationHelper.GetKafkaSettings(configuration);
                    var defaultProducerConfig = new ProducerConfig
                    {
                        AllowAutoCreateTopics = true,
                    };
                    rider.UsingKafka((_, configure) => configure.Host(kafkaSettings.Url));
                    rider.AddProducer<string, Notification>(topicSettings.Notification, defaultProducerConfig);
                    rider.AddProducer<string, LinkPreviewRequest>(topicSettings.LinkPreviewRequest, defaultProducerConfig);
                    rider.AddProducer<string, ImageResizeRequest>(topicSettings.ImageResizeRequest, defaultProducerConfig);
                    rider.AddProducer<string, RemoveImageRequest>(topicSettings.RemoveImageRequest, defaultProducerConfig);
                    rider.AddProducer<string, SendEmailRequest>(topicSettings.SendEmailRequest, defaultProducerConfig);
                });
            }
        });
        services.Configure<CachingSettings>(configuration.GetSection("Caching"));
        services.Configure<RtcSettings>(configuration.GetSection("Rtc"));
        services.Configure<CloudflareSettings>(configuration.GetSection("Cloudflare"));

        switch (configuration.GetValue<string>("Features:CachingMode"))
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
                services.AddScoped<IAccountCacheManager, AccountRedisCacheService>();

                services.AddScoped<IProfileService, ProfileService>();
                services.DecorateScoped<IProfileService, ProfileRedisCacheService>();
                services.AddScoped<IProfileCacheManager, ProfileRedisCacheService>();

                services.AddScoped<IIceServerConfigurationService, IceServerConfigurationService>();
                services.DecorateScoped<IIceServerConfigurationService, IceServerConfigurationRedisCacheService>();
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
                services.AddScoped<IAccountCacheManager, AccountMemoryCacheService>();

                services.AddScoped<IProfileService, ProfileService>();
                services.DecorateScoped<IProfileService, ProfileMemoryCacheService>();
                services.AddScoped<IProfileCacheManager, ProfileMemoryCacheService>();

                services.AddScoped<IIceServerConfigurationService, IceServerConfigurationService>();
                services.DecorateScoped<IIceServerConfigurationService, IceServerConfigurationMemoryCacheService>();
                break;
        }

        services.AddHttpClient(nameof(IceServerConfigurationService));

        await services.AddPersistenceAgnosticServices(configuration);

        return services;
    }
}
