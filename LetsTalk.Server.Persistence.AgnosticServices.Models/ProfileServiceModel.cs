namespace LetsTalk.Server.Persistence.AgnosticServices.Models;

public class ProfileServiceModel
{
    public string? Id { get; set; }

    public int AccountTypeId { get; set; }

    public string? ExternalId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? ImageId { get; set; }

    public int FileStorageTypeId { get; set; }
}
