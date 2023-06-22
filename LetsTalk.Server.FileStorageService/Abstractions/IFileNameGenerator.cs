using LetsTalk.Server.FileStorageService.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IFileNameGenerator
{
    FilePathInfo Generate(FileTypes fileType);

    string GetFilePath(string filename, FileTypes fileType);
}
