using LetsTalk.Server.FileStorage.Models;
using System.Text.RegularExpressions;

namespace LetsTalk.Server.Core.Helpers;

public static class Base64Helper
{
    public static bool IsBase64Image(string? input)
    {
        if (input == null) return false;

#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        var regex = new Regex(RegexConstants.ImageAsBase64);
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

        return regex.IsMatch(input);
    }
}
