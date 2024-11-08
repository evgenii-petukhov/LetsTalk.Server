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
            GroupId = configuration.GetValue<string>("Kafka:GroupId"),
        };
    }

    public static AwsSettings GetAwsSettings(IConfiguration configuration)
    {
        return new AwsSettings
        {
            AccessKey = configuration.GetValue<string>("Aws:AccessKey"),
            SecretKey = configuration.GetValue<string>("Aws:SecretKey"),
            Region = configuration.GetValue<string>("Aws:Region")
        };
    }

    public static TopicSettings GetTopicSettings(IConfiguration configuration)
    {
        return new TopicSettings
        {
            MessageNotification = configuration.GetValue<string>("Topics:MessageNotification"),
            LinkPreviewRequest = configuration.GetValue<string>("Topics:LinkPreviewRequest"),
            LinkPreviewNotification = configuration.GetValue<string>("Topics:LinkPreviewNotification"),
            ImageResizeRequest = configuration.GetValue<string>("Topics:ImageResizeRequest"),
            ImagePreviewNotification = configuration.GetValue<string>("Topics:ImagePreviewNotification"),
            RemoveImageRequest = configuration.GetValue<string>("Topics:RemoveImageRequest"),
            SendLoginCodeRequest = configuration.GetValue<string>("Topics:SendLoginCodeRequest"),
        };
    }

    public static QueueSettings GetQueueSettings(IConfiguration configuration)
    {
        return new QueueSettings
        {
            MessageNotification = configuration.GetValue<string>("Queues:MessageNotification"),
            LinkPreviewRequest = configuration.GetValue<string>("Queues:LinkPreviewRequest"),
            LinkPreviewNotification = configuration.GetValue<string>("Queues:LinkPreviewNotification"),
            ImageResizeRequest = configuration.GetValue<string>("Queues:ImageResizeRequest"),
            ImagePreviewNotification = configuration.GetValue<string>("Queues:ImagePreviewNotification"),
            RemoveImageRequest = configuration.GetValue<string>("Queues:RemoveImageRequest"),
            SendLoginCodeRequest = configuration.GetValue<string>("Queues:SendLoginCodeRequest"),
        };
    }
}
