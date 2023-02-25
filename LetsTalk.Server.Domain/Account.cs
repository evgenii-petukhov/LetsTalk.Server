namespace LetsTalk.Server.Domain;

public class Account : BaseEntity
{
    public AccountType? AccountType { get; set; }

    public int AccountTypeId { get; set; }

    public string? ExternalId { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? AuthToken { get; set; }
}
