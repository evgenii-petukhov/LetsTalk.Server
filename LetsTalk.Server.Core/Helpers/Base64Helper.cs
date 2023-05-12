using LetsTalk.Server.Core.Constants;
using System.Text.RegularExpressions;

namespace LetsTalk.Server.Core.Helpers;

public static class Base64Helper
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("GeneratedRegex", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    public static bool IsBase64Image(string? input)
    {
        if (input == null) return false;

        var regex = new Regex(RegexConstants.ImageAsBase64);

        return regex.IsMatch(input);
    }
}
