using LetsTalk.Server.FileStorage.Models;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IImageInfoService
{
    ImageSize GetImageSize(byte[] data);

    ImageContentTypes GetImageContentType(byte[] data);
}
