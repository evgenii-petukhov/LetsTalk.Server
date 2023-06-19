using LetsTalk.Server.FileStorage.Models;

namespace LetsTalk.Server.FileStorage.Abstractions;

public interface IImageFileNameGenerator
{
    string GetImagePath(string filename);

    FilePathInfo Generate();
}
