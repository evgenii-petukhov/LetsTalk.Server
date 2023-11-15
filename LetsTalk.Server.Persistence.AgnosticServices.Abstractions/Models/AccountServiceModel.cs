namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

public class AccountServiceModel
{
    public string? Id { get; set; }

    public int AccountTypeId { get; protected set; }

    public string? ExternalId { get; protected set; }

    public string? Email { get; protected set; }

    public string? PhotoUrl { get; protected set; }

    public string? FirstName { get; protected set; }

    public string? LastName { get; protected set; }

    public int? ImageId { get; protected set; }
}
