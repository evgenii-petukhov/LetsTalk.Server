using LetsTalk.Server.Domain.Events;
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

    public int? ImageId { get; protected set; }

    public Account(string externalId, int accountTypeId, string firstName, string lastName, string photoUrl, string email)
    {
        ExternalId = externalId;
        AccountTypeId = accountTypeId;
        FirstName = firstName;
        LastName = lastName;
        PhotoUrl = photoUrl;
        Email = email;
    }

    public Account(int id): base(id)
    {
    }

    protected Account()
    {
    }

    public void SetupProfile(string firstName, string lastName, string email, string photoUrl, bool hasImageId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;

        if (!hasImageId)
        {
            PhotoUrl = photoUrl;
        }
    }

    public void UpdateProfile(string firstName, string lastName, string email, int? imageId)
    {
        var avatarChangedDomainEvent = ImageId.HasValue ? new AvatarChangedDomainEvent
        {
            PreviousImageId = ImageId.Value
        } : null;

        FirstName = firstName;
        LastName = lastName;
        Email = email;

        if (imageId.HasValue)
        {
            ImageId = imageId.Value;
        }

        if (avatarChangedDomainEvent != null)
        {
            AddDomainEvent(avatarChangedDomainEvent);
        }
    }
}
