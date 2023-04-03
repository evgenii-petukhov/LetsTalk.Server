using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Kafka.Models;

public class MessageNotification
{
    public int RecipientId { get; set; }

    public MessageDto? Message { get; set; }
}