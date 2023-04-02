using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Configuration;

namespace LetsTalk.Server.Configuration;

public static class ConfigurationHelper
{
    public static KafkaSettings GetKafkaSettings(IConfiguration configuration)
    {
        return new KafkaSettings
        {
            Url = configuration.GetValue<string>("Kafka:Url"),
            MessageNotificationTopic = configuration.GetValue<string>("Kafka:MessageNotificationTopic"),
            MessageNotificationProducer = configuration.GetValue<string>("Kafka:MessageNotificationProducer"),
            MessageNotificationGroupId = configuration.GetValue<string>("Kafka:MessageNotificationGroupId"),
            LinkPreviewTopic = configuration.GetValue<string>("Kafka:LinkPreviewTopic")
        };
    }
}
