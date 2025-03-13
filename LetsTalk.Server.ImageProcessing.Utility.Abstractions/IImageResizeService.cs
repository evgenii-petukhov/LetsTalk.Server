using LetsTalk.Server.ImageProcessing.Utility.Abstractions.Models;

namespace LetsTalk.Server.ImageProcessing.Utility.Abstractions;

public interface IImageResizeService
{
    ImageResizeResult Resize(byte[] data, int maxWidth, int maxHeight);
}
