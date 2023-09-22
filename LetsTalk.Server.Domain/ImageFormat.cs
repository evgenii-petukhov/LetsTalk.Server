using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("imageformats")]
public class ImageFormat : BaseEntity
{
    public string? Name { get; protected set; }

    public ImageFormat(int id, string name) : base(id)
    {
        Name = name;
    }
}
