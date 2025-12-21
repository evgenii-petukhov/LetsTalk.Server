using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Kafka.Models;

public class Notification
{
    public string? RecipientId { get; set; }

    public MessageDto? Message { get; set; }

    public LinkPreviewDto? LinkPreview { get; set; }

    public ImagePreviewDto? ImagePreview { get; set; }

    public RtcSessionSettings? Connection { get; set; }
}
