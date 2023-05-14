using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("imagetypes")]
public class ImageType: BaseEntity
{
    public string? Name { get; set; }
}
