using System.Text.RegularExpressions;

namespace LetsTalk.Server.Core.Helpers;

public static partial class Base64Helper
{
    [GeneratedRegex(
    "data:image\\/(jpeg|png|gif){1};base64,([^\\\"]*)",
    RegexOptions.CultureInvariant,
    matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchBase64();

    public static bool IsValidBase64(string? input)
    {
        if (input == null) return false;

        return MatchBase64().IsMatch(input);
    }
}
