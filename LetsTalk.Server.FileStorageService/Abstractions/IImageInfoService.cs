using LetsTalk.Server.FileStorageService.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IImageInfoService
{
    ImageSize GetImageSize(byte[] data);

    ImageContentTypes GetImageContentType(byte[] data);
}
