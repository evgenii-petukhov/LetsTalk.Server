namespace LetsTalk.Server.API.Core.Abstractions;

public interface IRegexService
{
    (string html, string url) ReplaceUrlsByHref(string input);

    (string wrapped, int count, bool emojisOnly) WrapEmojisWithSpan(string input);
}