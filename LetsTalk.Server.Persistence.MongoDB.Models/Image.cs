namespace LetsTalk.Server.Persistence.MongoDB.Models;

public class Image : Upload
{
    public int ImageFormatId { get; set; }

    public int ImageRoleId { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }
}
