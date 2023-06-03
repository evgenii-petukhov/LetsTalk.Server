using LetsTalk.Server.FileStorage.Models;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.FileStorage.Abstractions;

public interface IImageFileNameGenerator
{
    string GetImagePath(string filename);

    FilePathInfo Generate(ImageContentTypes contentType);
}
