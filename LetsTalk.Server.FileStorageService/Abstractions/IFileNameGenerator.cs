using LetsTalk.Server.FileStorage.Models;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IFileNameGenerator
{
    FilePathInfo Generate(FileStorageItemType fileType);

    string GetFilePath(string filename, FileStorageItemType fileType);
}
