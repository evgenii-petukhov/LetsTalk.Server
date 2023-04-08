using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Models;

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

    public MessageProcessingResult ConvertToHtml(string text)
    {
        var lines = text
            .Split(_separators, StringSplitOptions.TrimEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => $"<p>{s}</p>");

        var html = string.Concat(lines);
        return _regexService.ReplaceUrlsByHref(html);
    }
}
