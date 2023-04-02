using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Persistence.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class MessageProcessor : IMessageProcessor
{
    private readonly string[] _separators = new string[]
    {
        "\r\n",
        "\n"
    };
    private readonly IMessageRepository _messageRepository;
    private readonly IRegexService _messageParsingService;

    public MessageProcessor(
        IMessageRepository messageRepository,
        IRegexService messageParsingService)
    {
        _messageRepository = messageRepository;
        _messageParsingService = messageParsingService;
    }

    public async Task<string> GetHtml(string text, int? messageId = null)
    {
        var lines = text
            .Split(_separators, StringSplitOptions.TrimEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => $"<p>{s}</p>");

        var html = string.Join(string.Empty, lines);
        html = _messageParsingService.ReplaceUrlsByHref(html);

        if (messageId.HasValue)
        {
            await _messageRepository.SetTextHtmlAsync(messageId.Value, html);
        }

        return html;
    }
}
