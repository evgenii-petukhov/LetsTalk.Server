using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("imageformats")]
public class ImageFormat : BaseEntity
{
    public string? Name { get; set; }
}
