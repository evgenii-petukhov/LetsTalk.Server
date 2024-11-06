using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Logging;
using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Handlers;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Notifications.Services;
using MassTransit;

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
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<NotificationConsumer<MessageDto>>();
                rider.AddConsumer<NotificationConsumer<LinkPreviewDto>>();
                rider.AddConsumer<NotificationConsumer<ImagePreviewDto>>();

                rider.UsingKafka((context, k) =>
                {
                    k.Host(kafkaSettings.Url);

                    k.TopicEndpoint<Notification<MessageDto>>(
                        kafkaSettings.MessageNotification!.Topic,
                        kafkaSettings.MessageNotification.GroupId,
                        e =>
                        {
                            e.ConfigureConsumer<NotificationConsumer<MessageDto>>(context);
                            e.CreateIfMissing();
                        });

                    k.TopicEndpoint<Notification<LinkPreviewDto>>(
                        kafkaSettings.LinkPreviewNotification!.Topic,
                        kafkaSettings.LinkPreviewNotification.GroupId,
                        e =>
                        {
                            e.ConfigureConsumer<NotificationConsumer<LinkPreviewDto>>(context);
                            e.CreateIfMissing();
                        });

                    k.TopicEndpoint<Notification<ImagePreviewDto>>(
                        kafkaSettings.ImagePreviewNotification!.Topic,
                        kafkaSettings.ImagePreviewNotification.GroupId,
                        e =>
                        {
                            e.ConfigureConsumer<NotificationConsumer<ImagePreviewDto>>(context);
                            e.CreateIfMissing();
                        });
                });
            });
        });

        return services;
    }
}
