using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain
{
    [Table("filestoragetypes")]
    public class FileStorageType(int id, string name) : BaseEntity(id)
    {
        public string? Name { get; protected set; } = name;
    }

}
