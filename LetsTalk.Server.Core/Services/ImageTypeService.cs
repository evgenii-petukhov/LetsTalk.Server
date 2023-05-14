using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.Core.Services;

public class ImageTypeService : IImageTypeService
{
    private readonly Dictionary<ImageFileTypes, string> _extensionByImageFileType = new()
    {
        { ImageFileTypes.Jpeg, ".jpg" },
        { ImageFileTypes.Png, ".png" },
        { ImageFileTypes.Gif, ".gif" },
        { ImageFileTypes.Unknown, string.Empty }
    };
    
    public string GetExtensionByImageType(ImageFileTypes imageFileType)
    {
        return _extensionByImageFileType[imageFileType];
    }
}
