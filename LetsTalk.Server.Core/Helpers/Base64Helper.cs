using LetsTalk.Server.Core.Enums;
using LetsTalk.Server.Core.Models;
using System.Text.RegularExpressions;

namespace LetsTalk.Server.Core.Helpers;

public static partial class Base64Helper
{
    [GeneratedRegex(
    "data:image\\/(jpeg|png|gif){1};base64,([^\\\"]*)",
    RegexOptions.CultureInvariant,
    matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchBase64();

    private static readonly Dictionary<string, UploadImageTypes> _imageTypeMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "jpeg", UploadImageTypes.Jpeg },
        { "png", UploadImageTypes.Png },
        { "gif", UploadImageTypes.Gif }
    };

    public static Base64ParsingResult? ParseBase64Image(string? input)
    {
        if (input == null) return null;

        var match = MatchBase64().Match(input);

        return new Base64ParsingResult
        {
            Imagetype = _imageTypeMappings.GetValueOrDefault(match.Groups[1].Value, UploadImageTypes.Unknown),
            Base64string = match.Groups[2].Value
        };
    }

    public static bool IsBase64Image(string? input)
    {
        if (input == null) return false;

        return MatchBase64().IsMatch(input);
    }
}
