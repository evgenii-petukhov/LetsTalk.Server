using LetsTalk.Server.ImageProcessor.Models;

namespace LetsTalk.Server.ImageProcessing.Abstractions;

public interface IImageResizeService
{
    ImageResizeResult Resize(byte[] data, int maxWidth, int maxHeight);
}
