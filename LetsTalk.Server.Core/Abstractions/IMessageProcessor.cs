using LetsTalk.Server.Core.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IMessageProcessor
{
    MessageProcessingResult ConvertToHtml(string text);
}
