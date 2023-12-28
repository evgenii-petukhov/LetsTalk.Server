using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("images")]
public class Image
{
    [MaxLength(36)]
    public string? Id { get; protected set; }

    public ImageFormat? ImageFormat { get; protected set; }

    public int ImageFormatId { get; protected set; }

    public Account? Account { get; protected set; }

    public int? Width { get; protected set; }

    public int? Height { get; protected set; }

    protected Image()
    {
    }

    public Image(string id, int imageFormatId, int width, int height)
    {
        Id = id;
        ImageFormatId = imageFormatId;
        Width = width;
        Height = height;
    }
}
