using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Models;

public class ImageValidationResult
{
    public ImageFormats ImageFormat { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}
