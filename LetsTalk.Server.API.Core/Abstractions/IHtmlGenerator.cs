using LetsTalk.Server.API.Core.Models.HtmlGenerator;

namespace LetsTalk.Server.API.Core.Abstractions;

public interface IHtmlGenerator
{
    HtmlGeneratorResult GetHtml(string text);
}
