using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("images")]
public class Image : BaseEntity
{
    public ImageFormat? ImageFormat { get; protected set; }

    public int ImageFormatId { get; protected set; }

    public ImageRole? ImageRole { get; protected set; }

    public int ImageRoleId { get; protected set; }

    public File? File { get; protected set; }

    public int FileId { get; protected set; }

    public Account? Account { get; protected set; }

    public int? Width { get; protected set; }

    public int? Height { get; protected set; }

    protected Image()
    {

    }

    public Image(int imageFormatId, int imageRoleId, int width, int height)
    {
        ImageFormatId = imageFormatId;
        ImageRoleId = imageRoleId;
        Width = width;
        Height = height;
    }
}
