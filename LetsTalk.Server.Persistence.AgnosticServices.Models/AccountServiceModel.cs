namespace LetsTalk.Server.Persistence.AgnosticServices.Models;

public class AccountServiceModel
{
    public string? Id { get; set; }

    public int? AccountTypeId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ImageId { get; set; }
}
