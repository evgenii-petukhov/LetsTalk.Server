using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("filetypes")]
public class FileType : BaseEntity
{
    public string? Name { get; set; }
}
