using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("files")]
public class File : BaseEntity
{
    public string? FileName { get; set; }

    public FileType? FileType { get; set; }

    public int FileTypeId { get; set; }
}
