using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("images")]
public class Image : BaseEntity
{
    public ImageFormat? ImageFormat { get; set; }

    public int ImageFormatId { get; set; }

    public ImageRole? ImageRole { get; set; }

    public int ImageRoleId { get; set; }

    public File? File { get; set; }

    public int FileId { get; set; }

    public Account? Account { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }
}
