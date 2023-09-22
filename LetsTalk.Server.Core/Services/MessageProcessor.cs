using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Core.Services;

public class MessageProcessor : IMessageProcessor
{
    private readonly IHtmlGenerator _htmlGenerator;

    public MessageProcessor(IHtmlGenerator htmlGenerator)
    {
        _htmlGenerator = htmlGenerator;
    }

    public void SetTextHtml(Message message, out string? url)
    {
        (var html, url) = _htmlGenerator.GetHtml(message.Text!);
        message.SetTextHtml(html);
    }
}
