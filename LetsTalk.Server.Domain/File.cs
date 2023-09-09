using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("files")]
public class File : BaseEntity
{
    public string? FileName { get; protected set; }

    public FileType? FileType { get; protected set; }
    public int FileTypeId { get; protected set; }

    public ICollection<Image> Images { get; } = new List<Image>();

    protected File()
    {

    }

    public File(string fileName, int fileTypeId)
    {
        FileName = fileName;
        FileTypeId = fileTypeId;
    }

    public void AddImage(Image image)
    {
        Images.Add(image);
    }
}
