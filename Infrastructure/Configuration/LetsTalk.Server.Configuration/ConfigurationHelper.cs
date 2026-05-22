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
            Notification = configuration.GetValue<string>("Topics:Notification"),
            LinkPreviewRequest = configuration.GetValue<string>("Topics:LinkPreviewRequest"),
            ImageResizeRequest = configuration.GetValue<string>("Topics:ImageResizeRequest"),
            RemoveImageRequest = configuration.GetValue<string>("Topics:RemoveImageRequest"),
            SendEmailRequest = configuration.GetValue<string>("Topics:SendEmailRequest"),
        };
    }

    public static QueueSettings GetQueueSettings(IConfiguration configuration)
    {
        return new QueueSettings
        {
            Notification = configuration.GetValue<string>("Queues:Notification"),
            LinkPreviewRequest = configuration.GetValue<string>("Queues:LinkPreviewRequest"),
            ImageResizeRequest = configuration.GetValue<string>("Queues:ImageResizeRequest"),
            RemoveImageRequest = configuration.GetValue<string>("Queues:RemoveImageRequest"),
            SendEmailRequest = configuration.GetValue<string>("Queues:SendEmailRequest"),
        };
    }
}
