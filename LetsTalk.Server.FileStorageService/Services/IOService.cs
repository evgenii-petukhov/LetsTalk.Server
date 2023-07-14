using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Services;

public class IOService : IIOService
{
    private readonly IFileStoragePathProvider _filePathGenerator;

    public IOService(IFileStoragePathProvider filePathGenerator)
    {
        _filePathGenerator = filePathGenerator;
    }

    public void DeleteFile(string filename, FileTypes fileType)
    {
        var path = _filePathGenerator.GetFilePath(filename, fileType);
        File.Delete(path);
    }
}
