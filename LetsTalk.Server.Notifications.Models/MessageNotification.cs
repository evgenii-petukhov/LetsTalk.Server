using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Notifications.Models;

public class MessageNotification
{
    public int RecipientId { get; set; }

    public MessageDto? Message { get; set; }
}
