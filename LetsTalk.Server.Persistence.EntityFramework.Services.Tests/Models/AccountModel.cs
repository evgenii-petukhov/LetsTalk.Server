using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Tests.Models;

public class AccountModel
{
    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ImageId { get; set; }

    public FileStorageTypes? FileStorageType { get; set; }
}
