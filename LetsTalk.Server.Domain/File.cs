using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("files")]
public class File : BaseEntity
{
    public string? FileName { get; protected set; }

    public FileType? FileType { get; protected set; }

    public int FileTypeId { get; protected set; }

    public Image? Image { get; protected set; }

    protected File()
    {
    }

    public File(int id)
    {
        Id = id;
    }

    public File(string fileName, int fileTypeId)
    {
        FileName = fileName;
        FileTypeId = fileTypeId;
    }

    public void SetImage(Image image)
    {
        Image = image;
    }
}
