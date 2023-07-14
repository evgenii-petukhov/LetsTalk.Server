using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IIOService
{
    void DeleteFile(string filename, FileTypes fileType);
}
