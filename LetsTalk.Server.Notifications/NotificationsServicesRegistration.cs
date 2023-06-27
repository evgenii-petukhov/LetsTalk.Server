using KafkaFlow;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
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
        services.AddTransient<INotificationService, NotificationService>();
        services.Configure<AuthenticationSettings>(configuration.GetSection("AuthenticationSettings"));
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
        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(new[]
                        {
                    kafkaSettings.Url
                        })
                        .CreateTopicIfNotExists(kafkaSettings.MessageNotification!.Topic, 1, 1)
                        .CreateTopicIfNotExists(kafkaSettings.LinkPreviewNotification!.Topic, 1, 1)
                        .AddConsumer(consumer => consumer
                            .Topic(kafkaSettings.MessageNotification.Topic)
                            .WithGroupId(kafkaSettings.MessageNotification.GroupId)
                            .WithBufferSize(100)
                            .WithWorkersCount(10)
                            .AddMiddlewares(middlewares => middlewares
                                .AddSerializer<JsonCoreSerializer>()
                                .AddTypedHandlers(h => h.AddHandler<MessageNotificationHandler>())
                            )
                        )
                        .AddConsumer(consumer => consumer
                            .Topic(kafkaSettings.LinkPreviewNotification.Topic)
                            .WithGroupId(kafkaSettings.LinkPreviewNotification.GroupId)
                            .WithBufferSize(100)
                            .WithWorkersCount(10)
                            .AddMiddlewares(middlewares => middlewares
                                .AddSerializer<JsonCoreSerializer>()
                                .AddTypedHandlers(h => h.AddHandler<LinkPreviewNotificationHandler>())
                            )
                        )
                )
        );

        return services;
    }
}
