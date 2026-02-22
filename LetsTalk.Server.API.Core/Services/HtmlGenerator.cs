using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Models.HtmlGenerator;

namespace LetsTalk.Server.API.Core.Services;

public class HtmlGenerator(IRegexService regexService) : IHtmlGenerator
{
    private static readonly string[] _separators =
    [
        "\r\n",
        "\n"
    ];

    private readonly IRegexService _regexService = regexService;

    public HtmlGeneratorResult GetHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new HtmlGeneratorResult();
        }

        var paragraphInfos = input
            .Split(_separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s =>
            {
                var (wrapped, count, emojisOnly) = _regexService.WrapEmojisWithSpan(s);

                return new
                {
                    Html = $"<p>{wrapped}</p>",
                    EmojisOnly = emojisOnly,
                    EmojiCount = count,
                };
            })
            .ToList();

        var lines = string.Concat(paragraphInfos.Select(p => p.Html));
        var (html, url) = _regexService.ReplaceUrlsByHref(lines);

        return new HtmlGeneratorResult(
            html,
            url,
            paragraphInfos.Count == 1 && paragraphInfos[0].EmojisOnly,
            paragraphInfos.Sum(p => p.EmojiCount));
    }
}
