using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("filetypes")]
public class FileType : BaseEntity
{
    public string? Name { get; protected set; }

    public FileType(int id, string name): base(id)
    {
        Name = name;
    }
}
