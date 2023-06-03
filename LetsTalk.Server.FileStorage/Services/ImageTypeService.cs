using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.FileStorage.Services;

public class ImageTypeService : IImageTypeService
{
    private readonly Dictionary<ImageContentTypes, string> _extensionByContentType = new()
    {
        { ImageContentTypes.Jpeg, ".jpg" },
        { ImageContentTypes.Png, ".png" },
        { ImageContentTypes.Gif, ".gif" },
        { ImageContentTypes.Unknown, string.Empty }
    };

    public string GetExtensionByImageType(ImageContentTypes contentType)
    {
        return _extensionByContentType[contentType];
    }
}