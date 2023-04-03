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
    private static partial Regex MatchTitle();

    [GeneratedRegex(
    @"<meta [^>]*property=[\""']og:image[\""'] [^>]*content=[\""']([^'^\""]+?)[\""'][^>]*>",
    RegexOptions.CultureInvariant,
    matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatchImageUrl();

    public OpenGraphModel GetOpenGraphModel(string input)
    {
        string? title = null;
        var matchTitle = MatchTitle().Match(input);
        if (matchTitle.Groups.Count > 1)
        {
            title = matchTitle.Groups[1].Value;
        }

        string? imageUrl = null;
        var matchImageUrl = MatchImageUrl().Match(input);
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
