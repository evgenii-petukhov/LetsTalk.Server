using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Service.Abstractions;

public interface IIOService
{
    void DeleteFile(string filename, FileTypes fileType);
}
