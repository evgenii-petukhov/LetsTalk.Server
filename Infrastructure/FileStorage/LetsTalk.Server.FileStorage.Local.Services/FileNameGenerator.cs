using LetsTalk.Server.FileStorage.Local.Services.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Local.Services;

public class FileNameGenerator(IFileStoragePathProvider fileStoragePathProvider) : IFileNameGenerator
{
    private readonly IFileStoragePathProvider _fileStoragePathProvider = fileStoragePathProvider;

    public (string, string) Generate(FileTypes fileType)
    {
        var filename = Guid.NewGuid().ToString();
        return (filename, _fileStoragePathProvider.GetFilePath(filename, fileType));
    }
}
