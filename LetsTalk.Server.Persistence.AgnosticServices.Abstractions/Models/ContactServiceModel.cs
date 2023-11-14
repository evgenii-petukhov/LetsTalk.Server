namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

public class ContactServiceModel
{
    public int Id { get; set; }

    public int AccountTypeId { get; protected set; }

    public string? PhotoUrl { get; protected set; }

    public string? FirstName { get; protected set; }

    public string? LastName { get; protected set; }

    public int? UnreadCount { get; set; }

    public long? LastMessageDate { get; set; }

    public int? LastMessageId { get; set; }

    public int? ImageId { get; protected set; }
}
