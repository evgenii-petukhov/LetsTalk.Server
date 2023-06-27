using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("accounts")]
[Index(nameof(ExternalId), nameof(AccountTypeId), IsUnique = true)]
public class Account : BaseEntity
{
    public AccountType? AccountType { get; set; }

    public int AccountTypeId { get; set; }

    [Column(TypeName = "longtext")]
    public string? ExternalId { get; set; }

    public string? Email { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Image? Image { get; set; }

    public int? ImageId { get; set; }
}
