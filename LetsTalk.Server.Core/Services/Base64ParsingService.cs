using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.FileStorage.Services;

public class Base64ParsingService: IBase64ParsingService
{
    private readonly Dictionary<ImageContentTypes, string> _nameByContentType = new()
    {
        { ImageContentTypes.Jpeg, "jpeg" },
        { ImageContentTypes.Png, "png" },
        { ImageContentTypes.Gif, "gif" }
    };

    public string CreateBase64String(byte[] content, ImageContentTypes contentType)
    {
        var contentTypeName = _nameByContentType.GetValueOrDefault(contentType, "jpeg");
        var base64String = Convert.ToBase64String(content);
        return $"data:image/{contentTypeName};base64,{base64String}";
    }
}
