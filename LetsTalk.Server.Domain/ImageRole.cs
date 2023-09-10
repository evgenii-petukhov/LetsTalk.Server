using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("imageroles")]
public class ImageRole : BaseEntity
{
    public string? Name { get; protected set; }

    public ImageRole(int id, string name) : base(id)
    {
        Name = name;
    }
}
