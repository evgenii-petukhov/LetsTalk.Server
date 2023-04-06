using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.LinkPreview.Models;
using System.Text.RegularExpressions;

namespace LetsTalk.Server.LinkPreview.Services;

public partial class RegexService : IRegexService
{
    [GeneratedRegex(
        @"<meta [^>]*property=[\""']og:title[\""'] [^>]*content=[\""']([^'^\""]+?)[\""'][^>]*>",
        RegexOptions.CultureInvariant,
        matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchOpenGraphTitle();

    [GeneratedRegex(
        @"<meta [^>]*property=[\""']og:image[\""'] [^>]*content=[\""']([^'^\""]+?)[\""'][^>]*>",
        RegexOptions.CultureInvariant,
        matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchOpenGraphImageUrl();

    [GeneratedRegex(
        @"<title.*?>(.*?)</title>",
        RegexOptions.CultureInvariant,
        matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchTitle();

    public OpenGraphModel GetOpenGraphModel(string input)
    {
        string? title = null;
        var matchOpenGraphTitle = MatchOpenGraphTitle().Match(input);
        if (matchOpenGraphTitle.Groups.Count > 1)
        {
            title = matchOpenGraphTitle.Groups[1].Value;
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            var matchTitle = MatchTitle().Match(input);
            if (matchTitle.Groups.Count > 1)
            {
                title = matchTitle.Groups[1].Value;
            }
        }

        string? imageUrl = null;
        var matchImageUrl = MatchOpenGraphImageUrl().Match(input);
        if (matchImageUrl.Groups.Count > 1)
        {
            imageUrl = matchImageUrl.Groups[1].Value;
        }

        return new OpenGraphModel
        {
            Title = title,
            ImageUrl = imageUrl
        };
    }
}
