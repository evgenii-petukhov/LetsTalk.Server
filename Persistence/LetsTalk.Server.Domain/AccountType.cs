using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("accounttypes")]
public class AccountType(int id, string name) : BaseEntity(id)
{
    public string? Name { get; protected set; } = name;
}
