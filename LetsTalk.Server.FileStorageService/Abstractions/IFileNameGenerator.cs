using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IFileNameGenerator
{
    (string, string) Generate(FileTypes fileType);

    string GetFilePath(string filename, FileTypes fileType);
}
