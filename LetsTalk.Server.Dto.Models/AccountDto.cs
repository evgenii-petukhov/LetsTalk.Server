namespace LetsTalk.Server.Dto.Models;

public class AccountDto
{
    public string? Id { get; set; }

    public int AccountTypeId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public int UnreadCount { get; set; }

    public long LastMessageDate { get; set; }

    public int LastMessageId { get; set; }

    public int? ImageId { get; set; }
}
