using LetsTalk.Server.Core.Models;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IImageFileNameGenerator
{
    FilePathInfo Generate(ImageContentTypes contentType);
}
