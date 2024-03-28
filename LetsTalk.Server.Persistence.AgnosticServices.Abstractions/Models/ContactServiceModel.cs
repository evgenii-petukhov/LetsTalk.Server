namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

public class ContactServiceModel
{
    public string? Id { get; set; }

    public int AccountTypeId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ChatName { get; set; }

    public int? UnreadCount { get; set; }

    public long? LastMessageDate { get; set; }

    public string? LastMessageId { get; set; }

    public string? ImageId { get; set; }
}
