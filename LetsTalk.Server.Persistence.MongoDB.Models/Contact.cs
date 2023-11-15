namespace LetsTalk.Server.Persistence.MongoDB.Models;

public class Contact
{
    public string? Id { get; set; }

    public int AccountTypeId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? UnreadCount { get; set; }

    public long? LastMessageDate { get; set; }

    public int? LastMessageId { get; set; }

    public int? ImageId { get; set; }
}
