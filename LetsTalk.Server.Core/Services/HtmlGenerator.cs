using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Models.HtmlGenerator;

namespace LetsTalk.Server.Core.Services;

public class HtmlGenerator : IHtmlGenerator
{
    private static readonly string[] _separators = new string[]
    {
        "\r\n",
        "\n"
    };

    private readonly IRegexService _regexService;

    public HtmlGenerator(IRegexService regexService)
    {
        _regexService = regexService;
    }

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
