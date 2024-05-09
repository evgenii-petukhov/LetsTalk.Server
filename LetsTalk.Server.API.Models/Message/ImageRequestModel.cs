using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.API.Models.Message;

public class ImageRequestModel: ISignable
{
    public string? Id { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int ImageFormat { get; set; }

    public string? Signature { get; set; }

    public long SignatureDate { get; set; }
}
