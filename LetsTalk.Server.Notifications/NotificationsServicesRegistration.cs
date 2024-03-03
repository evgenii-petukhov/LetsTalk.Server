using KafkaFlow;
using KafkaFlow.Serializer;
using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Logging;
using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Handlers;
using LetsTalk.Server.Notifications.Services;

namespace LetsTalk.Server.Notifications;

public static class NotificationsServicesRegistration
{
    public static IServiceCollection AddNotificationsServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddAuthenticationClientServices(configuration);
        services.AddControllers();
        services.AddSignalR(o => o.EnableDetailedErrors = true);
        services.AddCors(options =>
        {
            options.AddPolicy("all", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        services.AddLoggingServices();
        services.AddKafka(kafka => kafka
            .UseConsoleLog()
            .AddCluster(cluster => cluster
                .WithBrokers(new[]
                {
                    kafkaSettings.Url
                })
                .CreateTopicIfNotExists(kafkaSettings.MessageNotification!.Topic, 1, 1)
                .CreateTopicIfNotExists(kafkaSettings.LinkPreviewNotification!.Topic, 1, 1)
                .CreateTopicIfNotExists(kafkaSettings.ImagePreviewNotification!.Topic, 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic(kafkaSettings.MessageNotification.Topic)
                    .WithGroupId(kafkaSettings.MessageNotification.GroupId)
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddSerializer<JsonCoreSerializer>()
                        .AddTypedHandlers(h => h.AddHandler<NotificationHandler<MessageDto>>().WithHandlerLifetime(InstanceLifetime.Transient))))
                .AddConsumer(consumer => consumer
                    .Topic(kafkaSettings.LinkPreviewNotification.Topic)
                    .WithGroupId(kafkaSettings.LinkPreviewNotification.GroupId)
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddSerializer<JsonCoreSerializer>()
                        .AddTypedHandlers(h => h.AddHandler<NotificationHandler<LinkPreviewDto>>().WithHandlerLifetime(InstanceLifetime.Transient))))
                .AddConsumer(consumer => consumer
                    .Topic(kafkaSettings.ImagePreviewNotification.Topic)
                    .WithGroupId(kafkaSettings.ImagePreviewNotification.GroupId)
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddSerializer<JsonCoreSerializer>()
                        .AddTypedHandlers(h => h.AddHandler<NotificationHandler<ImagePreviewDto>>().WithHandlerLifetime(InstanceLifetime.Transient))))));

        return services;
    }
}
