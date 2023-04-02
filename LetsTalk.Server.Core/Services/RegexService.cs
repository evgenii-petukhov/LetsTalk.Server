using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Models;
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

    public MessageProcessingResult ReplaceUrlsByHref(string input)
    {
        var regex = MatchUrls();
        return new MessageProcessingResult
        {
            Html = regex.Replace(input, "<a href=\"$1\" target=\"_blank\">$1</a>"),
            Url = regex.Matches(input).FirstOrDefault()?.Value
        };
    }
}