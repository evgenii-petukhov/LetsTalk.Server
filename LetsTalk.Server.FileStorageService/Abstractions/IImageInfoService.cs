using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IImageInfoService
{
    (int, int) GetImageSize(byte[] data);

    ImageFormats GetImageFormat(byte[] data);
}
