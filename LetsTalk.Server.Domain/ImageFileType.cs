using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("imagefiletypes")]
public class ImageFileType: BaseEntity
{
    public string? Name { get; set; }
}
