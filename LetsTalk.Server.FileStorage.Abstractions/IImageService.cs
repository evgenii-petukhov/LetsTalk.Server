using LetsTalk.Server.FileStorage.Models;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.FileStorage.Abstractions;

public interface IImageService
{
    ImageSize GetImageSize(byte[] data);

    ImageContentTypes GetImageContentType(byte[] data);
}
