using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Logging;
using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Handlers;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Notifications.Services;
using MassTransit;
using System.Net.Mime;

namespace LetsTalk.Server.Notifications;

public static class NotificationsServicesRegistration
{
    public static IServiceCollection AddNotificationsServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
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
            if (configuration.GetValue<string>("Features:EventBrokerMode") == "Aws")
            {
                x.AddConsumer<NotificationConsumer<MessageDto>>();
                x.AddConsumer<NotificationConsumer<LinkPreviewDto>>();
                x.AddConsumer<NotificationConsumer<ImagePreviewDto>>();

                x.UsingAmazonSqs((context, configure) =>
                {
                    var awsSettings = ConfigurationHelper.GetAwsSettings(configuration);
                    var queueSettings = ConfigurationHelper.GetQueueSettings(configuration);
                    configure.Host(awsSettings.Region, h =>
                    {
                        h.AccessKey(awsSettings.AccessKey);
                        h.SecretKey(awsSettings.SecretKey);
                    });
                    configure.WaitTimeSeconds = 20;
                    configure.ReceiveEndpoint(queueSettings.MessageNotification!, e =>
                    {
                        e.WaitTimeSeconds = 20;
                        e.DefaultContentType = new ContentType("application/json");
                        e.UseRawJsonDeserializer();
                        e.ConfigureConsumeTopology = false;
                        e.ConfigureConsumer<NotificationConsumer<MessageDto>>(context);
                    });
                    configure.ReceiveEndpoint(queueSettings.LinkPreviewNotification!, e =>
                    {
                        e.WaitTimeSeconds = 20;
                        e.DefaultContentType = new ContentType("application/json");
                        e.UseRawJsonDeserializer();
                        e.ConfigureConsumeTopology = false;
                        e.ConfigureConsumer<NotificationConsumer<LinkPreviewDto>>(context);
                    });
                    configure.ReceiveEndpoint(queueSettings.ImagePreviewNotification!, e =>
                    {
                        e.WaitTimeSeconds = 20;
                        e.DefaultContentType = new ContentType("application/json");
                        e.UseRawJsonDeserializer();
                        e.ConfigureConsumeTopology = false;
                        e.ConfigureConsumer<NotificationConsumer<ImagePreviewDto>>(context);
                    });
                });
            }
            else
            {
                x.UsingInMemory();
                x.AddRider(rider =>
                {
                    rider.AddConsumer<NotificationConsumer<MessageDto>>();
                    rider.AddConsumer<NotificationConsumer<LinkPreviewDto>>();
                    rider.AddConsumer<NotificationConsumer<ImagePreviewDto>>();
                    rider.UsingKafka((context, k) =>
                    {
                        var kafkaSettings = ConfigurationHelper.GetKafkaSettings(configuration);
                        var topicSettings = ConfigurationHelper.GetTopicSettings(configuration);
                        k.Host(kafkaSettings.Url);
                        k.TopicEndpoint<Notification<MessageDto>>(
                            topicSettings.MessageNotification,
                            kafkaSettings.GroupId,
                            e =>
                            {
                                e.ConfigureConsumer<NotificationConsumer<MessageDto>>(context);
                                e.CreateIfMissing();
                            });
                        k.TopicEndpoint<Notification<LinkPreviewDto>>(
                            topicSettings.LinkPreviewNotification,
                            kafkaSettings.GroupId,
                            e =>
                            {
                                e.ConfigureConsumer<NotificationConsumer<LinkPreviewDto>>(context);
                                e.CreateIfMissing();
                            });
                        k.TopicEndpoint<Notification<ImagePreviewDto>>(
                            topicSettings.ImagePreviewNotification,
                            kafkaSettings.GroupId,
                            e =>
                            {
                                e.ConfigureConsumer<NotificationConsumer<ImagePreviewDto>>(context);
                                e.CreateIfMissing();
                            });
                    });
                });
            }
        });

        return services;
    }
}
