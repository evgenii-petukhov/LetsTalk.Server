using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("imageformats")]
public class ImageFormat(int id, string name) : BaseEntity(id)
{
    public string? Name { get; protected set; } = name;
}
