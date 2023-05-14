using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Constants;
using LetsTalk.Server.Core.Models;
using LetsTalk.Server.Persistence.Models;
using System.Text.RegularExpressions;

namespace LetsTalk.Server.Core.Services;

public class Base64ParsingService: IBase64ParsingService
{
    private readonly Dictionary<string, ImageFileTypes> _imageFileTypeByExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        { "jpeg", ImageFileTypes.Jpeg },
        { "png", ImageFileTypes.Png },
        { "gif", ImageFileTypes.Gif }
    };

    [System.Diagnostics.CodeAnalysis.SuppressMessage("GeneratedRegex", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    public Base64ParsingResult? ParseBase64Image(string? input)
    {
        if (input == null) return null;

        var regex = new Regex(RegexConstants.ImageAsBase64);

        var match = regex.Match(input);

        return new Base64ParsingResult
        {
            ImageFileType = _imageFileTypeByExtension.GetValueOrDefault(match.Groups[1].Value, ImageFileTypes.Unknown),
            Base64string = match.Groups[2].Value
        };
    }
}
