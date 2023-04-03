using LetsTalk.Server.LinkPreview.Models;

namespace LetsTalk.Server.LinkPreview.Abstractions;

public interface IRegexService
{
    OpenGraphModel GetOpenGraphModel(string input);
}
