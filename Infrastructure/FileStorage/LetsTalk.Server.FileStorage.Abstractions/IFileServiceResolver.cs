using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Abstractions;

public interface IFileServiceResolver
{
    IFileService Resolve();

    IFileService Resolve(FileStorageTypes fileStorageType);
}
