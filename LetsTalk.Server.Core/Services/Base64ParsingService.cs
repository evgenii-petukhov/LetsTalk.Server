using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Constants;
using LetsTalk.Server.Core.Models;
using LetsTalk.Server.Persistence.Models;
using System.Text.RegularExpressions;

namespace LetsTalk.Server.Core.Services;

public class Base64ParsingService: IBase64ParsingService
{
    private readonly Dictionary<string, ImageContentTypes> _contentTypeByExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        { "jpeg", ImageContentTypes.Jpeg },
        { "png", ImageContentTypes.Png },
        { "gif", ImageContentTypes.Gif }
    };

    private readonly Dictionary<ImageContentTypes, string> _nameByContentType = new()
    {
        { ImageContentTypes.Jpeg, "jpeg" },
        { ImageContentTypes.Png, "png" },
        { ImageContentTypes.Gif, "gif" }
    };

    [System.Diagnostics.CodeAnalysis.SuppressMessage("GeneratedRegex", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    public Base64ParsingResult? ParseBase64String(string? input)
    {
        if (input == null) return null;

        var regex = new Regex(RegexConstants.ImageAsBase64);

        var match = regex.Match(input);

        return new Base64ParsingResult
        {
            ImageContentType = _contentTypeByExtension.GetValueOrDefault(match.Groups[1].Value, ImageContentTypes.Unknown),
            Base64string = match.Groups[2].Value
        };
    }

    public string CreateBase64String(byte[] content, ImageContentTypes contentType)
    {
        var contentTypeName = _nameByContentType.GetValueOrDefault(contentType, "jpeg");
        var base64String = Convert.ToBase64String(content);
        return $"data:image/{contentTypeName};base64,{base64String}";
    }
}
