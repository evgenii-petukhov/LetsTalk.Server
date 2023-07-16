using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Service.Models;

public class ImageValidationResult
{
    public ImageFormats ImageFormat { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}
