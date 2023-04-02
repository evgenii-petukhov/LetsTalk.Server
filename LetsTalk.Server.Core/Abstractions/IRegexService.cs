using LetsTalk.Server.Core.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IRegexService
{
    MessageProcessingResult ReplaceUrlsByHref(string input);
}