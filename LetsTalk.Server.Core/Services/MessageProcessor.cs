using LetsTalk.Server.Core.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class MessageProcessor : IMessageProcessor
{
    private readonly string[] _separators = new string[]
    {
        "\r\n",
        "\n"
    };

    private readonly IRegexService _regexService;

    public MessageProcessor(IRegexService regexService)
    {
        _regexService = regexService;
    }

    public string GetHtml(string text, int? messageId = null)
    {
        var lines = text
            .Split(_separators, StringSplitOptions.TrimEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => $"<p>{s}</p>");

        var html = string.Join(string.Empty, lines);
        html = _regexService.ReplaceUrlsByHref(html);

        return html;
    }
}
