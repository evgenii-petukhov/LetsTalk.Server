using LetsTalk.Server.Core.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IHtmlGenerator
{
    HtmlGeneratorResult GetHtml(string text);
}
