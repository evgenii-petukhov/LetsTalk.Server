using LetsTalk.Server.Models.Dtos;

namespace LetsTalk.Server.Models.Kafka;

public class Notification
{
    public string? RecipientId { get; set; }

    public MessageDto? Message { get; set; }

    public LinkPreviewDto? LinkPreview { get; set; }

    public ImagePreviewDto? ImagePreview { get; set; }

    public IncomingCallRequest? IncomingCall { get; set; }

    public EstablishConnectionRequest? EstablishConnection { get; set; }
}
