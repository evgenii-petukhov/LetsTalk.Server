using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("imagecontenttypes")]
public class ImageContentType : BaseEntity
{
    public string? Name { get; set; }
}
