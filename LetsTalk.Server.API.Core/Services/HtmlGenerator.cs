using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Models.HtmlGenerator;
using System.Globalization;

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

        var paragraphs = input
            .Split(_separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var sb = new System.Text.StringBuilder();
        var emojisOnly = false;
        var emojiCount = 0;

        if (paragraphs.Length == 1)
        {
            (var wrapped, emojiCount, emojisOnly) = _regexService.WrapEmojisWithSpan(paragraphs[0]);
            sb.Append(CultureInfo.InvariantCulture, $"<p>{wrapped}</p>");
        }
        else
        {
            foreach (var paragraph in paragraphs)
            {
                var (wrapped, count, _) = _regexService.WrapEmojisWithSpan(paragraph);
                sb.Append(CultureInfo.InvariantCulture, $"<p>{wrapped}</p>");
                emojiCount += count;
            }
        }

        var (html, url) = _regexService.ReplaceUrlsByHref(sb.ToString());

        return new HtmlGeneratorResult(
            html,
            url,
            emojisOnly,
            emojiCount);
    }
}
