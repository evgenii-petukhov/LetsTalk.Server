namespace LetsTalk.Server.Domain;

public class AccountWithUnreadCount : BaseEntity
{
    public int AccountTypeId { get; protected set; }

    public string? PhotoUrl { get; protected set; }

    public string? FirstName { get; protected set; }

    public string? LastName { get; protected set; }

    public int? UnreadCount { get; protected set; }

    public long? LastMessageDate { get; protected set; }

    public int? ImageId { get; protected set; }

    public AccountWithUnreadCount(int id, string? firstName, string? lastName, string? photoUrl, int accountTypeId, long? lastMessageDate, int unreadCount, int? imageId)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        PhotoUrl = photoUrl;
        AccountTypeId = accountTypeId;
        LastMessageDate = lastMessageDate;
        UnreadCount = unreadCount;
        ImageId = imageId;
    }
}
