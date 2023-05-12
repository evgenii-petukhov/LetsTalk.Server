using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Enums;

namespace LetsTalk.Server.Core.Services;

public class ImageTypeService : IImageTypeService
{
    private readonly Dictionary<ImageTypes, string> _extensionByImageType = new()
    {
        { ImageTypes.Jpeg, ".jpg" },
        { ImageTypes.Png, ".png" },
        { ImageTypes.Gif, ".gif" },
        { ImageTypes.Unknown, string.Empty }
    };

    public string GetExtensionByImageType(ImageTypes imageType)
    {
        return _extensionByImageType[imageType];
    }
}
