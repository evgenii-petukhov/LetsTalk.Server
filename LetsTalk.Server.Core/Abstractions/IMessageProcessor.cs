namespace LetsTalk.Server.Core.Abstractions;

public interface IMessageProcessor
{
    string GetHtml(string text, int? messageId = null);
}
