using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Service.Services;

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
