using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.Core.Services;

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
