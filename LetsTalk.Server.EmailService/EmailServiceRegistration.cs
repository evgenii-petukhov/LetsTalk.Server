using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.EmailService.Abstractions;
using LetsTalk.Server.EmailService.Services;
using LetsTalk.Server.Kafka.Models;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mime;

namespace LetsTalk.Server.EmailService;

public static class EmailServiceRegistration
{
    public static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            if (configuration.GetValue<string>("Features:EventBrokerMode") == "aws")
            {
                x.AddConsumer<SendLoginCodeRequestConsumer>();

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
                    configure.ReceiveEndpoint(queueSettings.SendLoginCodeRequest!, e =>
                    {
                        e.WaitTimeSeconds = 20;
                        e.DefaultContentType = new ContentType("application/json");
                        e.UseRawJsonDeserializer();
                        e.ConfigureConsumeTopology = false;
                        e.ConfigureConsumer<SendLoginCodeRequestConsumer>(context);
                    });
                });
            }
            else
            {
                x.UsingInMemory();
                x.AddRider(rider =>
                {
                    rider.AddConsumer<SendLoginCodeRequestConsumer>();
                    rider.UsingKafka((context, k) =>
                    {
                        var kafkaSettings = ConfigurationHelper.GetKafkaSettings(configuration);
                        var topicSettings = ConfigurationHelper.GetTopicSettings(configuration);
                        k.Host(kafkaSettings.Url);
                        k.TopicEndpoint<SendLoginCodeRequest>(
                            topicSettings.SendLoginCodeRequest!,
                            kafkaSettings.GroupId,
                            e =>
                            {
                                e.ConfigureConsumer<SendLoginCodeRequestConsumer>(context);
                                e.CreateIfMissing();
                            });
                    });
                });
            }
        });

        services.Configure<EmailServiceSettings>(configuration.GetSection("EmailService"));
        services.AddScoped<IEmailService, DefaultEmailService>();

        return services;
    }
}
