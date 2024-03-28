namespace LetsTalk.Server.Domain;

public class ChatListItem
{
    public int Id { get; set; }

    public int? AccountTypeId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? ChatName { get; set; }

    public int? UnreadCount { get; set; }

    public long? LastMessageDate { get; set; }

    public int? LastMessageId { get; set; }

    public string? ImageId { get; set; }
}
