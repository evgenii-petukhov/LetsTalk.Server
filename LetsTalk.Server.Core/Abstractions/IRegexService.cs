namespace LetsTalk.Server.Core.Abstractions;

public interface IRegexService
{
    string ReplaceUrlsByHref(string input);
}