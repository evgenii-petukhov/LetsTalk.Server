namespace LetsTalk.Server.Configuration.Models;

public class KafkaSettings
{
    public string? Url { get; set; }
    public string? MessageNotificationTopic { get; set; }
    public string? MessageNotificationProducer { get; set; }
    public string? MessageNotificationGroupId { get; set; }
    public string? LinkPreviewTopic { get; set; }
    public string? LinkPreviewGroupId { get; set; }
    public string? LinkPreviewProducer { get; set; }
}
