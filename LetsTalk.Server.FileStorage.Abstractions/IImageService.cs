using LetsTalk.Server.FileStorage.Models;

namespace LetsTalk.Server.FileStorage.Abstractions;

public interface IImageService
{
    ImageInfo GetImageInfo(byte[] data);
}
