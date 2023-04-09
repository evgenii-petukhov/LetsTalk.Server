using LetsTalk.Server.Core.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IRegexService
{
    HtmlGeneratorResult ReplaceUrlsByHref(string input);
}