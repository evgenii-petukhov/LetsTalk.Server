using LetsTalk.Server.Core.Models.HtmlGenerator;

namespace LetsTalk.Server.Core.Abstractions;

public interface IHtmlGenerator
{
    HtmlGeneratorResult GetHtml(string text);
}
