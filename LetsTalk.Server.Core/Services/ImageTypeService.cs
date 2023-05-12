using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Constants;
using LetsTalk.Server.Core.Enums;
using LetsTalk.Server.Core.Models;
using System.Text.RegularExpressions;

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

    private readonly Dictionary<string, ImageTypes> _imagetypeByExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        { "jpeg", ImageTypes.Jpeg },
        { "png", ImageTypes.Png },
        { "gif", ImageTypes.Gif }
    };

    public string GetExtensionByImageType(ImageTypes imageType)
    {
        return _extensionByImageType[imageType];
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("GeneratedRegex", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    public Base64ParsingResult? ParseBase64Image(string? input)
    {
        if (input == null) return null;

        var regex = new Regex(RegexConstants.ImageAsBase64);

        var match = regex.Match(input);

        return new Base64ParsingResult
        {
            Imagetype = _imagetypeByExtension.GetValueOrDefault(match.Groups[1].Value, ImageTypes.Unknown),
            Base64string = match.Groups[2].Value
        };
    }
}
