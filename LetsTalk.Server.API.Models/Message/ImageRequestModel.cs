using LetsTalk.Server.SignPackage.Models;

namespace LetsTalk.Server.API.Models.Message;

public class ImageRequestModel: ISignable
{
    public string? Id { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int ImageFormat { get; set; }

    public int FileStorageTypeId { get; set; }

    public string? Signature { get; set; }
}
