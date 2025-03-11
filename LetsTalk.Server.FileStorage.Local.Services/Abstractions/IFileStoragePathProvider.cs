using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Local.Services.Abstractions;

public interface IFileStoragePathProvider
{
    string GetFilePath(string filename, FileTypes fileType);
}