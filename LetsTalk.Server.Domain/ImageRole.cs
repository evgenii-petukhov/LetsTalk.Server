using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("imageroles")]
public class ImageRole : BaseEntity
{
    public string? Name { get; set; }
}
