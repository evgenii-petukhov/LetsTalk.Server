using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.ImageProcessing.Abstractions;

public interface IImageInfoService
{
    (int, int) GetImageSize(byte[] data);

    ImageFormats GetImageFormat(byte[] data);
}
