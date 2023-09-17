using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Models.HtmlGenerator;
using System.Text.RegularExpressions;

namespace LetsTalk.Server.Core.Services;

public partial class RegexService : IRegexService
{
    private const string LINK_REPLACEMENT = "<a href=\"$1\" target=\"_blank\">$1</a>";

    // https://stevetalkscode.co.uk/regex-source-generator
    [GeneratedRegex(
    @"((http|ftp|https):\/\/([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:\/~+#-]*[\w@?^=%&\/~+#-]))",
    RegexOptions.CultureInvariant,
    matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchUrls();

    public HtmlGeneratorResult ReplaceUrlsByHref(string input)
    {
        var regex = MatchUrls();
        return new HtmlGeneratorResult
        {
            Html = regex.Replace(input, LINK_REPLACEMENT),
            Url = regex.Matches(input).FirstOrDefault()?.Value
        };
    }
}