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

    public void SetFirstName(string firstName)
    {
        FirstName = firstName;
    }

    public void SetLastName(string lastName)
    {
        LastName = lastName;
    }

    public void SetPhotoUrl(string photoUrl)
    {
        PhotoUrl = photoUrl;
    }

    public void SetEmail(string email)
    {
        Email = email;
    }

    public void SetImageId(int imageId)
    {
        ImageId = imageId;
    }
}
