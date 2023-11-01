using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Configuration;

namespace LetsTalk.Server.Configuration;

public static class KafkaSettingsHelper
{
    public static KafkaSettings GetKafkaSettings(IConfiguration configuration)
    {
        return new KafkaSettings
        {
            Url = configuration.GetValue<string>("Kafka:Url"),
            MessageNotification = GetKafkaEventSettings(configuration, "Kafka:MessageNotification"),
            LinkPreviewRequest = GetKafkaEventSettings(configuration, "Kafka:LinkPreviewRequest"),
            LinkPreviewNotification = GetKafkaEventSettings(configuration, "Kafka:LinkPreviewNotification"),
            ImageResizeRequest = GetKafkaEventSettings(configuration, "Kafka:ImageResizeRequest"),
            ImagePreviewNotification = GetKafkaEventSettings(configuration, "Kafka:ImagePreviewNotification"),
            RemoveImageRequest = GetKafkaEventSettings(configuration, "Kafka:RemoveImageRequest"),
        };
    }

    private static KafkaEventSettings GetKafkaEventSettings(IConfiguration configuration, string sectionPath)
    {
        return new KafkaEventSettings
        {
            Topic = configuration.GetValue<string>(sectionPath + ":Topic"),
            Producer = configuration.GetValue<string>(sectionPath + ":Producer"),
            GroupId = configuration.GetValue<string>(sectionPath + ":GroupId")
        };
    }
}
