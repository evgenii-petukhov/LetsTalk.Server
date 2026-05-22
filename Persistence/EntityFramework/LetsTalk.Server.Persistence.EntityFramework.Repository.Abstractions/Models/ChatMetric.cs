namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions.Models;

public class ChatMetric
{
    public int ChatId { get; set; }

    public long? LastMessageDate { get; set; }

    public int LastMessageId { get; set; }

    public int UnreadCount { get; set; }
}
