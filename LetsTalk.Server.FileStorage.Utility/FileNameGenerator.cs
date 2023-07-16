using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Utility;

public class FileNameGenerator : IFileNameGenerator
{
    private readonly IFileStoragePathProvider _fileStoragePathProvider;

    public FileNameGenerator(IFileStoragePathProvider fileStoragePathProvider)
    {
        _fileStoragePathProvider = fileStoragePathProvider;
    }

    public (string, string) Generate(FileTypes fileType)
    {
        var filename = Guid.NewGuid().ToString();
        return (filename, _fileStoragePathProvider.GetFilePath(filename, fileType));
    }
}
