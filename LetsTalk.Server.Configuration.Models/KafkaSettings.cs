namespace LetsTalk.Server.Configuration.Models;

public class KafkaSettings
{
    public string? Url { get; set; }
    public string? MessageNotificationTopic { get; set; }
    public string? MessageNotificationProducer { get; set; }
    public string? MessageNotificationGroupId { get; set; }
}
