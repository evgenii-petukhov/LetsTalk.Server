using LetsTalk.Server.AuthenticationClient;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Logging;
using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Handlers;
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
            if (configuration.GetValue<string>("Features:EventBrokerMode") == "aws")
            {
                x.AddConsumer<NotificationConsumer>();

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
                    configure.ReceiveEndpoint(queueSettings.Notification!, e =>
                    {
                        e.WaitTimeSeconds = 20;
                        e.DefaultContentType = new ContentType("application/json");
                        e.UseRawJsonDeserializer();
                        e.ConfigureConsumeTopology = false;
                        e.ConfigureConsumer<NotificationConsumer>(context);
                    });
                });
            }
            else
            {
                x.UsingInMemory();
                x.AddRider(rider =>
                {
                    rider.AddConsumer<NotificationConsumer>();
                    rider.UsingKafka((context, k) =>
                    {
                        var kafkaSettings = ConfigurationHelper.GetKafkaSettings(configuration);
                        var topicSettings = ConfigurationHelper.GetTopicSettings(configuration);
                        k.Host(kafkaSettings.Url);
                        k.TopicEndpoint<Notification>(
                            topicSettings.Notification,
                            kafkaSettings.GroupId,
                            e =>
                            {
                                e.ConfigureConsumer<NotificationConsumer>(context);
                                e.CreateIfMissing();
                            });
                    });
                });
            }
        });

        return services;
    }
}
