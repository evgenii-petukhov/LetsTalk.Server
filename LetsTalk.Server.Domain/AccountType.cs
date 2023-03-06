using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("accounttypes")]
public class AccountType : BaseEntity
{
    public string? Name { get; set; }
}
