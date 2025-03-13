using LetsTalk.Server.ImageProcessing.ImageResizeEngine.Abstractions.Models;

namespace LetsTalk.Server.ImageProcessing.ImageResizeEngine.Abstractions;

public interface IImageResizeService
{
    ImageResizeResult Resize(byte[] data, int maxWidth, int maxHeight);
}
