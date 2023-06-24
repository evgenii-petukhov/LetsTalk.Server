using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Services;

public class Base64ParsingService: IBase64ParsingService
{
    private readonly Dictionary<ImageContentTypes, string> _nameByContentType = new()
    {
        { ImageContentTypes.Jpeg, "jpeg" },
        { ImageContentTypes.Png, "png" },
        { ImageContentTypes.Gif, "gif" }
    };

    public string GetContentTypeName(ImageContentTypes contentType)
    {
        return _nameByContentType[contentType];
    }
}
