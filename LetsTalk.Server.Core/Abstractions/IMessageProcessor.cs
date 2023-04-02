namespace LetsTalk.Server.Core.Abstractions;

public interface IMessageProcessor
{
    Task<string> GetHtml(string text, int? messageId = null);
}
