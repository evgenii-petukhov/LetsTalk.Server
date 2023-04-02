using LetsTalk.Server.Core.Abstractions;
using System.Text.RegularExpressions;

namespace LetsTalk.Server.Core.Services;

public partial class RegexService : IRegexService
{
    // https://stevetalkscode.co.uk/regex-source-generator
    [GeneratedRegex(
    @"((http|ftp|https):\/\/([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:\/~+#-]*[\w@?^=%&\/~+#-]))",
    RegexOptions.CultureInvariant,
    matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchUrls();

    public string ReplaceUrlsByHref(string input)
    {
        return MatchUrls().Replace(input, "<a href=\"$1\" target=\"_blank\">$1</a>");
    }
}

