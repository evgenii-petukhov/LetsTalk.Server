using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Utility.Abstractions;

public interface IFileStoragePathProvider
{
    string GetFilePath(string filename, FileTypes fileType);
}