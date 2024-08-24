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

    public HtmlGeneratorResult GetHtml(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new HtmlGeneratorResult();
        }

        var lines = text
            .Split(_separators, StringSplitOptions.TrimEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => $"<p>{s}</p>");

        var html = string.Concat(lines);
        return _regexService.ReplaceUrlsByHref(html);
    }
}
