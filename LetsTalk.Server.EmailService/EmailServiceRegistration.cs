using KafkaFlow;
using KafkaFlow.Serializer;
using LetsTalk.Server.Configuration;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.EmailService.Abstractions;
using LetsTalk.Server.EmailService.Services;
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
        services.AddKafka(kafka => kafka
            .UseConsoleLog()
            .AddCluster(cluster => cluster
                .WithBrokers(new[]
                {
                    kafkaSettings.Url
                })
                .CreateTopicIfNotExists(kafkaSettings.SendLoginCodeRequest!.Topic, 1, 1)
                .AddConsumer(consumer => consumer
                    .Topic(kafkaSettings.SendLoginCodeRequest.Topic)
                    .WithGroupId(kafkaSettings.SendLoginCodeRequest.GroupId)
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<JsonCoreDeserializer>()
                        .AddTypedHandlers(h => h.AddHandler<SendLoginCodeRequestHandler>().WithHandlerLifetime(InstanceLifetime.Transient))))));

        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.Configure<EmailServiceSettings>(configuration.GetSection("EmailService"));
        services.AddScoped<IEmailService, DefaultEmailService>();

        return services;
    }
}
