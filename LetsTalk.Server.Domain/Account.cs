using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("accounts")]
[Index(nameof(ExternalId), nameof(AccountTypeId), IsUnique = true)]
public class Account : BaseEntity
{
    public AccountType? AccountType { get; protected set; }

    public int AccountTypeId { get; protected set; }

    [Column(TypeName = "longtext")]
    public string? ExternalId { get; protected set; }

    public string? Email { get; protected set; }

    public string? PhotoUrl { get; protected set; }

    public string? FirstName { get; protected set; }

    public string? LastName { get; protected set; }

    public Image? Image { get; protected set; }

    public string? ImageId { get; protected set; }

    public Account(string externalId, int accountTypeId, string firstName, string lastName, string photoUrl)
    {
        ExternalId = externalId;
        AccountTypeId = accountTypeId;
        FirstName = firstName;
        LastName = lastName;
        PhotoUrl = photoUrl;
    }

    public Account(int accountTypeId, string email)
    {
        AccountTypeId = accountTypeId;
        Email = email;
    }

    protected Account()
    {
    }

    public void SetupProfile(string firstName, string lastName, string photoUrl, bool hasImageId)
    {
        FirstName = firstName;
        LastName = lastName;

        if (!hasImageId)
        {
            PhotoUrl = photoUrl;
        }
    }

    public void UpdateProfile(string firstName, string lastName, Image? image = null)
    {
        FirstName = firstName;
        LastName = lastName;

        if (image != null)
        {
            Image = image;
        }
    }
}
