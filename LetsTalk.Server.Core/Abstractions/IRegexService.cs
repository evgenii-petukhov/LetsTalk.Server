using LetsTalk.Server.Core.Models.HtmlGenerator;

namespace LetsTalk.Server.Core.Abstractions;

public interface IRegexService
{
    HtmlGeneratorResult ReplaceUrlsByHref(string input);
}