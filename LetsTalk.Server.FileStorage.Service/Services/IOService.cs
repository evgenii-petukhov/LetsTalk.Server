using LetsTalk.Server.FileStorage.Local.Services.Abstractions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Service.Services;

public class IOService(IFileStoragePathProvider filePathGenerator) : IIOService
{
    private readonly IFileStoragePathProvider _filePathGenerator = filePathGenerator;

    public void DeleteFile(string filename, FileTypes fileType)
    {
        var path = _filePathGenerator.GetFilePath(filename, fileType);
        File.Delete(path);
    }
}
