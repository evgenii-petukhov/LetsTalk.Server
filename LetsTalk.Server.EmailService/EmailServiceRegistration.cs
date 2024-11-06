using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.EmailService.Abstractions;
using LetsTalk.Server.EmailService.Services;
using LetsTalk.Server.Kafka.Models;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Server.EmailService;

public static class EmailServiceRegistration
{
    public static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var kafkaSettings = KafkaSettingsHelper.GetKafkaSettings(configuration);
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<SendLoginCodeRequestConsumer>();

                rider.UsingKafka((context, k) =>
                {
                    k.Host(kafkaSettings.Url);

                    k.TopicEndpoint<SendLoginCodeRequest>(
                        kafkaSettings.SendLoginCodeRequest!.Topic,
                        kafkaSettings.SendLoginCodeRequest.GroupId,
                        e =>
                        {
                            e.ConfigureConsumer<SendLoginCodeRequestConsumer>(context);
                            e.CreateIfMissing();
                        });
                });
            });
        });

        services.Configure<EmailServiceSettings>(configuration.GetSection("EmailService"));
        services.AddScoped<IEmailService, DefaultEmailService>();

        return services;
    }
}
