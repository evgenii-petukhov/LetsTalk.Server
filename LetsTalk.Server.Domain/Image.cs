using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("images")]
public class Image : BaseEntity
{
    public ImageContentType? ImageContentType { get; set; }

    public int ImageContentTypeId { get; set; }

    public ImageType? ImageType { get; set; }

    public int ImageTypeId { get; set; }

    public File? File { get; set; }

    public int FileId { get; set; }

    public Account? Account { get; set; }
}
