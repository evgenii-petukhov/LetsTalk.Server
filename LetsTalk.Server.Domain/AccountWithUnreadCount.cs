namespace LetsTalk.Server.Domain;

public class AccountWithUnreadCount : BaseEntity
{
    public int AccountTypeId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? UnreadCount { get; set; }
}
