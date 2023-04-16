using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Core.Abstractions;

public interface IMessageProcessor
{
    void SetTextHtml(Message message, out string? url);
}
